namespace ABC.Application.Feedbacks.Commands.SubmitFeedback;

public record FeedbackSubmittedEvent(Guid ProductId, Guid FeedbackId);