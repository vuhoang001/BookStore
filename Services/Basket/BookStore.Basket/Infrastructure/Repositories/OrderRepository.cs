using BookStore.Basket.Domain.AggregateModels.OrderAggregate;
using BuildingBlocks.Chassis.Repository;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Basket.Infrastructure.Repositories;

public class OrderRepository(BasketDbContext context) : IOrderRepository
{
    public void Dispose()
    {
        // TODO release managed resources here
    }

    public IUnitOfWork UnitOfWork => context;

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
    {
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task AddAsync(Order order, CancellationToken cancellationToken)
    {
        var res = await context.Order.AddAsync(order, cancellationToken);
    }

    public async Task<string> GenerateAsync(CancellationToken cancellationToken)
    {
        var lastOrder = await context.Order
            .OrderByDescending(o => o.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);

        int nextNumber = 1;

        if (lastOrder != null)
        {
            var number = int.Parse(lastOrder.OrderCode.Replace("ORD", ""));
            nextNumber = number + 1;
        }

        return $"ORD{nextNumber:00000}";
    }
}
