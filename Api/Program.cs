using API.Data;
using API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;

var builder = WebApplication.CreateBuilder(args);

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

var modelBuilder = new ODataConventionModelBuilder
{
    Namespace = "Lotto",
    ContainerName = "LottoContainer"
};
modelBuilder.EntitySet<NumberOccurrenceDTO>("NumberOccurrences");
IEdmModel edmModel = modelBuilder.GetEdmModel();

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
    opt.AddRouteComponents("odata", edmModel)
        .Select()
        .Filter()
        .OrderBy()
        .Expand()
        .Count()
        .SetMaxTop(100);
});

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

app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (Exception ex)
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "text/plain";
        await context.Response.WriteAsync("UNHANDLED EXCEPTION:\n" + ex);
    }
});

app.UseCors("AllowClient");
app.UseStaticFiles();
app.UseAuthorization();
app.MapControllers();

// ✅ Serve EDMX metadata explicitly for Tableau at /edmx
app.MapGet("/edmx", async context =>
{
    context.Response.StatusCode = 200;
    context.Response.ContentType = "application/xml";

    var settings = new System.Xml.XmlWriterSettings
    {
        Async = true,
        Indent = true
    };

    using var memoryStream = new MemoryStream();
    using (var xmlWriter = System.Xml.XmlWriter.Create(memoryStream, settings))
    {
        if (!Microsoft.OData.Edm.Csdl.CsdlWriter.TryWriteCsdl(edmModel, xmlWriter, Microsoft.OData.Edm.Csdl.CsdlTarget.OData, out var errors))
        {
            context.Response.StatusCode = 500;
            await context.Response.WriteAsync("Failed to generate metadata XML.");
            return;
        }

        await xmlWriter.FlushAsync();
    }

    memoryStream.Position = 0;
    await memoryStream.CopyToAsync(context.Response.Body);
});

// ✅ Properly override /odata/$metadata to return EDMX XML (for Tableau)
app.MapGet("/odata/$metadata", async context =>
{
    context.Response.StatusCode = 200;
    context.Response.ContentType = "application/xml";

    var settings = new System.Xml.XmlWriterSettings
    {
        Async = true,
        Indent = true
    };

    using var memoryStream = new MemoryStream();
    using (var xmlWriter = System.Xml.XmlWriter.Create(memoryStream, settings))
    {
        if (!Microsoft.OData.Edm.Csdl.CsdlWriter.TryWriteCsdl(edmModel, xmlWriter, Microsoft.OData.Edm.Csdl.CsdlTarget.OData, out var errors))
        {
            context.Response.StatusCode = 500;
            await context.Response.WriteAsync("Failed to generate metadata XML.");
            return;
        }

        await xmlWriter.FlushAsync();
    }

    memoryStream.Position = 0;
    await memoryStream.CopyToAsync(context.Response.Body);
});

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.Run();
