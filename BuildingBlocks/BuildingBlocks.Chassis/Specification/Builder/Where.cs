using System.Linq.Expressions;
using BuildingBlocks.Chassis.Specification.Expressions;

namespace BuildingBlocks.Chassis.Specification.Builder;

public static partial class SpecificationBuilderExtensions
{
    public static ISpecificationBuilder<T> Where<T>(
        this ISpecificationBuilder<T> builder,
        Expression<Func<T, bool>> predicate
    )
        where T : class
    {
        var expr = new WhereExpression<T>(predicate);
        builder.Specification.Add(expr);
        return builder;
    }
}
