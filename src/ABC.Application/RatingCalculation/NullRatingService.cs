using FluentResults;

namespace ABC.Application.RatingCalculation;

public class NullRatingService : IRatingService
{
    public Task<Result<ScoreCalculationResponse>> CalculateFeedbackScoreAsync(string feedbackText, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException(
            "IRatingService is not intended to be used in this application host.");    }

    public Task<Result<ScoreCalculationResponse>> CalculateProductRatingAsync(IEnumerable<string> feedbackTexts, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException(
            "IRatingService is not intended to be used in this application host.");    }
}