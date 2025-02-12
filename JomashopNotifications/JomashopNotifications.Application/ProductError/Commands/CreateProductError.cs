using JomashopNotifications.Persistence.Abstractions;
using MediatR;

namespace JomashopNotifications.Application.ProductError.Commands;

public sealed record CreateProductErrorCommand(int ProductId, string Message) : IRequest<int>;

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