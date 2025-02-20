using Quartz;
using MediatR;
using MassTransit;
using JomashopNotifications.Application.Messages;
using JomashopNotifications.Application.Product.Queries;
using JomashopNotifications.Application.InStockProduct.Queries;
using JomashopNotifications.Persistence.Abstractions;
using Microsoft.Extensions.Logging;

namespace JomashopNotifications.Worker;

[DisallowConcurrentExecution]
public sealed class InStockProductsPublisherJob(
    IMediator mediator,
    IBus bus,
    IApplicationErrorsDatabase applicationErrorsDatabase,
    ILogger<InStockProductsPublisherJob> logger) : IJob
{
    public static readonly JobKey key =
        new(nameof(JomashopDataSyncJob), "ProductsPublisher");

    public async Task Execute(IJobExecutionContext context)
    {
        var inStockProductDtos = await mediator.Send(new ListInStockProductsQuery());

        if (inStockProductDtos.Count == 0)
        {
            logger.LogInformation("No in stock products were found in the database");
            return;
        }

        // implement filtering with Ids
        var productDtos = await mediator.Send(new ListProductsQuery()
        {
            Status = Persistence.Entities.ProductStatus.Active
        });

        var activeInStockProductDtos = inStockProductDtos.Where(ip => productDtos.Any(p => p.Id == ip.ProductId));

        if (!activeInStockProductDtos.Any())
        {
            logger.LogInformation("No active in stock products were found in the database");
            return;
        }

        logger.LogInformation(
            "Found {Count} active in stock products in the database. ProductIds: {ProductIds}",
            inStockProductDtos.Count,
            inStockProductDtos.Select(x => x.ProductId));

        var productInStockEvents = productDtos.Join(
            inStockProductDtos,
            p => p.Id,
            ip => ip.ProductId,
            (p, ip) => new ProductInStockEvent
            {
                InStockProductId = ip.Id,
                ProductId = p.Id,
                Link = p.Link,
                Price = ip.Price,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt,
                CheckedAt = ip.CheckedAt
            });

        logger.LogInformation(
            "Publishing {Count} product in stock events for ProductIds: {ProductIds}",
            productInStockEvents.Count(),
            productInStockEvents.Select(e => e.ProductId));

        foreach (var @event in productInStockEvents)
        {
            try
            {
                await bus.Publish(@event);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to publish product in stock event for ProductId: {ProductId}", @event.ProductId);
                await applicationErrorsDatabase.LogAsync(ex);
            }
        }

        // Check for price if > than threshold -> send notification - I need separate configuration table for this
        // I need separate notification handlers like EmailNotificationHandler, SmsNotificationHandler, DesktipMessageNotificationHandler
    }
}