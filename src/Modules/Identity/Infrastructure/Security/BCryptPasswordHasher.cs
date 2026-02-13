using SwiftScale.Modules.Identity.Application.Interfaces;

namespace SwiftScale.Modules.Identity.Infrastructure.Security
{
    internal sealed class BCryptPasswordHasher : IPasswordHasher
    {
        // A work factor of 12 is a professional balance between security and performance in 2026
        private const int WorkFactor = 12;

        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.EnhancedHashPassword(password, WorkFactor);
        }

        public bool VerifyPassword(string password, string hash)
        {
            return BCrypt.Net.BCrypt.EnhancedVerify(password, hash);
        }
    }
}
