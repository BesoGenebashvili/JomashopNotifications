using System.Collections;

namespace JomashopNotifications.Domain.Common;

public static class EnumerableExtensions
{
    public static TCollection? NullIfEmpty<TCollection>(this TCollection? self)
        where TCollection : class, ICollection =>
        self is { Count: > 0 } 
            ? self 
            : null;

    public static IEnumerable<T>? NullIfEmpty<T>(this IEnumerable<T>? self) =>
        self is { } && self.Any()
            ? self
            : null;

    public static IEnumerable<TLeft> Lefts<TLeft, TRight>(
        this IEnumerable<Either<TLeft, TRight>> self)
    {
        using var e = self.GetEnumerator();

        while (e.MoveNext())
        {
            if (e.Current.IsLeft(out var left))
                yield return left;
        }
    }

    public static IEnumerable<TRight> Rights<TLeft, TRight>(
        this IEnumerable<Either<TLeft, TRight>> self)
    {
        using var e = self.GetEnumerator();

        while (e.MoveNext())
        {
            if (e.Current.IsRight(out var right))
                yield return right;
        }
    }

    public static (IEnumerable<TLeft> Left, IEnumerable<TRight> Right) Partition<TLeft, TRight>(
        this IEnumerable<Either<TLeft, TRight>> self) =>
        (self.Lefts(), self.Rights());
}
