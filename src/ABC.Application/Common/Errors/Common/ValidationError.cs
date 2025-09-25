using ABC.Application.Common.Constants;
using ABC.Application.Common.Errors.Abstractions;
using FluentValidation.Results;

namespace ABC.Application.Common.Errors.Common;

public class ValidationError(
    IEnumerable<ValidationFailure> failures)
    : ApplicationError(
        ErrorCodes.ValidationFailed,
        "One or more validation errors occurred")
{
    private IReadOnlyList<ValidationFailure> Failures { get; } = failures.ToList();

    public Dictionary<string, string[]> GetValidationDictionary()
    {
        return Failures
            .GroupBy(f => f.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(f => f.ErrorMessage).ToArray()
            );
    }
}