using AlGreenMES.BuildingBlocks.EventBus.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AlGreenMES.BuildingBlocks.EventBus.InMemory;

/// <summary>
/// In-memory implementation of the event bus for modular monolith communication.
/// </summary>
public class InMemoryEventBus : IEventBus
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<InMemoryEventBus> _logger;

    public InMemoryEventBus(IServiceProvider serviceProvider, ILogger<InMemoryEventBus> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : IDomainEvent
    {
        _logger.LogDebug(
            "Publishing event {EventType} with ID {EventId}",
            typeof(TEvent).Name,
            @event.EventId);

        using var scope = _serviceProvider.CreateScope();

        var handlers = scope.ServiceProvider.GetServices<IEventHandler<TEvent>>();

        foreach (var handler in handlers)
        {
            try
            {
                await handler.HandleAsync(@event, cancellationToken);

                _logger.LogDebug(
                    "Event {EventType} handled by {HandlerType}",
                    typeof(TEvent).Name,
                    handler.GetType().Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error handling event {EventType} by handler {HandlerType}",
                    typeof(TEvent).Name,
                    handler.GetType().Name);

                throw;
            }
        }
    }
}
