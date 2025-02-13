using Quartz;
using MediatR;
using Serilog;
using JomashopNotifications.Domain;
using JomashopNotifications.Domain.Common;
using JomashopNotifications.Domain.Models;
using JomashopNotifications.Persistence.Entities;
using JomashopNotifications.Application.Product.Queries;
using JomashopNotifications.Application.Product.Contracts;
using JomashopNotifications.Application.ProductError.Commands;
using JomashopNotifications.Application.InStockProduct.Commands;
using JomashopNotifications.Application.OutOfStockProduct.Commands;

namespace JomashopNotifications.Worker;

[DisallowConcurrentExecution]
public sealed class JomashopDataSyncJob(
    IMediator mediator,
    JomashopBrowserDriverService browserDriverService) : IJob
{
    public static readonly JobKey key =
        new(nameof(JomashopDataSyncJob), "DataSync");

    public async Task Execute(IJobExecutionContext context)
    {
        var activeProducts = await GetActiveProductsAsync();

        if (activeProducts.Count == 0)
        {
            Log.Information("No active products found in the database");
            return;
        }

        Log.Information(
            "Found {Count} active products in the database: {ProductIds}",
            activeProducts.Count,
            activeProducts.Select(x => x.Id));

        var invalidActiveProducts = activeProducts.Where(p => !Uri.IsWellFormedUriString(p.Link, UriKind.Absolute));

        if (invalidActiveProducts.Any())
        {
            Log.Warning(
                "Found {Count} active products with invalid links: {ProductIds}",
                invalidActiveProducts.Count(),
                invalidActiveProducts.Select(p => p.Id));

            activeProducts = activeProducts.Except(invalidActiveProducts)
                                           .ToList();
        }

        //                                            Extension?
        var productsToCheck = activeProducts.Select(p => new Product.ToBeChecked(p.Id, new(p.Link)));

        var productCheckResults = await browserDriverService.CheckProductsAsync(productsToCheck);

        var (successfullyChecked, browserDriverErrors) = productCheckResults.Partition();

        if (browserDriverErrors.Any())
        {
            Log.Error(
                "An error occurred while operating with the browser for products: {ProductIds}",
                browserDriverErrors.Select(e => e.ProductId));

            // AppErrors data table ?
        }

        var inStockProducts = successfullyChecked.OfType<Product.Checked.InStock>();
        var OutOfStockProducts = successfullyChecked.OfType<Product.Checked.OutOfStock>();
        var productErrors = successfullyChecked.OfType<Product.Checked.Error>();

        Log.Debug("Successfully checked products: {ProductIds}", successfullyChecked.Select(p => p.Reference.Id));
        Log.Information("In stock products: {ProductIds}", inStockProducts.Select(p => p.Reference.Id));
        Log.Information("Out of stock products: {ProductIds}", OutOfStockProducts.Select(p => p.Reference.Id));
        Log.Information("Product errors: {ProductIds}", productErrors.Select(p => p.Reference.Id));

        var upsertCommands = successfullyChecked.Select(p => mediator.Send(ResolveUpsertCommand(p)));

        // More information on which products were upserted and which failed
        try
        {
            await Task.WhenAll(upsertCommands);
            Log.Information("Successfully upserted products");
        }
        catch (Exception ex)
        {
            Log.Error("An error occurred while upserting products. Error: {ex}", ex);
        }

        Task<List<ProductDto>> GetActiveProductsAsync() =>
            mediator.Send(
                new ListProductsQuery()
                {
                    Status = ProductStatus.Active
                });

        static IRequest<int> ResolveUpsertCommand(Product.Checked @checked) =>
            @checked switch
            {
                Product.Checked.InStock(var reference, var price) => new UpsertInStockProductCommand(reference.Id, price.Value),
                Product.Checked.OutOfStock(var reference) => new UpsertOutOfStockProductCommand(reference.Id),
                Product.Checked.Error(var reference, var message) => new UpsertProductErrorCommand(reference.Id, message),
                _ => throw new NotImplementedException(nameof(Product.Checked))
            };
    }
}