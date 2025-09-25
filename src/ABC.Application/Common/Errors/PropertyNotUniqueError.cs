using ABC.Application.Common.Constants;
using ABC.Application.Common.Errors.Abstractions;

namespace ABC.Application.Common.Errors;

public class PropertyNotUniqueError(string resourceType, string propertyName, string propertyValue) 
    : ConflictError(
        ErrorCodes.PropertyNotUnique,
        $"Property {propertyName} of the {resourceType} must be unique within the system." 
        + $" Property with value {propertyValue} already exists.")
{
    public string ResourceType { get; } = resourceType;
    public string PropertyName { get; } = propertyName;
    public string PropertyValue { get; } = propertyValue;
}