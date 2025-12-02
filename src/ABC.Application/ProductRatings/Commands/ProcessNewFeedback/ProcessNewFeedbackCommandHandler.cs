using ABC.Application.Abstractions;
using ABC.Application.Abstractions.Persistence;
using ABC.Application.Common.Errors.Common;
using ABC.Application.Common.Exceptions;
using ABC.Application.Feedbacks.Common;
using ABC.Application.ProductRatings.Common;
using ABC.Application.RatingCalculation;
using ABC.Domain.Entities;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ABC.Application.ProductRatings.Commands.ProcessNewFeedback;

public class ProcessNewFeedbackCommandHandler(
    IRatingService ratingService,
    IFeedbackRepository feedbackRepository,
    IProductRatingRepository productRatingRepository,
    IUnitOfWork unitOfWork,
    ILogger<ProcessNewFeedbackCommandHandler> logger)
    : IRequestHandler<ProcessNewFeedbackCommand, Result>
{
    public async Task<Result> Handle(ProcessNewFeedbackCommand request, CancellationToken cancellationToken)
    {
        var newFeedback = await feedbackRepository.GetByIdAsync(request.FeedbackId, cancellationToken);
        if (newFeedback is null)
        {
            logger.LogWarning("Feedback with ID {FeedbackId} not found", request.FeedbackId);
            return Result.Fail(new NotFoundError(nameof(Feedback), nameof(Feedback.Id), request.FeedbackId));
        }
        
        var feedbackScoreResult = await ratingService.CalculateFeedbackScoreAsync(newFeedback.Text, cancellationToken);
        if (feedbackScoreResult.IsFailed)
        {
            logger.LogWarning(
                "Feedback score calculation service failed for FeedbackId: {FeedbackId}. Reasons: {Reasons}",
                request.FeedbackId,
                string.Join(", ", feedbackScoreResult.Errors.Select(e => e.Message)));
            return Result.Fail(feedbackScoreResult.Errors);
        }

        if (!feedbackScoreResult.Value.HasScore)
        {
            logger.LogInformation(
                "Score wasn't retrieved from the feedback based on the current sentiment terms. FeedbackId: {FeedbackId}.",
                request.FeedbackId);
            return Result.Ok();
        }
        
        var feedbackTexts = await feedbackRepository
            .GetFeedbackTextsForProductAsync(newFeedback.ProductId, cancellationToken);

        var productRatingResult = await ratingService.CalculateProductRatingAsync(feedbackTexts, cancellationToken);
        if (productRatingResult.IsFailed)
        {
            logger.LogWarning(
                "Rating calculation service failed for ProductId: {ProductId}. Reasons: {Reasons}",
                newFeedback.ProductId,
                string.Join(", ", productRatingResult.Errors.Select(e => e.Message)));
            return Result.Fail(productRatingResult.Errors);
        }

        if (!productRatingResult.Value.HasScore)
        {
            throw new ServiceContractViolationException(
                nameof(IRatingService),
                "The service was expected to return a score when calculating a product rating but failed to do so.");
        }
        
        var productRating = await productRatingRepository.GetByProductIdAsync(newFeedback.ProductId, cancellationToken);
        if (productRating is null)
        {
            throw new UnexpectedSystemState("Feedback contains Product ID with a non-existent rating");
        }
        
        logger.LogInformation(
            "Existing rating of {OldRating} found for ProductId: {ProductId}. Updating to {NewRating}.",
            productRating.Value,
            newFeedback.ProductId,
            productRatingResult.Value);
            
        productRating.Value = productRatingResult.Value.Score!.Value;
        productRating.IsOutdated = false;
        productRating.IsCalculated = true;
        
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        return Result.Ok();
    }
}