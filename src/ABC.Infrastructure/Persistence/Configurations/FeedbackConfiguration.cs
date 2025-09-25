using ABC.Application.Common;
using ABC.Application.Common.Constants;
using ABC.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ABC.Infrastructure.Persistence.Configurations;

public class FeedbackConfiguration : IEntityTypeConfiguration<Feedback>
{
    public void Configure(EntityTypeBuilder<Feedback> builder)
    {
        builder.Property(f => f.Text)
            .HasMaxLength(ValidationConstants.MaxTextLength)
            .IsRequired();
        
        builder.Property(f => f.CustomerName)
            .HasMaxLength(ValidationConstants.MaxNameLength)
            .IsRequired();
        
        builder.Property(f => f.IsActive)
            .IsRequired();
        
        builder.HasIndex(f => new { f.ProductId, f.IsActive })
            .HasDatabaseName("ix_feedbacks_productId_isActive");
    }
}