namespace ABC.Application;

public class ApplicationOptionsBuilder
{
    internal bool IsRatingServiceEnabled { get; private set; }
    
    public ApplicationOptionsBuilder WithRatingService()
    {
        IsRatingServiceEnabled = true;
        return this;
    }
}