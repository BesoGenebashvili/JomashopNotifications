public record BrowserDriverError(string Message);

public static class BrowserDriverErrorExtensions
{
    public static BrowserDriverError FromException(this Exception self) =>
        new(self.Message);
}