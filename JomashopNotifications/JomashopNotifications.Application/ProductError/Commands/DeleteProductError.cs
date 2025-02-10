using MediatR;
using JomashopNotifications.Persistence.Abstractions;

namespace JomashopNotifications.Application.ProductError.Commands;

public sealed record DeleteProductErrorCommand(int Id) : IRequest<bool>;

public sealed class DeleteProductErrorCommandHandler(IProductErrorsDatabase productErrorsDatabase)
    : IRequestHandler<DeleteProductErrorCommand, bool>
{
    public Task<bool> Handle(
        DeleteProductErrorCommand request,
        CancellationToken cancellationToken) =>
        productErrorsDatabase.DeleteAsync(request.Id);
}