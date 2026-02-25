namespace BuildingBlocks.SharedKernel.SeedWork;

public interface IHasDomainEvents
{
   IReadOnlyCollection<DomainEvent> DomainEvents { get; }
}
