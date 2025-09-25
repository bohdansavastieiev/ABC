namespace ABC.Application.Feedbacks.Common;

public record FeedbackDto(
    Guid Id,
    Guid ProductId,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    string CustomerName,
    string Text);