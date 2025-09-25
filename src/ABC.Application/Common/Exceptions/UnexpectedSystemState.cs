namespace ABC.Application.Common.Exceptions;

public class UnexpectedSystemState(string message) : Exception(message)
{
}