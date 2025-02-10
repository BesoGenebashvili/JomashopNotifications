namespace JomashopNotifications.Domain.Common;

public static class StringExtensions
{
    public static string AsBrief(
        this string self,
        int characterCount,
        string suffix = "...") =>
        self.Length <= characterCount
            ? self
            : self[..characterCount] + suffix;
}