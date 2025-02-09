using MediatR;
using JomashopNotifications.Persistence.Abstractions;

namespace JomashopNotifications.Application.Product.Commands;

public sealed record SetStatusAsActiveCommand(int Id) : IRequest;

public sealed class SetStatusAsActiveCommandHandler(IProductsDatabase productsDatabase) 
    : IRequestHandler<SetStatusAsActiveCommand>
{
    public async Task Handle(SetStatusAsActiveCommand request, CancellationToken cancellationToken) =>
        await productsDatabase.SetStatusAsActiveAsync(request.Id);
}