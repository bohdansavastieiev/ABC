using System.Text.RegularExpressions;
using ABC.Application.SentimentTerms.Common;
using ABC.Domain.Entities;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace ABC.Application.RatingCalculation;

public class RatingService(
    ISentimentTermCache sentimentTermCache,
    ILogger<RatingService> logger)
    : IRatingService
{
    public async Task<Result<ScoreCalculationResponse>> CalculateFeedbackScoreAsync(
        string feedbackText,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(feedbackText))
        {
            logger.LogWarning("Empty feedback text provided, no score calculated");
            return Result.Ok(ScoreCalculationResponse.WithoutScore());
        }

        var sentimentTerms = await sentimentTermCache.GetTermsAsync(cancellationToken);
        if (!sentimentTerms.Any())
        {
            logger.LogWarning("No sentiment terms configured in the system");
            return Result.Ok(ScoreCalculationResponse.WithoutScore());
        }

        var score = CalculateSingleFeedbackScore(feedbackText, sentimentTerms);
        if (score.HasValue)
        {
            logger.LogInformation("Feedback scored {Score} based on sentiment terms", score.Value);
            return Result.Ok(ScoreCalculationResponse.WithScore(score.Value));
        }
        
        logger.LogDebug("No sentiment terms found in feedback text");
        return Result.Ok(ScoreCalculationResponse.WithoutScore());
    }

    public async Task<Result<ScoreCalculationResponse>> CalculateProductRatingAsync(
        IEnumerable<string> feedbackTexts,
        CancellationToken cancellationToken = default)
    {
        var sentimentTerms = await sentimentTermCache.GetTermsAsync(cancellationToken);
        if (!sentimentTerms.Any())
        {
            logger.LogWarning("No sentiment terms configured in the system");
            return Result.Ok(ScoreCalculationResponse.WithoutScore());
        }

        var feedbackList = feedbackTexts.ToList();
        if (feedbackList.Count == 0)
        {
            logger.LogDebug("No feedback texts provided for rating calculation");
            return Result.Ok(ScoreCalculationResponse.WithoutScore());
        }

        var scores = new List<double>();

        foreach (var feedbackText in feedbackList)
        {
            if (string.IsNullOrWhiteSpace(feedbackText))
            {
                continue;
            }

            var score = CalculateSingleFeedbackScore(feedbackText, sentimentTerms);
            if (score.HasValue)
            {
                scores.Add(score.Value);
            }
        }

        if (scores.Count == 0)
        {
            logger.LogInformation("No sentiment terms found in any feedback, no rating calculated");
            return Result.Ok(ScoreCalculationResponse.WithoutScore());
        }

        var averageRating = scores.Average();
        var roundedRating = Math.Round(averageRating, 1);
        
        logger.LogInformation("Product rating calculated: {Rating} from {Count} feedbacks with scores", 
            roundedRating, scores.Count);
        
        return Result.Ok(ScoreCalculationResponse.WithScore(averageRating));
    }

    private double? CalculateSingleFeedbackScore(string feedbackText, IReadOnlyList<SentimentTerm> sentimentTerms)
    {
        var sortedTerms = sentimentTerms
            .OrderByDescending(t => t.Term.Length)
            .ToList();

        var matches = new List<TermMatch>();
        
        foreach (var term in sortedTerms)
        {
            var pattern = $@"\b{Regex.Escape(term.Term)}\b";
            var regex = new Regex(pattern, RegexOptions.IgnoreCase);
            var regexMatches = regex.Matches(feedbackText);
            
            foreach (Match match in regexMatches)
            {
                matches.Add(new TermMatch
                {
                    Term = term.Term,
                    Score = term.Score,
                    StartIndex = match.Index,
                    EndIndex = match.Index + match.Length,
                    Length = match.Length
                });
            }
        }

        if (matches.Count == 0)
        {
            return null;
        }

        matches = matches
            .OrderByDescending(m => m.Length)
            .ThenBy(m => m.StartIndex)
            .ToList();

        var consumedRanges = new List<(int start, int end)>();
        var validMatches = new List<TermMatch>();

        foreach (var match in matches)
        {
            var overlaps = consumedRanges.Any(range => 
                match.StartIndex < range.end && match.EndIndex > range.start);
            if (overlaps) continue;
            
            validMatches.Add(match);
            consumedRanges.Add((match.StartIndex, match.EndIndex));
        }

        if (validMatches.Count == 0)
        {
            return null;
        }
        
        var averageScore = validMatches.Average(m => m.Score);
        
        logger.LogInformation("Feedback scored {Score} with {Count} term matches: {Terms}", 
            averageScore, 
            validMatches.Count,
            string.Join(", ", validMatches.Select(m => $"{m.Term}({m.Score})")));
        
        return averageScore;
    }

    private class TermMatch
    {
        public string Term { get; init; } = string.Empty;
        public double Score { get; init; }
        public int StartIndex { get; init; }
        public int EndIndex { get; init; }
        public int Length { get; init; }
    }
}