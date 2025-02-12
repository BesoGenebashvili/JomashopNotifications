namespace JomashopNotifications.Domain.Common;

public static class StringExtensions
{
    public static string AsBrief(
        this string self,
        int characterCount = 50,
        string suffix = "...") =>
        self.Length <= characterCount
            ? self
            : self[..characterCount] + suffix;
}