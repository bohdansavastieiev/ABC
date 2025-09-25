using ABC.Application.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace ABC.Infrastructure.Security;

public class ApiKeyValidator : IApiKeyValidator
{
    private readonly ApiKeyOptions _options;
    
    public ApiKeyValidator(IOptions<ApiKeyOptions> options)
    {
        _options = options.Value;

        if (string.IsNullOrWhiteSpace(_options.AdminKey))
        {
            throw new InvalidOperationException("Admin API key not configured");
        }
    }
    
    public bool IsValidAdminKey(string? apiKey) 
        => !string.IsNullOrEmpty(apiKey) && apiKey == _options.AdminKey;
}