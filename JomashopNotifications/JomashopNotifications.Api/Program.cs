using JomashopNotifications.Api.Middleware;
using JomashopNotifications.Application;
using JomashopNotifications.Domain;
using JomashopNotifications.Persistence;
using Microsoft.AspNetCore.HttpLogging;
using Serilog;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDomainServices();
builder.Services.AddApplicationServices();
builder.Services.AddPersistenceServices(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Host.UseSerilog(
    (context, config) => config.ReadFrom.Configuration(context.Configuration));

builder.Services.AddHttpLogging(options =>
{
    options.LoggingFields = HttpLoggingFields.RequestPath
                          | HttpLoggingFields.RequestHeaders
                          | HttpLoggingFields.RequestBody
                          | HttpLoggingFields.ResponseHeaders
                          | HttpLoggingFields.ResponseBody;

    options.RequestBodyLogLimit = 4096;
    options.ResponseBodyLogLimit = 4096;

    options.CombineLogs = true;
});

// Enum values as strings in the response/request
builder.Services.AddControllers()
                .AddJsonOptions(options =>
                    options.JsonSerializerOptions
                           .Converters
                           .Add(new JsonStringEnumConverter()));

var app = builder.Build();

app.MapGet("/", () => Results.Redirect("/swagger"))
   .ExcludeFromDescription()
   .WithHttpLogging(HttpLoggingFields.None);

app.MapGet("/swagger/v1/swagger.json", () => Results.Ok())
   .ExcludeFromDescription()
   .WithHttpLogging(HttpLoggingFields.None);

app.MapGet("/swagger/index.js", () => Results.Ok())
   .ExcludeFromDescription()
   .WithHttpLogging(HttpLoggingFields.None);

app.MapGet("/swagger/index.html", () => Results.Ok())
   .ExcludeFromDescription()
   .WithHttpLogging(HttpLoggingFields.None);

app.UseHttpLogging();
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

try
{
    Log.Information("Starting application");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}