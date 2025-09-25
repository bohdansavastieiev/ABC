using ABC.Application.Feedbacks.Common;
using FluentResults;
using MediatR;

namespace ABC.Application.Feedbacks.Queries.GetFeedbackById;

public record GetFeedbackByIdQuery(Guid Id) : IRequest<Result<FeedbackDto>>;