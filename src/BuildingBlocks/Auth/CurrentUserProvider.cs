using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace SwiftScale.BuildingBlocks.Auth
{
    public sealed class CurrentUserProvider(IHttpContextAccessor httpContextAccessor) : ICurrentUserProvider
    {
        public Guid UserId => Guid.TryParse(httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier),out var id) ? id : Guid.Empty;

        public string Email => httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Email) ?? string.Empty;

        public bool IsAuthenticated => httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;
    }
}
