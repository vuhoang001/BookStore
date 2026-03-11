using BookStore.Basket.Domain.AggregateModels.OrderAggregate;
using BookStore.Basket.Infrastructure.ReadModels.Books;
using BuildingBlocks.Chassis.Repository;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Basket.Infrastructure;

public class BasketDbContext(DbContextOptions<BasketDbContext> options) : DbContext(options: options), IUnitOfWork
{
    public DbSet<Order> Order { get; set; }
    public DbSet<OrderLine> OrderLine { get; set; }

    public DbSet<BookReadModel> BookReadModel { get; set; }


    public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
    {
        await SaveChangesAsync(cancellationToken);
        return true;
    }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.AddInboxStateEntity();
        modelBuilder.AddOutboxMessageEntity();
        modelBuilder.AddOutboxStateEntity();
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BasketDbContext).Assembly);
    }
}
