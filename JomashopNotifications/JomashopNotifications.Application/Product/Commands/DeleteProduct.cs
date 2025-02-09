using MediatR;
using JomashopNotifications.Persistence.Abstractions;

namespace JomashopNotifications.Application.Product.Commands;

public sealed record DeleteProductCommand : IRequest<bool>
{
    public required int Id { get; init; }
}

public sealed class DeleteProductCommandHandler(IProductsDatabase productsDatabase)
    : IRequestHandler<DeleteProductCommand, bool>
{
    public async Task<bool> Handle(
        DeleteProductCommand request,
        CancellationToken cancellationToken) =>
        await productsDatabase.DeleteAsync(request.Id);
}