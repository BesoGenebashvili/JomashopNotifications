using Quartz;
using MediatR;
using JomashopNotifications.Domain;
using JomashopNotifications.Domain.Common;
using JomashopNotifications.Domain.Models;
using JomashopNotifications.Application.Product.Queries;
using JomashopNotifications.Application.Product.Contracts;
using JomashopNotifications.Application.InStockProduct.Commands;
using JomashopNotifications.Application.OutOfStockProduct.Commands;
using JomashopNotifications.Application.ProductParseError.Commands;
using JomashopNotifications.Persistence.Abstractions;
using Microsoft.Extensions.Logging;
using JomashopNotifications.Persistence.Entities.Product;

namespace JomashopNotifications.Worker;

[DisallowConcurrentExecution]
public sealed class JomashopDataSyncJob(
    IMediator mediator,
    JomashopBrowserDriverService browserDriverService,
    IApplicationErrorsDatabase applicationErrorsDatabase,
    ILogger<JomashopDataSyncJob> logger) : IJob
{
    public static readonly JobKey key =
        new(nameof(JomashopDataSyncJob), "DataSync");

    // I can pass cancellationToken from IJobExecutionContext to commands
    public async Task Execute(IJobExecutionContext _)
    {
        var activeProducts = await GetActiveProductsAsync();

        if (activeProducts.Count == 0)
        {
            logger.LogInformation("No active products were found in the database");
            return;
        }

        logger.LogInformation(
            "Found {Count} active products in the database: {ProductIds}",
            activeProducts.Count,
            activeProducts.Select(x => x.Id));

        var invalidActiveProducts = activeProducts.Where(p => !Uri.IsWellFormedUriString(p.Link, UriKind.Absolute));

        if (invalidActiveProducts.Any())
        {
            logger.LogWarning(
                "Found {Count} active products with invalid links: {ProductIds}",
                invalidActiveProducts.Count(),
                invalidActiveProducts.Select(p => p.Id));

            activeProducts = activeProducts.Except(invalidActiveProducts)
                                           .ToList();
        }

        //                                            Extension?
        var productsToCheck = activeProducts.Select(p => new Product.ToCheck(p.Id, new(p.Link)));

        var productCheckResults = await browserDriverService.CheckProductsAsync(productsToCheck);

        var (successfullyCheckedProducts, browserDriverErrors) = productCheckResults.Partition();

        if (browserDriverErrors.Any())
        {
            logger.LogError(
                "An error occurred while operating with the browser for products: {ProductIds}",
                browserDriverErrors.Select(e => e.id));

            await Task.WhenAll(
                browserDriverErrors.Select(
                    e => applicationErrorsDatabase.LogAsync(e.error.Exception)));
        }

        if (!successfullyCheckedProducts.Any())
        {
            logger.LogWarning("No products were successfully checked");
            return;
        }

        var inStockProducts = successfullyCheckedProducts.OfType<Product.Checked.InStock>();
        var OutOfStockProducts = successfullyCheckedProducts.OfType<Product.Checked.OutOfStock>();
        var productErrors = successfullyCheckedProducts.OfType<Product.Checked.ParseError>();

        logger.LogDebug("Successfully checked products: {ProductIds}", successfullyCheckedProducts.Select(p => p.Reference.Id));
        logger.LogInformation("In stock products: {ProductIds}", inStockProducts.Select(p => p.Reference.Id));
        logger.LogInformation("Out of stock products: {ProductIds}", OutOfStockProducts.Select(p => p.Reference.Id));
        logger.LogInformation("Product errors: {ProductIds}", productErrors.Select(p => p.Reference.Id));

        await UpsertSuccessfullyCheckedProducts();

        Task<List<ProductDto>> GetActiveProductsAsync() =>
            mediator.Send(
                new ListProductsQuery()
                {
                    Status = ProductStatus.Active
                });

        async Task UpsertSuccessfullyCheckedProducts()
        {
            foreach (var product in successfullyCheckedProducts)
            {
                var command = ResolveUpsertCommand(product);

                try
                {
                    await mediator.Send(command);
                    logger.LogInformation("Successfully upserted product {ProductId}", product.Reference.Id);
                }
                catch (Exception ex)
                {
                    logger.LogError(
                        "An error occurred while upserting product {ProductId}. Error: {ex}",
                        product.Reference.Id,
                        ex);

                    await applicationErrorsDatabase.LogAsync(ex);
                }
            }

            static IRequest<int> ResolveUpsertCommand(Product.Checked @checked) =>
                @checked switch
                {
                    Product.Checked.InStock({ Id: var pId }, var price, var checkedAt) => new UpsertInStockProductCommand(pId, price.Value, price.Currency, checkedAt),
                    Product.Checked.OutOfStock({ Id: var pId }, var checkedAt) => new UpsertOutOfStockProductCommand(pId, checkedAt),
                    Product.Checked.ParseError({ Id: var pId }, var message, var checkedAt) => new UpsertProductParseErrorCommand(pId, message, checkedAt),
                    _ => throw new NotImplementedException(nameof(Product.Checked))
                };
        }
    }
}