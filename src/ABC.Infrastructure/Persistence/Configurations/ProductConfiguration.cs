using ABC.Application.Common.Constants;
using ABC.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ABC.Infrastructure.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.Property(c => c.Name)
            .HasMaxLength(ValidationConstants.MaxTermLength)
            .IsRequired();

        builder.HasOne(p => p.Rating)
            .WithOne()
            .HasForeignKey<ProductRating>(pr => pr.ProductId);
        
        builder.HasMany(p => p.Feedbacks)
            .WithOne()
            .HasForeignKey(f => f.ProductId)
            .IsRequired();
    }
}