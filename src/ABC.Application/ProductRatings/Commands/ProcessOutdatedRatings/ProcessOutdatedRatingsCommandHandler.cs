using ABC.Application.Abstractions.Persistence;
using ABC.Application.Products.Common;
using ABC.Application.RatingCalculation;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ABC.Application.ProductRatings.Commands.ProcessOutdatedRatings;

public class ProcessOutdatedRatingsCommandHandler(
    IProductRepository productRepository,
    IUnitOfWork unitOfWork,
    IRatingService ratingService,
    ILogger<ProcessOutdatedRatingsCommandHandler> logger) : IRequestHandler<ProcessOutdatedRatingsCommand, Result<int>>
{
    public async Task<Result<int>> Handle(ProcessOutdatedRatingsCommand request, CancellationToken cancellationToken)
    {
        var totalProcessed = 0;
        
        while (!cancellationToken.IsCancellationRequested)
        {
            var products = await productRepository.GetProductsWithOutdatedRatingAndFeedbacksAsync(
                request.BatchSize, cancellationToken);

            if (products.Count == 0)
            {
                break;
            }
            
            var batchProcessed = 0;
            
            foreach (var product in products)
            {
                var ratingResponse = await ratingService.CalculateProductRatingAsync(
                    product.Feedbacks.Select(f => f.Text), cancellationToken);
                
                if (ratingResponse.IsFailed)
                {
                    logger.LogWarning(
                        "Rating calculation failed for ProductId: {ProductId}. Skipping.",
                        product.Id);
                    continue;
                }

                if (!ratingResponse.Value.HasScore)
                {
                    continue;
                }

                product.Rating.IsCalculated = true;
                product.Rating.Value = ratingResponse.Value.Score!.Value;
                product.Rating.IsOutdated = false;
                
                batchProcessed++;
            }
            
            if (batchProcessed > 0)
            {
                await unitOfWork.SaveChangesAsync(cancellationToken);
                totalProcessed += batchProcessed;
                
                logger.LogInformation(
                    "Processed batch of {BatchSize} ratings, total so far: {Total}", 
                    batchProcessed, totalProcessed);
            }
        }
        
        return Result.Ok(totalProcessed);
    }
}