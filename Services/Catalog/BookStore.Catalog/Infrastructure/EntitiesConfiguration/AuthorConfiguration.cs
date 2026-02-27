using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookStore.Catalog.Infrastructure.EntitiesConfiguration;

public class AuthorConfiguration : IEntityTypeConfiguration<Author>
{
    public void Configure(EntityTypeBuilder<Author> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(p => p.Id).HasDefaultValueSql(UniqueIdentifierHelper.NewUuidV7);

        builder.Property(p => p.Name).HasMaxLength(DataSchemaLength.Large).IsRequired();

        builder.HasIndex(e => e.Name).IsUnique();
    }
}
