namespace BuildingBlocks.Chassis.Specification.Builder;

public interface ISpecificationBuilder<T>
    where T : class
{
    Specification<T> Specification { get; }
}
