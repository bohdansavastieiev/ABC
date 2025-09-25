using System.Reflection;
using System.Reflection.PortableExecutable;
using ABC.Domain.Abstractions;
using ABC.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ABC.Infrastructure.Persistence;

public partial class ApplicationDbContext(
    DbContextOptions<ApplicationDbContext> options) 
    : DbContext(options)
{
    public DbSet<Product> Products { get; set; }
    public DbSet<Feedback> Feedbacks { get; set; }
    public DbSet<SentimentTerm> SentimentTerms { get; set; }
    public DbSet<ProductRating> ProductRatings { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes()
                     .Where(e => typeof(Entity).IsAssignableFrom(e.ClrType)))
        {
            modelBuilder.Entity(entityType.ClrType)
                .HasKey(nameof(Entity.Id));

            modelBuilder.Entity(entityType.ClrType)
                .Property(nameof(Entity.CreatedAt))
                .IsRequired();
            
            modelBuilder.Entity(entityType.ClrType)
                .Property(nameof(Entity.UpdatedAt))
                .IsRequired();
        }
        
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        
        base.OnModelCreating(modelBuilder);
    }
    
    private void OnBeforeSaving()
    {
        var entries = ChangeTracker.Entries<Entity>();
        var utcNow = DateTime.UtcNow;

        foreach (var entry in entries)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = utcNow;
                    entry.Entity.UpdatedAt = utcNow;
                    break;

                case EntityState.Modified:
                    entry.Entity.UpdatedAt = utcNow;
                    break;
            }
        }
    }
    
    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        OnBeforeSaving();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        OnBeforeSaving();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }
}