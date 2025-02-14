using JomashopNotifications.Persistence.Abstractions;
using MediatR;

namespace JomashopNotifications.Application.InStockProduct.Commands;

public sealed record UpsertInStockProductCommand(
    int ProductId, 
    decimal Price,
    DateTime CheckedAt) : IRequest<int>;

public sealed class UpsertInStockProductCommandHandler(IInStockProductsDatabase inStockProductsDatabase)
    : IRequestHandler<UpsertInStockProductCommand, int>
{
    public async Task<int> Handle(
        UpsertInStockProductCommand request,
        CancellationToken cancellationToken) =>
        await inStockProductsDatabase.UpsertAsync(
            request.ProductId, 
            request.Price,
            request.CheckedAt);
}