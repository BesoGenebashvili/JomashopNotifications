using JomashopNotifications.Persistence.Abstractions;
using MediatR;

namespace JomashopNotifications.Application.OutOfStockProduct.Commands;

public sealed record CreateOutOfStockProductCommand(int ProductId) : IRequest<int>;

public sealed class CreateOutOfStockProductCommandHandler(IOutOfStockProductsDatabase outOfStockProductsDatabase)
    : IRequestHandler<CreateOutOfStockProductCommand, int>
{
    public async Task<int> Handle(
        CreateOutOfStockProductCommand request,
        CancellationToken cancellationToken) =>
        await outOfStockProductsDatabase.InsertAsync(request.ProductId);
}