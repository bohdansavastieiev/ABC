using ABC.Application.Abstractions;
using ABC.Application.Abstractions.Persistence;
using ABC.Domain.Abstractions;

namespace ABC.Infrastructure.Persistence.Repositories.Base;

public abstract class Repository<T>(ApplicationDbContext dbContext) 
    : IRepository<T> where T : Entity
{
    public async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Set<T>().FindAsync([id], cancellationToken);
    }

    public void Add(T entity)
    {
        dbContext.Set<T>().Add(entity);
    }
}