using JomashopNotifications.Domain.Models;

namespace JomashopNotifications.Application.InStockProduct.Contracts;

public sealed record MoneyDto(
    decimal Amount, 
    Currency Currency);
