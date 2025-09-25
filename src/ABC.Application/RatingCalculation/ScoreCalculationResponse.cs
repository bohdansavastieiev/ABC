namespace ABC.Application.RatingCalculation;

public record ScoreCalculationResponse
{
    public double? Score { get; }
    public bool HasScore { get; }

    private ScoreCalculationResponse(double? score, bool hasScore)
    {
        Score = score;
        HasScore = hasScore;
    }

    public static ScoreCalculationResponse WithScore(double score) =>
        new(score, true);

    public static ScoreCalculationResponse WithoutScore() =>
        new(null, false);
}