using FluentAssertions;
using SwiftScale.Modules.Ordering.Application.Order.GetOrderHistory;
using System.Net;
using System.Net.Http.Json;

namespace IntegrationTests.Orders
{
    public class GetOrderHistoryTests : IClassFixture<IntegrationTestWebAppFactory>
    {
        private readonly HttpClient _client;

        public GetOrderHistoryTests(IntegrationTestWebAppFactory factory)
        {
            _client = factory.CreateClient(); // Simulates real HTTP calls
        }

        [Fact]
        public async Task GetHistory_ShouldReturnOk_WhenUserIsAuthenticated()
        {
            // Act - This will now succeed with a 200 OK because of TestAuthHandler
            //var response = await _client.GetAsync("/ordering/history");

            var response = await _client.GetAsync("/ordering/history");
            if (!response.IsSuccessStatusCode)
            {
                var errorDetails = await response.Content.ReadAsStringAsync();
                // Set a breakpoint here and inspect 'errorDetails'
                throw new Exception(errorDetails);
            }
            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }

        [Fact]
        public async Task GetHistory_ShouldReturnOrders_WhenOrdersExistInPostgres()
        {
            // 1. Arrange: Login and get a real JWT
            // (In Day 5, we use a 'TestAuthHandler' to skip real identity for speed)

            // 2. Act: Call the real Dapper endpoint
            var response = await _client.GetAsync("/ordering/history");

            // 3. Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadFromJsonAsync<List<OrderHistoryResponse>>();
            content.Should().NotBeNull();
        }
    }
}
