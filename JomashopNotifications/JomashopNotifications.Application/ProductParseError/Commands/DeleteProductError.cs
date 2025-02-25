using MediatR;
using JomashopNotifications.Persistence.Abstractions;

namespace JomashopNotifications.Application.ProductParseError.Commands;

public sealed record DeleteProductParseErrorCommand(int Id) : IRequest<bool>;

public sealed class DeleteProductParseErrorCommandHandler(IProductParseErrorsDatabase productParseErrorsDatabase)
    : IRequestHandler<DeleteProductParseErrorCommand, bool>
{
    public Task<bool> Handle(
        DeleteProductParseErrorCommand request,
        CancellationToken cancellationToken) =>
        productParseErrorsDatabase.DeleteAsync(request.Id);
}