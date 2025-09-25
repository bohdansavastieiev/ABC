using FluentResults;
using MediatR;

namespace ABC.Application.ProductRatings.Commands.ProcessNewFeedback;

public record ProcessNewFeedbackCommand(Guid FeedbackId) : IRequest<Result>;