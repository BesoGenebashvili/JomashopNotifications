using System.Diagnostics.CodeAnalysis;

namespace JomashopNotifications.Domain.Common;

public readonly record struct Either<TLeft, TRight>
{
    [AllowNull]
    private readonly TLeft _left;

    [AllowNull]
    private readonly TRight _right;

    private readonly bool _isLeft;

    private Either([AllowNull] TLeft left, [AllowNull] TRight right, bool isLeft) =>
        (_left, _right, _isLeft) = (left, right, isLeft);

    public static Either<TLeft, TRight> Left(TLeft left) => new(left, default, true);

    public static Either<TLeft, TRight> Right(TRight right) => new(default, right, false);

    public T Match<T>(Func<TLeft, T> mapL, Func<TRight, T> mapR) =>
        _isLeft ? mapL(_left) : mapR(_right);

    public bool IsLeft([MaybeNullWhen(false)] out TLeft left)
    {
        left = _left;
        return _isLeft;
    }

    public bool IsRight([MaybeNullWhen(false)] out TRight right)
    {
        right = _right;
        return !_isLeft;
    }

    public override string ToString() => Match(l => $"Left {l}", r => $"Right {r}");
}