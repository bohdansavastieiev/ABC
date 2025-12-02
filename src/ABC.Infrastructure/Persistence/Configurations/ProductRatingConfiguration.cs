using ABC.Domain.Common;
using ABC.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ABC.Infrastructure.Persistence.Configurations;

public class ProductRatingConfiguration : IEntityTypeConfiguration<ProductRating>
{
    public void Configure(EntityTypeBuilder<ProductRating> builder)
    {
        builder.Property(c => c.IsCalculated)
            .IsRequired();
        
        builder.Property(c => c.IsOutdated)
            .IsRequired();
        
        builder.ToTable("product_ratings", table => 
            table.HasCheckConstraint(
                $"ck_product_ratings_rating_range",
                $"value IS NULL OR (value >= {DomainConstants.MinScore} AND value <= {DomainConstants.MaxScore})"));
        
        builder.Property<uint>("Version").IsRowVersion();
        
        builder.HasIndex(f => f.ProductId)
            .IsUnique()
            .HasDatabaseName("ix_product_ratings_product_id");
        
        builder.HasIndex(pr => pr.IsOutdated)
            .HasDatabaseName("ix_product_ratings_is_outdated_partial")
            .HasFilter("is_outdated = true");
    }
}