using API.Data;
using API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.OData.ModelBuilder;
using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

// Configure controllers and OData
builder.Services.AddControllers(options =>
{
    var xmlTypes = new[]
    {
        "application/xml",
        "application/xml;odata.metadata=minimal",
        "application/xml;odata.metadata=minimal; charset=utf-8",
        "application/xml;odata.metadata=full",
        "application/xml;odata.metadata=full; charset=utf-8",
        "application/xml;odata.metadata=none",
        "application/xml;odata.metadata=none; charset=utf-8"
    };

    foreach (var formatter in options.OutputFormatters.OfType<ODataOutputFormatter>())
    {
        foreach (var type in xmlTypes)
        {
            if (!formatter.SupportedMediaTypes.Contains(type))
                formatter.SupportedMediaTypes.Add(type);
        }
    }

    foreach (var formatter in options.InputFormatters.OfType<ODataInputFormatter>())
    {
        foreach (var type in xmlTypes)
        {
            if (!formatter.SupportedMediaTypes.Contains(type))
                formatter.SupportedMediaTypes.Add(type);
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

builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default"), sql =>
    {
        sql.CommandTimeout(60);
        sql.EnableRetryOnFailure();
    });
});

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

var app = builder.Build();

// Global exception handling
app.Use(async (context, next) =>
{
    try { await next(); }
    catch (Exception ex)
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "text/plain";
        await context.Response.WriteAsync("UNHANDLED EXCEPTION:\n" + ex);
    }
});

// Normalize $metadata content type
app.Use(async (context, next) =>
{
    var isMetadata = context.Request.Path.Value?.EndsWith("$metadata") == true;

    if (isMetadata)
    {
        // Set Accept header
        context.Request.Headers["Accept"] = "application/xml;odata.metadata=minimal";
    }

    await next();

    if (isMetadata && context.Response.ContentType?.StartsWith("application/xml") == true)
    {
        context.Response.ContentType = "application/xml;odata.metadata=minimal";
    }
});

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.UseCors("AllowClient");
app.UseStaticFiles();

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

app.UseAuthorization();
app.MapControllers();

// SPA fallback for frontend
app.MapWhen(
    context => !context.Request.Path.StartsWithSegments("/api") &&
               !context.Request.Path.StartsWithSegments("/odata"),
    builder => builder.Run(async context =>
    {
        context.Response.ContentType = "text/html";
        await context.Response.SendFileAsync(Path.Combine(app.Environment.WebRootPath, "index.html"));
    }));

app.Run();
