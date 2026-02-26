using System.Diagnostics.CodeAnalysis;
using BookStore.Catalog.Exceptions;
using BuildingBlocks.SharedKernel.SeedWork;

namespace BookStore.Catalog.Domain.AggregatesModel.CategoryAggregate;

public sealed class Category() : Entity, IAggregateRoot
{
    public Category(string name) : this()
    {
        Name = name;
    }

    [DisallowNull]
    public string? Name { get; private set; }

    public Category UpdateName(string name)
    {
        Name = !string.IsNullOrWhiteSpace(name)
            ? name
            : throw new CatalogDomainException("Category name must be provided.");
        return this;
    }
}
