using JomashopNotifications.Api;
using JomashopNotifications.Api.Middleware;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiServices(builder.Configuration);

builder.Host.UseSerilog(
    (context, config) => config.ReadFrom.Configuration(context.Configuration));

var app = builder.Build();

app.MapSwaggerRoutes();
app.UseHttpLogging();
app.UseMiddleware<ExceptionHandlingMiddleware>();

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