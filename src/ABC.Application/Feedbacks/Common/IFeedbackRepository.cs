using ABC.Application.Abstractions;
using ABC.Application.Abstractions.Persistence;
using ABC.Domain.Entities;

namespace ABC.Application.Feedbacks.Common;

public interface IFeedbackRepository : IRepository<Feedback>
{
    Task<IReadOnlyList<string>> GetFeedbackTextsForProductAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Feedback>> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default);
}