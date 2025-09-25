using ABC.Domain.Entities;
using FluentResults;

namespace ABC.Application.RatingCalculation;

public interface IRatingService
{
    Task<Result<ScoreCalculationResponse>> CalculateFeedbackScoreAsync(
        string feedbackText, 
        CancellationToken cancellationToken = default);
    Task<Result<ScoreCalculationResponse>> CalculateProductRatingAsync(
        IEnumerable<string> feedbackTexts,
        CancellationToken cancellationToken = default);
}