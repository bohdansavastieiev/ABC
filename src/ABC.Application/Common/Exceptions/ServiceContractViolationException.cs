namespace ABC.Application.Common.Exceptions;

public class ServiceContractViolationException(
    string serviceName,
    string reason)
    : Exception($"A contract violation occurred with the service: '{serviceName}'. Reason: {reason}");