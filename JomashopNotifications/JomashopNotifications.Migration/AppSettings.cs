internal sealed record AppSettings
{
    public const string SectionName = "AppSettings";

    public bool CreateDatabaseTables { get; init; }
    public bool DeleteDatabaseTables { get; init; }
}
