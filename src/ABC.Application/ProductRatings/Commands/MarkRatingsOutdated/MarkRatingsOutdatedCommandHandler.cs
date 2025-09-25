using ABC.Application.Abstractions;
using ABC.Application.Common.Errors.Common;
using ABC.Application.ProductRatings.Common;
using ABC.Application.SentimentTerms.Common;
using ABC.Domain.Entities;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ABC.Application.ProductRatings.Commands.MarkRatingsOutdated;

public class MarkRatingsOutdatedCommandHandler(
    IProductRatingRepository productRatingRepository,
    ISentimentTermRepository sentimentTermRepository,
    ILogger<MarkRatingsOutdatedCommandHandler> logger) 
    : IRequestHandler<MarkRatingsOutdatedCommand, Result<int>>
{
    public async Task<Result<int>> Handle(MarkRatingsOutdatedCommand request, CancellationToken cancellationToken)
    {
        var sentimentTerm = await sentimentTermRepository.GetByIdAsync(request.NewSentimentTermId, cancellationToken);
        if (sentimentTerm is null)
        {
            logger.LogWarning("Sentiment term with ID {SentimentTermId} not found", request.NewSentimentTermId);
            return Result.Fail(new NotFoundError(
                nameof(SentimentTerm), nameof(SentimentTerm.Id), request.NewSentimentTermId));
        }
        
        var ratingsMarked = await productRatingRepository.MarkOutdatedForTermAsync(sentimentTerm.Term, cancellationToken);
        return Result.Ok(ratingsMarked);
    }
}