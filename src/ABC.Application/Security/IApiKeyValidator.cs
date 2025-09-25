namespace ABC.Application.Security;

public interface IApiKeyValidator
{
    bool IsValidAdminKey(string? apiKey);
}