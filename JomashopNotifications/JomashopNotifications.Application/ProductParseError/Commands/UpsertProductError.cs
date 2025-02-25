using JomashopNotifications.Persistence.Abstractions;
using MediatR;

namespace JomashopNotifications.Application.ProductParseError.Commands;

public sealed record UpsertProductParseErrorCommand(int ProductId, string Message, DateTime CheckedAt) : IRequest<int>;

public sealed class UpsertProductErrorCommandHandler(IProductParseErrorsDatabase productParseErrorsDatabase)
    : IRequestHandler<UpsertProductParseErrorCommand, int>
{
    public async Task<int> Handle(
        UpsertProductParseErrorCommand request,
        CancellationToken cancellationToken) =>
        await productParseErrorsDatabase.UpsertAsync(
            request.ProductId,
            request.Message,
            request.CheckedAt);
}