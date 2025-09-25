using ABC.Application.Feedbacks.Common;
using FluentResults;
using MediatR;

namespace ABC.Application.Feedbacks.Queries.GetFeedbacksByProductId;

public record GetFeedbacksByProductIdQuery(Guid ProductId) : IRequest<Result<FeedbacksDto>>;