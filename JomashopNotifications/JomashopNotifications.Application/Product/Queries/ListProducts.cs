using MediatR;
using JomashopNotifications.Application.Product.Contracts;
using JomashopNotifications.Application.Common;
using JomashopNotifications.Persistence.Abstractions;
using JomashopNotifications.Persistence.Entities;

namespace JomashopNotifications.Application.Product.Queries;

public sealed record ListProductsQuery : IRequest<List<ProductDto>>
{
    public ProductStatus? Status { get; init; }
    public int[]? Ids { get; init; }
}

public sealed class ListProductsQueryHandler(IProductsDatabase productsDatabase)
    : IRequestHandler<ListProductsQuery, List<ProductDto>>
{
    public async Task<List<ProductDto>> Handle(ListProductsQuery request, CancellationToken cancellationToken)
    {
        var products = await productsDatabase.ListAsync(request.Status, request.Ids);

        return products.Select(ProductExtensions.ToDto)
                       .ToList();
    }
}