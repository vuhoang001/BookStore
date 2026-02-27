namespace BookStore.Catalog.Domain.AggregatesModel.CategoryAggregate;

public sealed class CategoryData : List<Category>
{
    public CategoryData()
    {
        AddRange([
            new Category("Fiction"),
            new Category("Non-Fiction"),
            new Category("Science Fiction"),
            new Category("Fantasy"),
            new Category("Mystery"),
            new Category("Biography"),
            new Category("History"),
            new Category("Romance"),
            new Category("Self-Help"),
            new Category("Children's Books"),
        ]);
    }
}
