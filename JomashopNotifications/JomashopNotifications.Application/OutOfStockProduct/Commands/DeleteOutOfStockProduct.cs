using MediatR;
using JomashopNotifications.Persistence.Abstractions;

namespace JomashopNotifications.Application.OutOfStockProduct.Commands;

public sealed record DeleteOutOfStockProductCommand(int Id) : IRequest<bool>;

public sealed class DeleteOutOfStockProductCommandHandler(IOutOfStockProductsDatabase outOfStockProductsDatabase)
    : IRequestHandler<DeleteOutOfStockProductCommand, bool>
{
    public Task<bool> Handle(
        DeleteOutOfStockProductCommand request,
        CancellationToken cancellationToken) =>
        outOfStockProductsDatabase.DeleteAsync(request.Id);
}