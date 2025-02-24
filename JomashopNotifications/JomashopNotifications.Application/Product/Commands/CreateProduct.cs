using MediatR;
using System.Text.Json.Serialization;
using JomashopNotifications.Domain;
using JomashopNotifications.Persistence.Abstractions;
using JomashopNotifications.Persistence.Entities;

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
            async brandAndName =>
            {
                var insertEntity = new InsertProductEntity
                {
                    Brand = brandAndName.brand,
                    Name = brandAndName.name,
                    Link = request.Link,
                    Status = request.Status
                };

                return await productsDatabase.InsertAsync(insertEntity);
            },
            error => throw new Exception($"Error while fetching product data: {error.Message}"));
    }
}