using MediatR;
using JomashopNotifications.Application.Common;
using JomashopNotifications.Persistence.Abstractions;
using JomashopNotifications.Application.ProductProfile.Contracts;

namespace JomashopNotifications.Application.ProductProfile.Queries;

public sealed record ListProductProfilesQuery : IRequest<List<ProductProfileDto>>
{
    public int[]? ProductIds { get; init; }
}

public sealed class ListProductProfilesQueryHandler(IProductProfilesDatabase productProfilesDatabase)
    : IRequestHandler<ListProductProfilesQuery, List<ProductProfileDto>>
{
    public async Task<List<ProductProfileDto>> Handle(ListProductProfilesQuery request, CancellationToken cancellationToken)
    {
        var productProfiles = await productProfilesDatabase.ListAsync(request.ProductIds);

        return productProfiles.Select(ProductExtensions.ToDto)
                              .ToList();
    }
}