namespace BookStore.Basket.Infrastructure.ReadModels.Books;

public class BookReadModel
{
    public Guid Id { get; set; }
    public string BookName { get; set; } = string.Empty;
    public string? BookDescription { get; set; }
}
