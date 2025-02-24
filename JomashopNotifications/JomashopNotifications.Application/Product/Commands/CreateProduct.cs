using MediatR;
using System.Text.Json.Serialization;
using JomashopNotifications.Domain;
using JomashopNotifications.Persistence.Abstractions;
using JomashopNotifications.Persistence.Entities;
using Product = JomashopNotifications.Domain.Models.Product;

namespace JomashopNotifications.Application.Product.Commands;

public sealed record CreateProductCommand : IRequest<int>
{
    public required string Link { get; init; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public required ProductStatus Status { get; init; } = ProductStatus.Active;
}

public sealed class CreateProductCommandHandler(
    JomashopBrowserDriverService browserDriverService,
    IProductsDatabase productsDatabase) : IRequestHandler<CreateProductCommand, int>
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
                var (brand, name) = enriched switch
                {
                    Domain.Models.Product.Enriched.Success(_, var b, var n) => (b, n),
                    Domain.Models.Product.Enriched.ParseError(_, var error) => throw new Exception($"Error while parsing product data: {error}"),
                    _ => throw new NotImplementedException(nameof(Domain.Models.Product)),
                };

                var insertEntity = new InsertProductEntity
                {
                    Brand = brand,
                    Name = name,
                    Link = request.Link,
                    Status = request.Status
                };

                return await productsDatabase.InsertAsync(insertEntity);
            },
            error => throw new Exception($"Error while fetching product data: {error.Message}"));
    }
}