namespace ABC.Application.Common.Constants;

public static class ErrorCodes
{
    public const string ValidationFailed = "VALIDATION_FAILED";
    public const string InvalidRequest = "INVALID_REQUEST";
    public const string Unauthorized = "UNAUTHORIZED";
    public const string ResourceNotFound = "RESOURCE_NOT_FOUND";
    public const string InternalServerError = "INTERNAL_SERVER_ERROR";
    
    public const string ReferencedResourceNotFound = "REFERENCED_RESOURCE_NOT_FOUND";
    public const string PropertyNotUnique = "PPOPERTY_NOT_UNIQUE";
    
    public const string NonEmptyCollectionExpected = "NON_EMPTY_COLLECTION_EXPECTED";
}