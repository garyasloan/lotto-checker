using API.Data;
using API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.OData.ModelBuilder;
using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

// Add controllers and OData services
builder.Services.AddControllers()
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

// Add XML support for $metadata (required by Tableau)
// Add XML support for $metadata (required by Tableau)
builder.Services.Configure<MvcOptions>(options =>
{
    const string xmlMediaType = "application/xml";

    foreach (var formatter in options.OutputFormatters.OfType<ODataOutputFormatter>())
    {
        if (!formatter.SupportedMediaTypes.Contains(xmlMediaType))
            formatter.SupportedMediaTypes.Add(xmlMediaType);
    }

    foreach (var formatter in options.InputFormatters.OfType<ODataInputFormatter>())
    {
        if (!formatter.SupportedMediaTypes.Contains(xmlMediaType))
            formatter.SupportedMediaTypes.Add(xmlMediaType);
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
