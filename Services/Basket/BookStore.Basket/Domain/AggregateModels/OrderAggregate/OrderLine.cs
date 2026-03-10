using BuildingBlocks.SharedKernel.SeedWork;

namespace BookStore.Basket.Domain.AggregateModels.OrderAggregate;

public class OrderLine : Entity
{
    public OrderLine(Guid bookId, string bookName, int quantity, decimal unitPrice)
    {
        BookId = bookId;
        BookName = bookName;
        Quantity = quantity;
        UnitPrice = unitPrice;
        CalTotal();
    }

    public void Update(string bookName, int quantity, decimal unitPrice)
    {
        this.BookName = bookName;
        this.Quantity = quantity;
        this.UnitPrice = unitPrice;
        CalTotal();
    }

    public Guid BookId { get; private set; }
    public string BookName { get; private set; }
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal TotalPrice { get; private set; }

    public void CalTotal()
    {
        TotalPrice = Quantity * UnitPrice;
    }
}
