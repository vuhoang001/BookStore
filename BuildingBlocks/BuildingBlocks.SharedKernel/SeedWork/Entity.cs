using BuildingBlocks.SharedKernel.Exceptions;

namespace BuildingBlocks.SharedKernel.SeedWork;

public abstract class Entity : HasDomainEvents
{
    public Guid Id { get; set; }

    protected void CheckRule(IBusinessRule rule)
    {
        if (rule.IsBroken())
        {
            throw new BusinessRuleValidationException(rule);
        }
    }
}

public abstract class Entity<TId> : Entity
    where TId : IEquatable<TId>
{
    public new TId Id { get; set; } = default!;
}
