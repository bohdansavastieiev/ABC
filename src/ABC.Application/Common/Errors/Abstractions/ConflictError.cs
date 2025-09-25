namespace ABC.Application.Common.Errors.Abstractions;

public abstract class ConflictError(string code, string message) 
    : ApplicationError(code, message)
{
}