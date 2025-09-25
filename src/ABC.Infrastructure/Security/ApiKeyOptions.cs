namespace ABC.Infrastructure.Security;

public class ApiKeyOptions
{
    public const string SectionName = "ApiKeys";
    
    public string AdminKey { get; set; } = string.Empty;
}