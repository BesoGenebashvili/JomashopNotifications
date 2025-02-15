using MediatR;
using JomashopNotifications.Application.Common;
using JomashopNotifications.Persistence.Abstractions;
using JomashopNotifications.Application.InStockProduct.Contracts;

namespace JomashopNotifications.Application.InStockProduct.Queries;

public sealed record ListInStockProductsQuery : IRequest<List<InStockProductDto>>;

public sealed class ListInStockProductsQueryHandler(IInStockProductsDatabase inStockProductsDatabase)
    : IRequestHandler<ListInStockProductsQuery, List<InStockProductDto>>
{
    public async Task<List<InStockProductDto>> Handle(ListInStockProductsQuery request, CancellationToken cancellationToken)
    {
        var inStockProducts = await inStockProductsDatabase.ListAsync();

        return inStockProducts.Select(ProductExtensions.ToDto)
                              .ToList();
    }
}