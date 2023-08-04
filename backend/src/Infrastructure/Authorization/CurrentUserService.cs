using Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Authorization;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        this.httpContextAccessor = httpContextAccessor;
    }

    public string UserId => this.httpContextAccessor.HttpContext?.User.Identity?.Name ?? string.Empty;
}
