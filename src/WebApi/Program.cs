using Microsoft.EntityFrameworkCore;
using SwiftScale.Modules.Catalog.Infrastructure;
using SwiftScale.Modules.Identity.Infrastructure;
using SwiftScale.Modules.Identity.Presentation;
using SwiftScale.Modules.Ordering.Infrastructure;
using SwiftScale.Modules.Payment.Infrastructure;
using SwiftScale.WebApi.Infrastructure;

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

builder.Services.RegisterModules(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();
app.UseExceptionHandler();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        // This ensures Swagger UI knows where to look if things get messy
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    });
}

app.UseHttpsRedirection();
//app.MapUserEndpoints();
app.MapModuleEndpoints();
app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
