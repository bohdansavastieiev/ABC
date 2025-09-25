using FluentResults;

namespace ABC.Application.Common.Errors.Abstractions;

public abstract class ApplicationError(
    string code,
    string message) 
    : Error(message)
{
    public string Code { get; } = code;
}