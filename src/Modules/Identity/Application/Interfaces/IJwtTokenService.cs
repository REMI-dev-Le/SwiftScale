using SwiftScale.Modules.Identity.Domain;

namespace SwiftScale.Modules.Identity.Application.Interfaces
{
    public interface IJwtTokenService
    {
        string GenerateToken(User user);
    }
}
