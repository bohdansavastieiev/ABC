namespace ABC.Application.ProductRatings.Common;

public class RatingRecalculationOptions
{
    public const string SectionName = "RatingRecalculation";
    
    public int BatchSize { get; set; }
    public bool EnableScheduledRecalculation { get; set; }
    public int ScheduleIntervalDays { get; set; }
    public int ScheduleHourUtc { get; set; }
    public int ScheduleMinuteUtc { get; set; }
}