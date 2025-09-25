using ABC.Domain.Abstractions;

namespace ABC.Application.Abstractions.Persistence;

public interface IRepository<T> where T : Entity
{
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    void Add(T entity);
}