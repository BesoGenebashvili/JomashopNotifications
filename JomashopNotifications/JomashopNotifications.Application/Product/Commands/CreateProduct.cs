using JomashopNotifications.Persistence.Abstractions;
using JomashopNotifications.Persistence.Entities;
using MediatR;
using System.Text.Json.Serialization;

//Add validation
namespace JomashopNotifications.Application.Product.Commands;

public sealed record CreateProductCommand : IRequest<int>
{
    public required string Link { get; init; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public required ProductStatus Status { get; init; } = ProductStatus.Active;
}

public sealed class CreateProductCommandHandler(IProductsDatabase productsDatabase)
    : IRequestHandler<CreateProductCommand, int>
{
    public async Task<int> Handle(
        CreateProductCommand request,
        CancellationToken cancellationToken)
    {
        // fetch information from website
        var brand = "";
        var name = "";

        var insertEntity = new InsertProductEntity
        {
            Brand = brand,
            Name = name,
            Link = request.Link,
            Status = request.Status
        };

        return await productsDatabase.InsertAsync(insertEntity);
    }
}