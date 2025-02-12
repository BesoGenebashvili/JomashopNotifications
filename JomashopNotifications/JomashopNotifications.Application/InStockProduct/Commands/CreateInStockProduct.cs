using JomashopNotifications.Persistence.Abstractions;
using MediatR;

namespace JomashopNotifications.Application.InStockProduct.Commands;

public sealed record CreateInStockProductCommand(
    int ProductId, 
    decimal Price) : IRequest<int>;

public sealed class CreateInStockProductCommandHandler(IInStockProductsDatabase inStockProductsDatabase)
    : IRequestHandler<CreateInStockProductCommand, int>
{
    public async Task<int> Handle(
        CreateInStockProductCommand request,
        CancellationToken cancellationToken) =>
        await inStockProductsDatabase.InsertAsync(
            request.ProductId, 
            request.Price);
}