internal sealed record AppSettings
{
    public const string SectionName = "AppSettings";

    public required bool CreateDatabaseTables { get; init; }
    public required bool DeleteDatabaseTables { get; init; }
}
