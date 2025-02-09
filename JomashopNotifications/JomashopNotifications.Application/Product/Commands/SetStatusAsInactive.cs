using MediatR;
using JomashopNotifications.Persistence.Abstractions;

namespace JomashopNotifications.Application.Product.Commands;

public sealed record SetStatusAsInactiveCommand : IRequest<bool>
{
    public required int Id { get; init; }
}

public sealed class SetStatusAsInactiveCommandHandler(IProductsDatabase productsDatabase)
    : IRequestHandler<SetStatusAsInactiveCommand, bool>
{
    public async Task<bool> Handle(SetStatusAsInactiveCommand request, CancellationToken cancellationToken) =>
        await productsDatabase.SetStatusAsInactiveAsync(request.Id);
}
