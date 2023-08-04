using Microsoft.AspNetCore.Authentication;

namespace Infrastructure.Authorization;

public class ApiKeyAuthenticationOptions : AuthenticationSchemeOptions
{
    public const string DefaultScheme = "ClientKey";
    public const string HeaderName = "x-api-key";
}
