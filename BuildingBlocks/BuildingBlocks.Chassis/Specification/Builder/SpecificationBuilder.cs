namespace BuildingBlocks.Chassis.Specification.Builder;

internal class SpecificationBuilder<T>(Specification<T> specification) : ISpecificationBuilder<T> where T : class
{
    public Specification<T> Specification { get; } = specification;
}
