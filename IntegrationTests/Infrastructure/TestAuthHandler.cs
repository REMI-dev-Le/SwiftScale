using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace IntegrationTests.Infrastructure
{
    public class TestAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
                                 ILoggerFactory logger,
                                 UrlEncoder encoder) : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
    {
        public const string AuthenticationScheme = "TestScheme";

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var claims = new[] {
        new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
        new Claim(ClaimTypes.Name, "Test User"),
        new Claim(ClaimTypes.Role, "Admin") // Ensure this matches your endpoint requirements
    };

            var identity = new ClaimsIdentity(claims, AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, AuthenticationScheme);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}