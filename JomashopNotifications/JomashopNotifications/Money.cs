#pragma warning disable IDE0055

using System.Globalization;

namespace JomashopNotifications;

public sealed record Money(decimal Value, Currency Currency)
{
    public static Money Parse(string value) =>
        TryParse(value, out var result)
            ? result!
            : throw new FormatException($"Can't resolve Money from: {value}");

    public static bool TryParse(string value, out Money? result)
    {
        result = value switch
        {
            ['$', .. var n] => new(decimal.Parse(n, CultureInfo.InvariantCulture), Currency.USD),
            ['€', .. var n] => new(decimal.Parse(n, CultureInfo.InvariantCulture), Currency.EUR),
            _ => null,
        };

        return result != null;
    }
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