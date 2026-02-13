using Microsoft.EntityFrameworkCore;
using SwiftScale.Modules.Catalog.Infrastructure;
using SwiftScale.Modules.Identity.Infrastructure;
using SwiftScale.Modules.Ordering.Infrastructure;
using SwiftScale.Modules.Payment.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();


var connectionString = builder.Configuration.GetConnectionString("Database");

// Register Identity (Created yesterday)
builder.Services.AddDbContext<IdentityDbContext>(opt => opt.UseNpgsql(connectionString));

// Register Catalog
builder.Services.AddDbContext<CatalogDbContext>(opt => opt.UseNpgsql(connectionString));

// Register Ordering
builder.Services.AddDbContext<OrderingDbContext>(opt => opt.UseNpgsql(connectionString));

// Register Payment
builder.Services.AddDbContext<PaymentDbContext>(opt => opt.UseNpgsql(connectionString));

builder.Services.AddCatalogInfrastructure(builder.Configuration);
builder.Services.AddIdentityInfrastructure(builder.Configuration);
builder.Services.AddOrderingInfrastructure(builder.Configuration);
builder.Services.AddPaymentInfrastructure(builder.Configuration);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseSwaggerUI(options =>
    {
        // This ensures Swagger UI knows where to look if things get messy
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    });
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
