using MediatR;
using JomashopNotifications.Application.Product.Contracts;
using JomashopNotifications.Persistence.Abstractions;
using JomashopNotifications.Application.Common;

namespace JomashopNotifications.Application.Product.Queries;

public sealed record GetProductByIdQuery : IRequest<ProductDto?>
{
    public required int Id { get; init; }
}

public sealed class GetProductByIdQueryHandler(IProductsDatabase productsDatabase)
    : IRequestHandler<GetProductByIdQuery, ProductDto?>
{
    public async Task<ProductDto?> Handle(
        GetProductByIdQuery request,
        CancellationToken cancellationToken) =>
        await productsDatabase.GetAsync(request.Id) is { } product 
                ? product.ToDto()
                : null;
}

