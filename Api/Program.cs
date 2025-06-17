using API.Data;
using API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.OData.ModelBuilder;
using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// OData + XML support
builder.Services.AddControllers().AddOData(opt =>
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

// Add XML support for OData formatters
builder.Services.Configure<MvcOptions>(options =>
{
    var odataXmlMediaTypes = new[]
    {
        "application/xml",
        "application/xml;odata.metadata=minimal",
        "application/xml;odata.metadata=full",
        "application/atom+xml",
        "application/atomsvc+xml"
    };

    foreach (var formatter in options.OutputFormatters.OfType<ODataOutputFormatter>())
    {
        foreach (var mediaType in odataXmlMediaTypes)
        {
            if (!formatter.SupportedMediaTypes.Contains(mediaType))
                formatter.SupportedMediaTypes.Add(mediaType);
        }
    }

    foreach (var formatter in options.InputFormatters.OfType<ODataInputFormatter>())
    {
        foreach (var mediaType in odataXmlMediaTypes)
        {
            if (!formatter.SupportedMediaTypes.Contains(mediaType))
                formatter.SupportedMediaTypes.Add(mediaType);
        }
    }
});

builder.Services.AddOpenApi();

var connStr = builder.Configuration.GetConnectionString("Default");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connStr, sql =>
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

var app = builder.Build();

// Add global error logging
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
        await context.Response.WriteAsync("UNHANDLED EXCEPTION:\n" + ex.ToString());
    }
});

// Force correct Accept for $metadata to avoid XML formatter mismatch
app.Use(async (context, next) =>
{
    if (context.Request.Path.Value?.EndsWith("$metadata") == true)
    {
        context.Request.Headers["Accept"] = "application/xml";
        context.Response.ContentType = "application/xml";
    }

    await next();
});

using var scope = app.Services.CreateScope();
var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
dbContext.Database.Migrate();

app.UseCors("AllowClient");
app.UseStaticFiles();

// HEAD emulation
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

// SPA fallback
app.MapWhen(
    context => !context.Request.Path.StartsWithSegments("/api") &&
               !context.Request.Path.StartsWithSegments("/odata"),
    builder => builder.Run(async context =>
    {
        context.Response.ContentType = "text/html";
        await context.Response.SendFileAsync(Path.Combine(app.Environment.WebRootPath, "index.html"));
    }));

app.Run();
