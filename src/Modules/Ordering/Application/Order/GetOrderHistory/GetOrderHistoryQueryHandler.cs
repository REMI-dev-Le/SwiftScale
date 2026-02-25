using Dapper;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;
using SwiftScale.BuildingBlocks;
using SwiftScale.BuildingBlocks.Auth;
using SwiftScale.Modules.Catalog.Application.Order.CreateOrder;
using SwiftScale.Modules.Ordering.Domain;
using System.Linq;

namespace SwiftScale.Modules.Ordering.Application.Order.GetOrderHistory
{
    internal sealed class GetOrderHistoryQueryHandler(IConfiguration configuration,ICurrentUserProvider currentUser, ILogger<GetOrderHistoryQuery> logger) : IRequestHandler<GetOrderHistoryQuery, Result<IReadOnlyList<OrderHistoryResponse>>>
    {
        public async Task<Result<IReadOnlyList<OrderHistoryResponse>>> Handle(GetOrderHistoryQuery request, CancellationToken ct)
        {
            // CA1873: Check if logging is enabled before evaluating expensive argument
            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Attempting to retrieve order history for User: {UserId}", currentUser.UserId);
            }
            // 1. Check authentication
            if (!currentUser.IsAuthenticated)
            {
                return Result<IReadOnlyList<OrderHistoryResponse>>.Failure(new Error("You must be logged in to view history."));
            }

            // 1. Establish a connection to PostgreSQL
            using var connection = new NpgsqlConnection(configuration.GetConnectionString("Database"));

            // 2. Write the optimized SQL Join
            const string sql = @"SELECT 
                o.""Id"" AS OrderId, 
                o.""CreatedAt"", 
                o.""Status"", 
                SUM(oi.""UnitPrice"" * oi.""Quantity"") AS TotalAmount,
                COUNT(oi.""Id"") AS ItemCount
            FROM ordering.""Orders"" o
            LEFT JOIN ordering.""OrderItems"" oi ON o.""Id"" = oi.""OrderId""
            WHERE o.""UserId"" = @CustomerId
            GROUP BY o.""Id"", o.""CreatedAt"", o.""Status""
            ORDER BY o.""CreatedAt"" DESC";

            // 3. Execute with Dapper
            var orders = await connection.QueryAsync<OrderHistoryResponse>(sql, new { CustomerId = currentUser.UserId });

            logger.LogInformation("Order(s) successfully retrieved for User {UserId}. OrderIds: {OrderIds}", currentUser.UserId, string.Join(", ", orders.Select(x => x.OrderId)));

            return Result<IReadOnlyList<OrderHistoryResponse>>.Success(orders.ToList()); 
        }
    }
}
