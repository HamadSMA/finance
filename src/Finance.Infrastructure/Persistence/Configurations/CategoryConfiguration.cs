using Finance.Domain;
using Finance.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Finance.Infrastructure.Persistence.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    private static readonly DateTime SeedTimestamp = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("categories");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id).ValueGeneratedNever();

        builder.Property(c => c.Name).IsRequired().HasMaxLength(50);

        builder.Property(c => c.CreatedAt).IsRequired();

        builder.HasIndex(c => c.Name).IsUnique();

        builder.HasData(
            Categories.All.Select(c => new Category
            {
                Id = c.Key,
                Name = c.Value,
                CreatedAt = SeedTimestamp
            })
        );
    }
}
