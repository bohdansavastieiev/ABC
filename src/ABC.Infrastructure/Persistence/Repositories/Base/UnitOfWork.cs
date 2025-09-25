using ABC.Application.Abstractions.Persistence;

namespace ABC.Infrastructure.Persistence.Repositories.Base;

public class UnitOfWork(ApplicationDbContext dbContext) : IUnitOfWork
{
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.SaveChangesAsync(cancellationToken);
    }
}