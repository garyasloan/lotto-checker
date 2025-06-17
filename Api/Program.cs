using API.Data;
using API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using Microsoft.OData.ModelBuilder;

var builder = WebApplication.CreateBuilder(args);

// XML media types to support for OData
string[] xmlMediaTypes =
{
    "application/xml",
    "application/xml;odata.metadata=minimal",
    "application/xml;odata.metadata=minimal; charset=utf-8",
    "application/xml;odata.metadata=full",
    "application/xml;odata.metadata=full; charset=utf-8",
    "application/xml;odata.metadata=none",
    "application/xml;odata.metadata=none; charset=utf-8"
};

// Add OData and formatters
builder.Services.AddControllers(options =>
{
    foreach (var outputFormatter in options.OutputFormatters.OfType<ODataOutputFormatter>())
    {
        foreach (var mediaType in xmlMediaTypes)
        {
            if (!outputFormatter.SupportedMediaTypes.Contains(mediaType))
            {
                outputFormatter.SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse(mediaType));
            }
        }
    }

    foreach (var inputFormatter in options.InputFormatters.OfType<ODataInputFormatter>())
    {
        foreach (var mediaType in xmlMediaTypes)
        {
            if (!inputFormatter.SupportedMediaTypes.Contains(mediaType))
            {
                inputFormatter.SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse(mediaType));
            }
        }
    }
})
.AddOData(opt =>
{
    var modelBuilder = new ODataConventionModelBuilder
    {
        Namespace = "Lotto",
        ContainerName = "LottoContainer"
    };
    modelBuilder.EntitySet<NumberOccurrenceDTO>("NumberOccurrences");

    opt.AddRouteComponents("odata", modelBuilder.GetEdmModel())
        .Select()
        .Filter()
        .OrderBy()
        .Expand()
        .Count()
        .SetMaxTop(100);
});

// Other services
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default"), sql =>
    {
        sql.CommandTimeout(60);
        sql.EnableRetryOnFailure();
    }));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowClient", policy =>
    {
        policy.WithOrigins(
                "https://www.lotto-checker.com",
                "https://lotto-checker-app.wittyglacier-91c7b4e8.westus2.azurecontainerapps.io",
                "https://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddOpenApi();

var app = builder.Build();

// Global exception handler
app.Use(async (context, next) =>
{
    try
    {
        await next.Invoke();
    }
    catch (Exception ex)
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "text/plain";
        await context.Response.WriteAsync("UNHANDLED EXCEPTION:\n" + ex);
    }
});

// HEAD support for Tableau
app.Use(async (context, next) =>
{
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

app.UseCors("AllowClient");
app.UseStaticFiles();
app.UseAuthorization();
app.MapControllers();

// Fallback for SPA
app.MapWhen(
    context => !context.Request.Path.StartsWithSegments("/api") &&
               !context.Request.Path.StartsWithSegments("/odata"),
    builder => builder.Run(async context =>
    {
        context.Response.ContentType = "text/html";
        await context.Response.SendFileAsync(Path.Combine(app.Environment.WebRootPath, "index.html"));
    }));

// Apply migrations
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.Run();
