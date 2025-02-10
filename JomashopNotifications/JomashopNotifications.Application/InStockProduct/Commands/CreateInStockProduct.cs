using JomashopNotifications.Persistence.Abstractions;
using MediatR;

namespace JomashopNotifications.Application.InStockProduct.Commands;

public sealed record CreateInStockProductCommand : IRequest<int>
{
    public required int ProductId { get; init; }
    public required decimal Price { get; init; }
}

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