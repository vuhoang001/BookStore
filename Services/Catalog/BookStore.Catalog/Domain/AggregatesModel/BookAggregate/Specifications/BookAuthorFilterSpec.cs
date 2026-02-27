using BookStore.Catalog.Domain.AggregatesModel.AuthorAggregate;
using BuildingBlocks.Chassis.Specification;
using BuildingBlocks.Chassis.Specification.Builder;

namespace BookStore.Catalog.Domain.AggregatesModel.BookAggregate.Specifications;

public sealed class BookAuthorFilterSpec : Specification<Author>
{
    public BookAuthorFilterSpec(Guid id)
    {
        Query.Where(x => x.BookAuthors.Any(y => y.AuthorId == id));
    }
}
