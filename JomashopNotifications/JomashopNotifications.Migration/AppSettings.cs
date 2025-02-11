internal sealed record AppSettings(
    bool CreateDatabaseTables, 
    bool DeleteDatabaseTables)
{
    public const string SectionName = "AppSettings";
}
