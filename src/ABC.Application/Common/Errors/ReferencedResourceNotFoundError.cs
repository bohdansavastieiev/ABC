using ABC.Application.Common.Constants;
using ABC.Application.Common.Errors.Abstractions;
using ABC.Application.Common.Errors.Common;

namespace ABC.Application.Common.Errors;

public class ReferencedResourceNotFoundError(
    string resourceType,
    string propertyName,
    Guid resourceId) 
    : NotFoundError(
        resourceType,
        propertyName,
        resourceId,
        ErrorCodes.ReferencedResourceNotFound)
{
}