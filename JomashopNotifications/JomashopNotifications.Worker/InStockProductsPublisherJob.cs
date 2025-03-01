﻿using Quartz;
using MediatR;
using MassTransit;
using JomashopNotifications.Application.Messages;
using JomashopNotifications.Application.Product.Queries;
using JomashopNotifications.Application.InStockProduct.Queries;
using JomashopNotifications.Persistence.Abstractions;
using Microsoft.Extensions.Logging;
using JomashopNotifications.Persistence.Entities.Product;

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
        var inStockProducts = await mediator.Send(new ListInStockProductsQuery());

        if (inStockProducts.Count == 0)
        {
            logger.LogInformation("No in stock products were found in the database");
            return;
        }

        var products = await mediator.Send(new ListProductsQuery()
        {
            Status = ProductStatus.Active,
            Ids = inStockProducts.Select(ip => ip.ProductId)
                                 .ToArray()
        });

        if (products.Count == 0)
        {
            logger.LogInformation("No active in stock products were found in the database");
            return;
        }

        logger.LogInformation(
            "Found {Count} active in stock products in the database. ProductIds: {ProductIds}",
            inStockProducts.Count,
            inStockProducts.Select(x => x.ProductId));

        var productInStockEvents = products.Join(
            inStockProducts,
            p => p.Id,
            ip => ip.ProductId,
            (p, ip) => new ProductInStockEvent
            {
                ProductId = p.Id,
                InStockProductId = ip.Id,
                Brand = p.Brand,
                Name = p.Name,
                Link = p.Link,
                Price = ip.Price,
                CheckedAt = ip.CheckedAt,
                ProductImages = p.ProductImages
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
        // WindowsToastNotificationHandler
        // Error Queue
    }
}