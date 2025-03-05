using MediatR;
using JomashopNotifications.Persistence.Abstractions;

namespace JomashopNotifications.Application.ProductProfile.Commands;

public sealed record DeactivateProductProfileCommand(int ProductId) : IRequest;

public sealed class DeactivateProductProfileCommandCommandHandler(IProductProfilesDatabase productProfilesDatabase)
    : IRequestHandler<DeactivateProductProfileCommand>
{
    public async Task Handle(DeactivateProductProfileCommand request, CancellationToken cancellationToken) =>
        await productProfilesDatabase.DeactivateAsync(request.ProductId);
}