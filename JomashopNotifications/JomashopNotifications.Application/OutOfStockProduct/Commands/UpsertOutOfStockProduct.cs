using JomashopNotifications.Persistence.Abstractions;
using MediatR;

namespace JomashopNotifications.Application.OutOfStockProduct.Commands;

public sealed record UpsertOutOfStockProductCommand(int ProductId) : IRequest<int>;

public sealed class UpsertOutOfStockProductCommandHandler(IOutOfStockProductsDatabase outOfStockProductsDatabase)
    : IRequestHandler<UpsertOutOfStockProductCommand, int>
{
    public async Task<int> Handle(
        UpsertOutOfStockProductCommand request,
        CancellationToken cancellationToken) =>
        await outOfStockProductsDatabase.UpsertAsync(request.ProductId);
}