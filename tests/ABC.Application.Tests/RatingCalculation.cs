using ABC.Application.RatingCalculation;
using ABC.Application.SentimentTerms.Common;
using ABC.Domain.Entities;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace ABC.Application.Tests;

public class RatingServiceTests
{
    private readonly IRatingService _sut;

    private readonly List<SentimentTerm> _sentimentTerms =
    [
        new("excellent", 5),
        new("very good", 5),
        new("good", 4),
        new("average", 3),
        new("bad", 2),
        new("very bad", 1)
    ];

    public RatingServiceTests()
    {
        var sentimentTermCacheMock = new Mock<ISentimentTermCache>();
        var loggerMock = new Mock<ILogger<RatingService>>();

        sentimentTermCacheMock
            .Setup(c => c.GetTermsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(_sentimentTerms);

        _sut = new RatingService(sentimentTermCacheMock.Object, loggerMock.Object);
    }
    
    [Fact]
    public async Task CalculateFeedbackScoreAsync_ShouldReturnAverageScore_WhenMultipleTermsAreFound()
    {
        // Arrange
        const string feedback = "The product is good but the usability is bad."; // good (4), bad (2)

        // Act
        var result = await _sut.CalculateFeedbackScoreAsync(feedback);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.HasScore.Should().BeTrue();
        result.Value.Score.Should().Be((4.0 + 2.0) / 2.0); // 3.0
    }
    
    [Fact]
    public async Task CalculateFeedbackScoreAsync_ShouldPrioritizeLongerTerm_WhenTermsOverlap()
    {
        // Arrange
        const string feedback = "The battery is very bad."; // "very bad" (1) should be chosen over "bad" (2)

        // Act
        var result = await _sut.CalculateFeedbackScoreAsync(feedback);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.HasScore.Should().BeTrue();
        result.Value.Score.Should().Be(1.0);
    }
    
    [Fact]
    public async Task CalculateFeedbackScoreAsync_ShouldReturnWithoutScore_WhenNoTermsAreFound()
    {
        // Arrange
        const string feedback = "This item is perfectly acceptable.";

        // Act
        var result = await _sut.CalculateFeedbackScoreAsync(feedback);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.HasScore.Should().BeFalse();
    }
    
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task CalculateFeedbackScoreAsync_ShouldReturnWithoutScore_WhenFeedbackIsWhitespace(string feedback)
    {
        // Act
        var result = await _sut.CalculateFeedbackScoreAsync(feedback);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.HasScore.Should().BeFalse();
    }
    
    [Fact]
    public async Task CalculateProductRatingAsync_ShouldReturnAverageOfAllFeedbackScores()
    {
        // Arrange
        var feedbacks = new List<string>
        {
            "This is excellent!",               // Score: 5
            "A very good product, but the delivery was bad.", // Score: (5+2)/2 = 3.5
            "Just average.",                    // Score: 3
            "This review has no keywords."      // No score, should be ignored
        };
    
        const double expectedAverage = (5.0 + 3.5 + 3.0) / 3.0;

        // Act
        var result = await _sut.CalculateProductRatingAsync(feedbacks);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.HasScore.Should().BeTrue();
        result.Value.Score.Should().BeApproximately(expectedAverage, 0.001);
    }
}