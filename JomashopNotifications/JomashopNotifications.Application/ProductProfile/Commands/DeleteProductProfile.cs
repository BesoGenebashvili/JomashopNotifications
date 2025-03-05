using MediatR;
using JomashopNotifications.Persistence.Abstractions;

namespace JomashopNotifications.Application.ProductProfile.Commands;

public sealed record DeleteProductProfileCommand(int ProductId) : IRequest<bool>;

public sealed class DeleteProductProfileCommandHandler(IProductProfilesDatabase productProfilesDatabase)
    : IRequestHandler<DeleteProductProfileCommand, bool>
{
    public Task<bool> Handle(
        DeleteProductProfileCommand request,
        CancellationToken cancellationToken) =>
        productProfilesDatabase.DeleteAsync(request.ProductId);
}