using JomashopNotifications.Persistence.Abstractions;
using JomashopNotifications.Persistence.Entities;
using MediatR;

//Add validation
namespace JomashopNotifications.Application.Product.Commands;

public sealed record CreateProductCommand : IRequest<int>
{
    public required string Link { get; init; }
    public required ProductStatus Status { get; init; } = ProductStatus.Active;
}

public sealed class CreateProductCommandHandler(IProductsDatabase productsDatabase)
    : IRequestHandler<CreateProductCommand, int>
{
    public async Task<int> Handle(
        CreateProductCommand request,
        CancellationToken cancellationToken)
    {
        var insertEntity = new InsertProductEntity
        {
            Link = request.Link,
            Status = request.Status
        };

        return await productsDatabase.InsertAsync(insertEntity);
    }
}