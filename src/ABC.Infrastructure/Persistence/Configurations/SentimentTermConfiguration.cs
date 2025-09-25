using ABC.Application.Common.Constants;
using ABC.Domain.Common;
using ABC.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ABC.Infrastructure.Persistence.Configurations;

public class SentimentTermConfiguration : IEntityTypeConfiguration<SentimentTerm>
{
    public void Configure(EntityTypeBuilder<SentimentTerm> builder)
    {
        builder.Property(c => c.Term)
            .HasMaxLength(ValidationConstants.MaxTermLength)
            .IsRequired()
            .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
        
        builder.HasIndex(c => c.Term)
            .IsUnique()
            .HasDatabaseName("ix_sentiment_terms_term");
        
        builder.Property(c => c.IsActive)
            .IsRequired();
        
        builder.Property(c => c.Score)
            .IsRequired();

        builder.ToTable("sentiment_terms", table => 
            table.HasCheckConstraint(
                $"ck_sentiment_terms_score_range",
                $"score >= {DomainConstants.MinScore} AND score <= {DomainConstants.MaxScore}"));
        
        builder.HasIndex(c => new { c.Score, c.Term })
            .HasDatabaseName("ix_sentiment_terms_score_term");
    }
}