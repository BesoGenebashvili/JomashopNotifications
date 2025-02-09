using MediatR;
using JomashopNotifications.Persistence.Abstractions;

namespace JomashopNotifications.Application.Product.Commands;

public sealed record DeleteProductCommand(int Id) : IRequest<bool>;

public sealed class DeleteProductCommandHandler(IProductsDatabase productsDatabase)
    : IRequestHandler<DeleteProductCommand, bool>
{
    public async Task<bool> Handle(
        DeleteProductCommand request,
        CancellationToken cancellationToken) =>
        await productsDatabase.DeleteAsync(request.Id);
}