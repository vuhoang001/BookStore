using BookStore.Basket.Domain.AggregateModels.OrderAggregate.Rules;
using BuildingBlocks.SharedKernel.SeedWork;

namespace BookStore.Basket.Domain.AggregateModels.OrderAggregate;

public class Order : AuditableEntity, IAggregateRoot, ISoftDelete
{
    private Order() { }

    public Order(string orderCode)
    {
        if (string.IsNullOrWhiteSpace(orderCode))
            throw new ArgumentException("Order code cannot be null or empty.", nameof(orderCode));

        Id = Guid.NewGuid();
        OrderCode = orderCode;
    }


    public static Order NewOrder(
        string orderCode,
        List<(Guid BookId, string BookName, int Quantity, decimal UnitPrice)> lines)
    {
        if (lines is null || lines.Count == 0)
            throw new ArgumentException("Order must have at least one item.", nameof(lines));

        var order = new Order(orderCode);

        foreach (var line in lines)
            order.AddOrUpdateOrderLine(line.BookId, line.BookName, line.Quantity, line.UnitPrice);

        return order;
    }


    private readonly List<OrderLine> _orderLines = [];

    public IReadOnlyCollection<OrderLine> OrderLines => _orderLines.AsReadOnly();

    public string OrderCode { get; private set; } = default!;
    public decimal TotalPrice { get; private set; }
    public bool IsDeleted { get; set; }


    public void AddOrUpdateOrderLine(Guid bookId, string bookName, int quantity, decimal unitPrice)
    {
        var existing = _orderLines.FirstOrDefault(ol => ol.BookId == bookId);

        if (existing is null)
            _orderLines.Add(new OrderLine(bookId, bookName, quantity, unitPrice));
        else
            existing.Update(bookName, quantity, unitPrice);

        RecalculateTotalPrice();
    }

    public void RemoveOrderLine(Guid bookId)
    {
        var orderLine = _orderLines.FirstOrDefault(ol => ol.BookId == bookId);
        if (orderLine is null) return;

        _orderLines.Remove(orderLine);

        if (_orderLines.Count > 0)
            RecalculateTotalPrice();
        else
            TotalPrice = 0;
    }

    public void Delete() => IsDeleted = true;


    private void RecalculateTotalPrice()
    {
        CheckRule(new OrderHasAtLeastOneItem(this));
        TotalPrice = _orderLines.Sum(ol => ol.TotalPrice);
    }
}
