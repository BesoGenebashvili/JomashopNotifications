#pragma warning disable IDE0055

namespace JomashopNotifications.Domain.Models;

// Add rounding.
public sealed record Money(decimal Value, Currency Currency)
{
    public static Money Parse(string value) =>
        TryParse(value, out var result)
            ? result!
            : throw new FormatException($"Can't resolve Money from: {value}");

    // This logic shouldn't be here
    public static bool TryParse(string value, out Money? result)
    {
        result = value.Trim() switch
        {
            // $100
            ['$', .. var n] when decimal.TryParse(n.Trim(), out decimal v) => new(v, Currency.USD),
            ['€', .. var n] when decimal.TryParse(n.Trim(), out decimal v) => new(v, Currency.EUR),

            // 100€
            [.. var n, '$'] when decimal.TryParse(n.Trim(), out decimal v) => new(v, Currency.USD),
            [.. var n, '€'] when decimal.TryParse(n.Trim(), out decimal v) => new(v, Currency.EUR),

            _ => null,
        };

        return result != null;
    }

    public static Money Zero(Currency currency) => new(0, currency);
}

public enum Currency : byte
{
    USD,
    EUR
}

public static class CurrencyExtensions
{
    public static char AsSymbol(this Currency self) => self switch
    {
        Currency.USD => '$',
        Currency.EUR => '€',
        _ => throw new NotImplementedException(nameof(Currency))
    };
}