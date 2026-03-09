using BookStore.Catalog.Domain.Events;
using BookStore.Catalog.IntegrationEvents.Events;
using BuildingBlocks.Chassis.EventBus;
using BuildingBlocks.Chassis.EventBus.Dispatcher;

namespace BookStore.Catalog.Infrastructure.Services;

public class EventMapper(ILogger<EventMapper> logger) : IEventMapper
{
    public IntegrationEvent MapToIntegrationEvent(DomainEvent domainEvent)
    {
        logger.LogInformation(
            "EventMapper: Mapping domain event {DomainEventType}",
            domainEvent.GetType().Name
        );

        var integrationEvent = domainEvent switch
        {
            BookCreatedEvent e => new BookCreateIntegrationEvent
            {
                OrderId = e.Book.Id,
                Name = e.Book.Name,
                Description = e.Book.Description
            },
            _ => throw new NotImplementedException(
                $"No integration event mapping found for {domainEvent.GetType().Name}")
        };

        logger.LogInformation(
            "EventMapper: Successfully mapped {DomainEventType} to {IntegrationEventType}",
            domainEvent.GetType().Name,
            integrationEvent.GetType().Name
        );

        return integrationEvent;
    }
}
