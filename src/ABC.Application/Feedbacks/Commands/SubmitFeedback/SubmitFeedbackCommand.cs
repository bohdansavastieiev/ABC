using ABC.Application.Feedbacks.Common;
using FluentResults;
using MediatR;

namespace ABC.Application.Feedbacks.Commands.SubmitFeedback;

public sealed record SubmitFeedbackCommand(
    Guid ProductId, 
    string Text,
    string CustomerName) 
    : IRequest<Result<FeedbackDto>>;