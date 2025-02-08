using Dapper;
using Microsoft.Data.SqlClient;

namespace JomashopNotifications.Persistence.Common
{
    public static class SqlDatabaseExtensions
    {
        public static async Task<bool> DeleteFromTableAsync(
            string connectionString,
            string table,
            int id)
        {
            var connection = new SqlConnection(connectionString);

            var @params = new
            {
                id
            };

            var sql = $"""
                   DELETE FROM dbo.{table}
                   WHERE Id = @id
                   """;

            return await connection.ExecuteAsync(sql, @params) > 0;
        }
    }
}
