using Microsoft.EntityFrameworkCore;
using Serilog;
using TA_API.Middleware;
using TA_API.Services.Authentication;
using TA_API.Services.Characters;
using TA_API.Services.Data;
using TA_API.Services.RickAndMorty;
using TA_API.Services.Validation;

var builder = WebApplication.CreateBuilder(args);
{
    builder.Host.UseSerilog((ctx, lc) => lc.WriteTo.Console());
    builder.Services.AddDbContext<AssessmentDbContext>(options => options.UseSqlite(builder.Configuration.GetConnectionString("AssessmentDB")));

    // Add CORS
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAngularFrontend", policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
    });

    // Register Rick and Morty API Service
    builder.Services.AddHttpClient<IRickAndMortyService, RickAndMortyService>();

    // Register Character Service for CRUD operations
    builder.Services.AddScoped<ICharacterService, CharacterService>();

    // Register Authentication Service
    builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();

    // Register Validation Service
    builder.Services.AddScoped<IValidationService, CharacterValidationService>();

    // Comment out AddControllers/MapControllers if you prefer to implement Minimal APIs.
    builder.Services.AddControllers();
}
var app = builder.Build();
{
    app.UseSerilogRequestLogging();

    // Enable CORS
    app.UseCors("AllowAngularFrontend");

    // Add API Key Authentication Middleware
    app.UseMiddleware<ApiKeyAuthenticationMiddleware>();

    app.MapGet("/", () => "Technical Assessment API");
    app.MapGet("/lbhealth", () => "Technical Assessment API");

    app.MapControllers();
}
app.Run();
