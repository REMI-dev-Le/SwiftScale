using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Headers;

namespace IntegrationTests.Infrastructure
{
    public abstract class BaseIntegrationTest : IClassFixture<IntegrationTestWebAppFactory>
    {
        protected readonly HttpClient Client;
        protected readonly IServiceScope Scope;

        protected BaseIntegrationTest(IntegrationTestWebAppFactory factory)
        {
            Client = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    // Force the TestScheme to be the default for everything
                    services.AddAuthentication(options =>
                    {
                        options.DefaultAuthenticateScheme = TestAuthHandler.AuthenticationScheme;
                        options.DefaultScheme = TestAuthHandler.AuthenticationScheme;
                        options.DefaultChallengeScheme = TestAuthHandler.AuthenticationScheme;
                    })
                    .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                        TestAuthHandler.AuthenticationScheme, options => { });
                });
            }).CreateClient();

            // 2. The Header MUST match the scheme name
            Client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(TestAuthHandler.AuthenticationScheme);
        }
    }
}
