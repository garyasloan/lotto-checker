using API.Data;
using API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using Microsoft.AspNetCore.OData;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddOData(opt =>
        opt.Select().Filter().OrderBy().Expand().Count().SetMaxTop(100)
            .AddRouteComponents("odata", GetEdmModel()));

builder.Services.AddOpenApi();

var connStr = builder.Configuration.GetConnectionString("Default");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connStr, sql =>
    {
        sql.CommandTimeout(60);          // Allow longer queries
        sql.EnableRetryOnFailure();      // Retry on transient failures
    }));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowClient", policy =>
    {
        policy.WithOrigins(
                "https://www.lotto-checker.com",
                "https://lotto-checker-app.wittyglacier-91c7b4e8.westus2.azurecontainerapps.io",
                "https://localhost:5173"
            )
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

using var scope = app.Services.CreateScope();
var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
var pending = dbContext.Database.GetPendingMigrations();
dbContext.Database.Migrate();

app.UseCors("AllowClient");
app.UseStaticFiles();
app.UseAuthorization();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();

static IEdmModel GetEdmModel()
{
    var builder = new ODataConventionModelBuilder();
    builder.EntitySet<NumberOccurrenceDTO>("NumberOccurrences");
    return builder.GetEdmModel();
}
