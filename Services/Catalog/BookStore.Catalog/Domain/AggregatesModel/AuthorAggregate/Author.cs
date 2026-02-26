using BuildingBlocks.SharedKernel.SeedWork;

namespace BookStore.Catalog.Domain.AggregatesModel.AuthorAggregate;

public class Author : Entity, IAggregateRoot
{
    // private readonly 
    
    public string? Name { get; private set;  }
    
}
