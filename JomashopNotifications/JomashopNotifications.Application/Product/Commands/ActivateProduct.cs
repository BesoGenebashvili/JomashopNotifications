using MediatR;
using JomashopNotifications.Persistence.Abstractions;

namespace JomashopNotifications.Application.Product.Commands;

public sealed record ActivateProductCommand(int Id) : IRequest;

public sealed class ActivateProductCommandHandler(IProductsDatabase productsDatabase) 
    : IRequestHandler<ActivateProductCommand>
{
    public async Task Handle(ActivateProductCommand request, CancellationToken cancellationToken) =>
        await productsDatabase.ActivateAsync(request.Id);
}