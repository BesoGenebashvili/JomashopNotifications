using Dapper;
using JomashopNotifications.Persistence.Abstractions;
using JomashopNotifications.Persistence.Common;
using Microsoft.Data.SqlClient;

namespace JomashopNotifications.Persistence.Implementations;

public sealed class ApplicationErrorsSqlDatabase(string connectionString) : IApplicationErrorsDatabase
{
    public Task InsertAsync(string message, string? type)
    {
        using var connection = new SqlConnection(connectionString);

        type = type.NullIfWhiteSpace();

        var @params = new
        {
            message,
            type
        };

        var sql = $"""
                   INSERT INTO dbo.{DatabaseTable.ApplicationErrors} (Message, Type)
                   VALUES (@message, @type);
                   """;

        return connection.ExecuteAsync(sql, @params);
    }
}
