using MediatR;
using JomashopNotifications.Persistence.Abstractions;

namespace JomashopNotifications.Application.ProductProfile.Commands;

public sealed record ActivateProductProfileCommand(int ProductId) : IRequest;

public sealed class ActivateProductProfileCommandCommandHandler(IProductProfilesDatabase productProfilesDatabase)
    : IRequestHandler<ActivateProductProfileCommand>
{
    public async Task Handle(ActivateProductProfileCommand request, CancellationToken cancellationToken) =>
        await productProfilesDatabase.ActivateAsync(request.ProductId);
}