using Microsoft.EntityFrameworkCore;
using Serilog;
using TA_API.Services.Data;

var builder = WebApplication.CreateBuilder(args);
{
    builder.Host.UseSerilog((ctx, lc) => lc.WriteTo.Console());
    builder.Services.AddDbContext<AssessmentDbContext>(options => options.UseSqlite(builder.Configuration.GetConnectionString("AssessmentDB")));

    // Comment out AddControllers/MapControllers if you prefer to implement Minimal APIs.
    builder.Services.AddControllers();
}
var app = builder.Build();
{
    app.UseSerilogRequestLogging();

    app.MapGet("/", () => "Technical Assessment API");
    app.MapGet("/lbhealth", () => "Technical Assessment API");

    app.MapControllers();
}
app.Run();
