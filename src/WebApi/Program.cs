using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Serilog;
using SwiftScale.BuildingBlocks.Auth;
using SwiftScale.Modules.Catalog.Infrastructure;
using SwiftScale.Modules.Identity.Infrastructure;
using SwiftScale.Modules.Ordering.Infrastructure;
using SwiftScale.Modules.Payment.Infrastructure;
using SwiftScale.WebApi.Infrastructure;
using System.Text;


// 1. Setup Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/swiftscale-.txt", rollingInterval: RollingInterval.Day)
    .Enrich.FromLogContext()
    .CreateLogger();
try
{
    Log.Information("Starting SwiftScale Web API...");
    var builder = WebApplication.CreateBuilder(args);

    // 2. Tell ASP.NET to use Serilog
    builder.Host.UseSerilog();

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
    builder.Services.AddHttpContextAccessor(); // Required for CurrentUserProvider
    builder.Services.AddScoped<ICurrentUserProvider, CurrentUserProvider>();
    builder.Services.RegisterModules(builder.Configuration);
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Enter your JWT token"
        });

        // ? New v10 syntax
        options.AddSecurityRequirement((document) => new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("Bearer", document)] = new List<string>()
        });
    });
    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
    builder.Services.AddProblemDetails();

    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidAudience = builder.Configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
            };
        });



    builder.Services.AddAuthorization();

    var app = builder.Build();
    app.UseExceptionHandler();
    app.UseStaticFiles();

    app.UseAuthentication(); // Must come before UseAuthorization
    app.UseAuthorization();
    // Configure the HTTP r
    // equest pipeline.
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
}
catch (Exception)
{

    throw;
}