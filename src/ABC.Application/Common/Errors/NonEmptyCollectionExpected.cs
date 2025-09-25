using ABC.Application.Common.Constants;
using ABC.Application.Common.Errors.Abstractions;

namespace ABC.Application.Common.Errors;

public class NonEmptyCollectionExpected(string collectionName)  
    : ApplicationError(ErrorCodes.NonEmptyCollectionExpected,
        $"Non-empty collection expected. Collection name: {collectionName}")
{
}