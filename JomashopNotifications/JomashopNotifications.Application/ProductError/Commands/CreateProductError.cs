using JomashopNotifications.Persistence.Abstractions;
using MediatR;

namespace JomashopNotifications.Application.ProductError.Commands;

public sealed record CreateProductErrorCommand : IRequest<int>
{
    public required int ProductId { get; init; }
    public required string Message { get; init; }
}

public sealed class CreateProductErrorCommandHandler(IProductErrorsDatabase productErrorsDatabase)
    : IRequestHandler<CreateProductErrorCommand, int>
{
    public async Task<int> Handle(
        CreateProductErrorCommand request,
        CancellationToken cancellationToken) =>
        await productErrorsDatabase.InsertAsync(
            request.ProductId, 
            request.Message);
}