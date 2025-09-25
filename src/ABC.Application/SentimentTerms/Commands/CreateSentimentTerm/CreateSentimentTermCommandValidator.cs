using ABC.Application.Common.Constants;
using ABC.Domain.Common;
using FluentValidation;

namespace ABC.Application.SentimentTerms.Commands.CreateSentimentTerm;

public class CreateSentimentTermCommandValidator : AbstractValidator<CreateSentimentTermCommand>
{
    public CreateSentimentTermCommandValidator()
    {
        RuleFor(x => x.Term)
            .NotEmpty()
            .WithMessage("Term is required")
            .MaximumLength(ValidationConstants.MaxTermLength)
            .WithMessage($"Term must not exceed {ValidationConstants.MaxTermLength} characters");
        
        RuleFor(x => x.Score)
            .InclusiveBetween(DomainConstants.MinScore, DomainConstants.MaxScore)
            .WithMessage($"Score must be between {DomainConstants.MinScore} and {DomainConstants.MaxScore}.");
    }
}