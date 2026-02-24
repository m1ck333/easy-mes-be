using System.Net;
using System.Text.Json;
using AlGreenMES.Modules.Orders.Application.Interfaces;
using AlGreenMES.Modules.Orders.Domain.Repositories;
using Lib.Net.Http.WebPush;
using Lib.Net.Http.WebPush.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AlGreenMES.Modules.Orders.Infrastructure.Services;

public class WebPushService : IWebPushService
{
    private readonly PushServiceClient _client;
    private readonly WebPushSettings _settings;
    private readonly IPushSubscriptionRepository _subscriptionRepository;
    private readonly IOrdersUnitOfWork _unitOfWork;
    private readonly ILogger<WebPushService> _logger;
    private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public WebPushService(
        IOptions<WebPushSettings> settings,
        IPushSubscriptionRepository subscriptionRepository,
        IOrdersUnitOfWork unitOfWork,
        ILogger<WebPushService> logger)
    {
        _settings = settings.Value;
        _subscriptionRepository = subscriptionRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;

        _client = new PushServiceClient();
        if (_settings.Enabled && !string.IsNullOrEmpty(_settings.VapidPublicKey))
        {
            _client.DefaultAuthentication = new VapidAuthentication(
                _settings.VapidPublicKey,
                _settings.VapidPrivateKey)
            {
                Subject = _settings.VapidSubject
            };
        }
    }

    public string GetVapidPublicKey() => _settings.VapidPublicKey;

    public async Task SubscribeAsync(Guid tenantId, Guid userId, string endpoint, string p256dhKey, string authKey, CancellationToken cancellationToken = default)
    {
        var existing = await _subscriptionRepository.FindByEndpointActiveAsync(endpoint, cancellationToken);
        if (existing != null)
        {
            existing.UpdateKeys(p256dhKey, authKey);
        }
        else
        {
            var subscription = Domain.Entities.PushSubscription.Create(tenantId, userId, endpoint, p256dhKey, authKey);
            await _subscriptionRepository.AddAsync(subscription, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task UnsubscribeAsync(string endpoint, CancellationToken cancellationToken = default)
    {
        var subscription = await _subscriptionRepository.FindByEndpointActiveAsync(endpoint, cancellationToken);
        if (subscription != null)
        {
            subscription.Deactivate();
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task SendToUserAsync(Guid userId, string title, string body, object? data = null, CancellationToken cancellationToken = default)
    {
        if (!_settings.Enabled) return;

        var subscriptions = await _subscriptionRepository.GetByUserIdActiveAsync(userId, cancellationToken);
        await SendToSubscriptionsAsync(subscriptions, title, body, data, cancellationToken);
    }

    public async Task SendToUsersAsync(IEnumerable<Guid> userIds, string title, string body, object? data = null, CancellationToken cancellationToken = default)
    {
        if (!_settings.Enabled) return;

        var subscriptions = await _subscriptionRepository.GetByUserIdsActiveAsync(userIds, cancellationToken);
        await SendToSubscriptionsAsync(subscriptions, title, body, data, cancellationToken);
    }

    public async Task SendToTenantAsync(Guid tenantId, string title, string body, object? data = null, CancellationToken cancellationToken = default)
    {
        if (!_settings.Enabled) return;

        var subscriptions = await _subscriptionRepository.GetByTenantIdActiveAsync(tenantId, cancellationToken);
        await SendToSubscriptionsAsync(subscriptions, title, body, data, cancellationToken);
    }

    private async Task SendToSubscriptionsAsync(IReadOnlyList<Domain.Entities.PushSubscription> subscriptions, string title, string body, object? data, CancellationToken cancellationToken)
    {
        if (subscriptions.Count == 0) return;

        var payload = JsonSerializer.Serialize(new { title, body, data }, _jsonOptions);
        var needsSave = false;

        foreach (var sub in subscriptions)
        {
            try
            {
                var pushSubscription = new Lib.Net.Http.WebPush.PushSubscription
                {
                    Endpoint = sub.Endpoint,
                    Keys = { ["p256dh"] = sub.P256dhKey, ["auth"] = sub.AuthKey }
                };

                var message = new PushMessage(payload) { Urgency = PushMessageUrgency.High };
                await _client.RequestPushMessageDeliveryAsync(pushSubscription, message, cancellationToken);
            }
            catch (PushServiceClientException ex) when (ex.StatusCode == HttpStatusCode.Gone || ex.StatusCode == HttpStatusCode.NotFound)
            {
                _logger.LogInformation("Push subscription expired for user {UserId}, endpoint {Endpoint}", sub.UserId, sub.Endpoint);
                sub.Deactivate();
                needsSave = true;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to send push notification to user {UserId}, endpoint {Endpoint}", sub.UserId, sub.Endpoint);
            }
        }

        if (needsSave)
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
