﻿using JomashopNotifications.Persistence.Abstractions;
using MediatR;

namespace JomashopNotifications.Application.ProductError.Commands;

public sealed record UpsertProductErrorCommand(int ProductId, string Message, DateTime CheckedAt) : IRequest<int>;

public sealed class UpsertProductErrorCommandHandler(IProductErrorsDatabase productErrorsDatabase)
    : IRequestHandler<UpsertProductErrorCommand, int>
{
    public async Task<int> Handle(
        UpsertProductErrorCommand request,
        CancellationToken cancellationToken) =>
        await productErrorsDatabase.UpsertAsync(
            request.ProductId,
            request.Message,
            request.CheckedAt);
}