using BuildingBlocks.Chassis.Repository;

namespace BookStore.Basket.Domain.AggregateModels.OrderAggregate;

public interface IOrderRepository : IRepository<Order>, IUnitOfWork
{
    Task AddAsync(Order order, CancellationToken cancellationToken);

    Task<string> GenerateAsync(CancellationToken cancellationToken);
}
