using ABC.Application.Common.Constants;
using ABC.Application.Common.Errors.Abstractions;

namespace ABC.Application.Common.Errors.Common;

public class NotFoundError(
    string resourceType,
    string propertyName,
    Guid resourceId,
    string code = ErrorCodes.ResourceNotFound) 
    : ApplicationError(
        code, 
        $"{resourceType} with the provided {propertyName} was not found")
{
    public string ResourceType { get; } = resourceType;
    public Guid ResourceId { get; } = resourceId;
}