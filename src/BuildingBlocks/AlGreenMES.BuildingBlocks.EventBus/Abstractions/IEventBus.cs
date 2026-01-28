namespace AlGreenMES.BuildingBlocks.EventBus.Abstractions;

/// <summary>
/// Interface for publishing domain events.
/// </summary>
public interface IEventBus
{
    Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : IDomainEvent;
}
