using MediatR;
using System.Text.Json.Serialization;
using JomashopNotifications.Domain;
using JomashopNotifications.Persistence.Abstractions;
using JomashopNotifications.Persistence.Entities;
using Microsoft.Extensions.Logging;
using JomashopNotifications.Persistence.Entities.Product;

namespace JomashopNotifications.Application.Product.Commands;

using Product = Domain.Models.Product;

public sealed record CreateProductCommand : IRequest<int>
{
    public required string Link { get; init; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public required ProductStatus Status { get; init; } = ProductStatus.Active;
}

public sealed class CreateProductCommandHandler(
    JomashopBrowserDriverService browserDriverService,
    IProductsDatabase productsDatabase,
    ILogger<CreateProductCommandHandler> logger) : IRequestHandler<CreateProductCommand, int>
{
    public async Task<int> Handle(
        CreateProductCommand request,
        CancellationToken cancellationToken)
    {
        //Add validation
        if (!Uri.IsWellFormedUriString(request.Link, UriKind.Absolute))
        {
            // ValidationException
            throw new Exception("Invalid URL: Request.Link");
        }

        var productFetchResults = await browserDriverService.FetchProductDataAsync(new(request.Link));

        return await productFetchResults.Match(
            async enriched =>
            {
                var (brand, name, images) = enriched switch
                {
                    Product.Enriched.Success(_, var b, var n, var i) => (b, n, i),
                    Product.Enriched.ParseError(_, var error) => throw new Exception($"Error while parsing product data: {error}"),
                    _ => throw new NotImplementedException(nameof(Product)),
                };

                var downloadImages = images.Select(i => DownloadImageBytesAsync(i.ImageLink));

                var imageBytes = await Task.WhenAll(downloadImages);

                var imageEntities = images.Zip(imageBytes)
                                          .Select(t => new ProductImageEntity
                                          {
                                              IsPrimary = t.First.IsPrimary,
                                              ImageData = t.Second
                                          })
                                          .ToList();

                var insertEntity = new InsertProductEntity
                {
                    Brand = brand,
                    Name = name,
                    Link = request.Link,
                    Status = request.Status,
                    Images = imageEntities
                };

                return await productsDatabase.InsertAsync(insertEntity);
            },
            error => throw new Exception($"Error while fetching product data: {error.Message}"));
    }

    private async Task<byte[]> DownloadImageBytesAsync(Uri link)
    {
        try
        {
            logger.LogInformation("Downloading image from {Link}", link);

            using var client = new HttpClient();
            return await client.GetByteArrayAsync(link);
        }
        catch (Exception ex)
        {
            logger.LogInformation(ex, "Error while downloading image from {Link}", link);
            // Log error in Error database? Custom exception?
            throw;
        }
    }
}