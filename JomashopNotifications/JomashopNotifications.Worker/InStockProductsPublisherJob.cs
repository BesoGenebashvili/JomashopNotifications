using Quartz;
using MediatR;
using Serilog;
using JomashopNotifications.Persistence.Abstractions;
using JomashopNotifications.Application.InStockProduct.Queries;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace JomashopNotifications.Worker;

[DisallowConcurrentExecution]
public sealed class InStockProductsPublisherJob(
    IMediator mediator,
    IApplicationErrorsDatabase applicationErrorsDatabase) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        // Get all instock products
        var inStockProductDtos = await mediator.Send(new ListInStockProductsQuery());

        if (inStockProductDtos.Count == 0)
        {
            Log.Information("No in stock products were found in the database");
            return;
        }

        Log.Information(
            "Found {Count} in stock products in the database. ProductIds: {ProductIds}",
            inStockProductDtos.Count,
            inStockProductDtos.Select(x => x.ProductId));

        var factory = new ConnectionFactory() { HostName = "localhost" };
        using var connection = await factory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();
        var exchange = "in-stock-products-exchange";

        await channel.ExchangeDeclareAsync(
                        exchange: exchange,
                        type: ExchangeType.Fanout,
                        durable: true,
                        autoDelete: false);

        foreach (var (id, _, _, _) in inStockProductDtos)
        {
            var message = JsonSerializer.Serialize(
                new
                {
                    id
                });

            var body = Encoding.UTF8.GetBytes(message);

            await channel.BasicPublishAsync(exchange: exchange,
                                            routingKey: string.Empty,
                                            body: body);
        }

        // Check for price if > than threshold -> send notification - I need separate configuration table for this
        // I need separate notification handlers like EmailNotificationHandler, SmsNotificationHandler, DesktipMessageNotificationHandler
    }
}