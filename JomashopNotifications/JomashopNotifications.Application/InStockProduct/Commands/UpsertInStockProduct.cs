using JomashopNotifications.Domain.Models;
using JomashopNotifications.Persistence.Abstractions;
using JomashopNotifications.Persistence.Entities.InStockProduct;
using MediatR;

namespace JomashopNotifications.Application.InStockProduct.Commands;

public sealed record UpsertInStockProductCommand(
    int ProductId, 
    decimal Price,
    Currency Currency,
    DateTime CheckedAt) : IRequest<int>;

public sealed class UpsertInStockProductCommandHandler(IInStockProductsDatabase inStockProductsDatabase)
    : IRequestHandler<UpsertInStockProductCommand, int>
{
    public async Task<int> Handle(
        UpsertInStockProductCommand request,
        CancellationToken cancellationToken) =>
        await inStockProductsDatabase.UpsertAsync(
            new UpsertInStockProductEntity
            {
                ProductId = request.ProductId,
                Price = request.Price,
                Currency = request.Currency.ToString(),
                CheckedAt = request.CheckedAt
            });
}