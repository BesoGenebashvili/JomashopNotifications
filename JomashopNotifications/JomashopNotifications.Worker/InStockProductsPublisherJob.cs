using Quartz;
using MediatR;
using Serilog;
using JomashopNotifications.Persistence.Abstractions;
using JomashopNotifications.Application.InStockProduct.Queries;
using MassTransit;
using JomashopNotifications.Application.Product.Queries;
using JomashopNotifications.Application.Messages;

namespace JomashopNotifications.Worker;

[DisallowConcurrentExecution]
public sealed class InStockProductsPublisherJob(
    IMediator mediator,
    IBus bus,
    IApplicationErrorsDatabase applicationErrorsDatabase) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        var inStockProductDtos = await mediator.Send(new ListInStockProductsQuery());

        if (inStockProductDtos.Count == 0)
        {
            Log.Information("No in stock products were found in the database");
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
            Log.Information("No active in stock products were found in the database");
            return;
        }

        Log.Information(
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

        Log.Information(
            "Publishing {Count} product in stock events for ProductIds:",
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
                Log.Error(ex, "Failed to publish product in stock event for ProductId: {ProductId}", @event.ProductId);
                await applicationErrorsDatabase.LogAsync(ex);
            }
        }

        // Check for price if > than threshold -> send notification - I need separate configuration table for this
        // I need separate notification handlers like EmailNotificationHandler, SmsNotificationHandler, DesktipMessageNotificationHandler
    }
}