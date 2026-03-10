namespace BookStore.Basket.Dtos;

public class CreateOrderLineDto
{
    public Guid BookId { get; set; }
    public string BookName { get; set; } = default!;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}
