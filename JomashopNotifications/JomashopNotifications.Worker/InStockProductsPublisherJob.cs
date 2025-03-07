using Quartz;
using MediatR;
using MassTransit;
using JomashopNotifications.Application.Messages;
using JomashopNotifications.Application.Product.Queries;
using JomashopNotifications.Application.InStockProduct.Queries;
using JomashopNotifications.Persistence.Abstractions;
using Microsoft.Extensions.Logging;
using JomashopNotifications.Persistence.Entities.Product;
using JomashopNotifications.Application.ProductProfile.Queries;

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

        if (inStockProducts.Count is 0)
        {
            logger.LogInformation("No in stock products were found in the database");
            return;
        }

        var products = await mediator.Send(
            new ListProductsQuery
            {
                Status = ProductStatus.Active,
                Ids = inStockProducts.Select(ip => ip.ProductId)
                                     .ToArray()
            });

        if (products.Count is 0)
        {
            logger.LogInformation("No active in stock products were found in the database");
            return;
        }

        var productProfiles = await mediator.Send(
            new ListProductProfilesQuery
            {
                ProductIds = products.Select(p => p.Id).ToArray()
            });

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
            "Found {Count} active in stock products in the database: {ProductIds}",
            inStockProducts.Count,
            inStockProducts.Select(x => x.ProductId));

        var productInStockEventsToPublish = productInStockEvents.Where(
                p => MeetsProfileRequirements(p.ProductId, p.Price.Amount));

        var productInStockEventsToSkip = productInStockEvents.Except(productInStockEventsToPublish);

        if (productInStockEventsToSkip.Any())
            logger.LogInformation(
                "Skipping {Count} 'ProductInStockEvent' events for products: {ProductIds}",
                productInStockEventsToSkip.Count(),
                productInStockEventsToSkip.Select(x => x.ProductId));

        logger.LogInformation(
            "Publishing {Count} 'ProductInStockEvent' events for products: {ProductIds}",
            productInStockEventsToPublish.Count(),
            productInStockEventsToPublish.Select(e => e.ProductId));

        List<int> successfullyPublished = [];

        foreach (var @event in productInStockEventsToPublish)
        {
            try
            {
                await bus.Publish(@event);
                successfullyPublished.Add(@event.ProductId);
            }
            catch (Exception ex)
            {
                logger.LogError(
                    "An error occurred while publishing 'ProductInStockEvent' for: {ProductId}. Error: {ex}",
                    @event.ProductId,
                    ex);

                await applicationErrorsDatabase.LogAsync(ex);
            }
        }

        if (successfullyPublished.Count is not 0)
        {
            logger.LogInformation(
                "Successfully published 'ProductInStockEvent' events for products: {ProductIds}",
                successfullyPublished);
        }

        bool MeetsProfileRequirements(int productId, decimal price)
        {
            var profile = productProfiles.FirstOrDefault(p => p.ProductId == productId);

            return profile switch
            {
                { IsActive: true, PriceThreshold: var priceThreshold } => price <= profile.PriceThreshold,
                _ => true,
            };
        }

        // Error Queue ?
    }
}