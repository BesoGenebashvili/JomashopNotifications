using MediatR;
using JomashopNotifications.Persistence.Abstractions;

namespace JomashopNotifications.Application.Product.Commands;

public sealed record DeactivateProductCommand(int Id) : IRequest;

public sealed class DeactivateProductCommandHandler(IProductsDatabase productsDatabase)
    : IRequestHandler<DeactivateProductCommand>
{
    public async Task Handle(DeactivateProductCommand request, CancellationToken cancellationToken) =>
        await productsDatabase.DeactivateAsync(request.Id);
}
