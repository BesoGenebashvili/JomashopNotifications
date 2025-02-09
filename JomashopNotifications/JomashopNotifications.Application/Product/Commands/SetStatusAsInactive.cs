using MediatR;
using JomashopNotifications.Persistence.Abstractions;

namespace JomashopNotifications.Application.Product.Commands;

public sealed record SetStatusAsInactiveCommand(int Id) : IRequest;

public sealed class SetStatusAsInactiveCommandHandler(IProductsDatabase productsDatabase)
    : IRequestHandler<SetStatusAsInactiveCommand>
{
    public async Task Handle(SetStatusAsInactiveCommand request, CancellationToken cancellationToken) =>
        await productsDatabase.SetStatusAsInactiveAsync(request.Id);
}
