using FluentResults;
using MediatR;

namespace ABC.Application.ProductRatings.Commands.MarkRatingsOutdated;

/// <summary>
/// Returns the number of product ratings affected.
/// </summary>
/// <param name="NewSentimentTermId"></param>
public record MarkRatingsOutdatedCommand(Guid NewSentimentTermId) : IRequest<Result<int>>;