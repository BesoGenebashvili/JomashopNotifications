using MediatR;
using JomashopNotifications.Persistence.Abstractions;

namespace JomashopNotifications.Application.Product.Commands;

public sealed record SetStatusAsActiveCommand : IRequest<bool>
{
    public required int Id { get; init; }
}

public sealed class SetStatusAsActiveCommandHandler(IProductsDatabase productsDatabase) 
    : IRequestHandler<SetStatusAsActiveCommand, bool>
{
    public async Task<bool> Handle(SetStatusAsActiveCommand request, CancellationToken cancellationToken) =>
        await productsDatabase.SetStatusAsActiveAsync(request.Id);
}