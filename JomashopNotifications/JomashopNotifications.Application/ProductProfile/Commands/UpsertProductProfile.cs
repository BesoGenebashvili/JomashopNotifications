using JomashopNotifications.Persistence.Abstractions;
using MediatR;

namespace JomashopNotifications.Application.ProductProfile.Commands;

public sealed record UpsertProductProfileCommand(
    int ProductId,
    decimal PriceThreshold) : IRequest<int>;

public sealed class UpsertProductProfileCommandCommandHandler(IProductProfilesDatabase productProfilesDatabase)
    : IRequestHandler<UpsertProductProfileCommand, int>
{
    public async Task<int> Handle(
        UpsertProductProfileCommand request,
        CancellationToken cancellationToken) =>
        await productProfilesDatabase.UpsertAsync(
            request.ProductId,
            request.PriceThreshold);
}