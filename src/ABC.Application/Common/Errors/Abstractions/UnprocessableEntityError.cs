namespace ABC.Application.Common.Errors.Abstractions;

public abstract class UnprocessableEntityError(string code, string message) 
    : ApplicationError(code, message)
{
}