namespace ABC.Application.Common.Exceptions;

public class ResultContractViolationException(
    ContractViolationType violationType,
    string message)
    : InvalidOperationException(message)
{
    public ContractViolationType ViolationType { get; } = violationType;
}

public enum ContractViolationType
{
    HandleFailureOnSuccess,
    NoErrorsInFailedResult,
    NonApplicationError,
    UnexpectedErrorType
}