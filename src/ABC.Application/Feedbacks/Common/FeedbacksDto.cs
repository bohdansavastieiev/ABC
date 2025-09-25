namespace ABC.Application.Feedbacks.Common;

public record FeedbacksDto(Guid ProductId, List<FeedbackDetailsDto> Feedbacks);

public record FeedbackDetailsDto(Guid Id,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    string CustomerName,
    string Text);