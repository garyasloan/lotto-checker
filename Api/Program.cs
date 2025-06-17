using API.Data;
using API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddOData(opt =>
    {
        var modelBuilder = new ODataConventionModelBuilder
        {
            Namespace = "Lotto",
            ContainerName = "LottoContainer"
        };
        modelBuilder.EntitySet<NumberOccurrenceDTO>("NumberOccurrences");

        opt
            .AddRouteComponents("odata", modelBuilder.GetEdmModel())
            .Select()
            .Filter()
            .OrderBy()
            .Expand()
            .Count()
            .SetMaxTop(100);
    });

// Add support for XML in metadata responses
builder.Services.Configure<MvcOptions>(options =>
{
    foreach (var outputFormatter in options.OutputFormatters.OfType<ODataOutputFormatter>())
    {
        outputFormatter.SupportedMediaTypes.Add("application/xml");
    }

    foreach (var inputFormatter in options.InputFormatters.OfType<ODataInputFormatter>())
    {
        inputFormatter.SupportedMediaTypes.Add("application/xml");
    }
});

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

app.Use(async (context, next) =>
{
    // Emulate HEAD by short-circuiting GET without response body
    if (context.Request.Method == HttpMethods.Head)
    {
        context.Request.Method = HttpMethods.Get;

        var originalBody = context.Response.Body;
        await using var memStream = new MemoryStream();
        context.Response.Body = memStream;

        await next();

        context.Response.Body = originalBody;
        context.Response.ContentLength = memStream.Length;
        context.Response.Headers["OData-Version"] = "4.0";
        context.Response.Headers["Allow"] = "GET,HEAD";
        return;
    }

    await next();
});

app.UseAuthorization();
app.MapControllers();
app.MapWhen(context =>
    !context.Request.Path.StartsWithSegments("/api") &&
    !context.Request.Path.StartsWithSegments("/odata"),
    builder => builder.Run(async context =>
{
    context.Response.ContentType = "text/html";
    await context.Response.SendFileAsync(Path.Combine(app.Environment.WebRootPath, "index.html"));
}));

app.Run();

static IEdmModel GetEdmModel()
{
    var builder = new ODataConventionModelBuilder();
    builder.EntitySet<NumberOccurrenceDTO>("NumberOccurrences");
    return builder.GetEdmModel();
}
