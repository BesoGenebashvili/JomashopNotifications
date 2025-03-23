using JomashopNotifications.Api;
using JomashopNotifications.Api.Middleware;
using Microsoft.AspNetCore.HttpLogging;
using Serilog;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddApiServices(builder.Configuration);
builder.Services.AddControllers();

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

app.MapSwaggerRoutes();
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