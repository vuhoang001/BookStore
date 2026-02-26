namespace BuildingBlocks.Chassis.Specification.Builder;

public interface IIncludeSpecificationBuilder<T, out TProperty> : ISpecificationBuilder<T> where T : class
{
}
