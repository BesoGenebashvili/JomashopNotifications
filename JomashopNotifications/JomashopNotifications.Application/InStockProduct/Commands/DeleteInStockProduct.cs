using MediatR;
using JomashopNotifications.Persistence.Abstractions;

namespace JomashopNotifications.Application.InStockProduct.Commands;

public sealed record DeleteInStockProductCommand(int Id) : IRequest<bool>;

public sealed class DeleteInStockProductCommandHandler(IInStockProductsDatabase inStockProductsDatabase)
    : IRequestHandler<DeleteInStockProductCommand, bool>
{
    public Task<bool> Handle(
        DeleteInStockProductCommand request,
        CancellationToken cancellationToken) =>
        inStockProductsDatabase.DeleteAsync(request.Id);
}