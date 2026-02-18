using Dapper;
using MediatR;
using Microsoft.Extensions.Configuration;
using Npgsql;
using SwiftScale.BuildingBlocks;

namespace SwiftScale.Modules.Catalog.Application.Products.GetProduct
{
    internal sealed class GetProductQueryHandler(IConfiguration configuration)
    : IRequestHandler<GetProductQuery, Result<ProductResponse>>
    {
        public async Task<Result<ProductResponse>> Handle(GetProductQuery request, CancellationToken ct)
        {
            // Get the string directly from appsettings.json via IConfiguration
            using var connection = new NpgsqlConnection(configuration.GetConnectionString("Database"));

            // Use raw SQL for maximum performance (The 'R' in CQRS)
            const string sql = @"
            SELECT 
                ""Id"", 
                ""Name"", 
                ""Description"", 
                ""PriceAmount"" AS Price, 
                ""Sku"" 
            FROM catalog.""Products"" 
            WHERE ""Id"" = @Id";

            var product = await connection.QueryFirstOrDefaultAsync<ProductResponse>(sql, new { request.Id });

            return product is not null
                ? Result<ProductResponse>.Success(product)
                : Result<ProductResponse>.Failure(new Error($"Product with ID {request.Id} was not found."));
        }
    }
}
