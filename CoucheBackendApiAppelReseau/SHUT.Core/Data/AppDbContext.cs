using Npgsql;
using System.Data;
using Microsoft.Extensions.Options;

namespace SHUT.Core.Data
{
    public class AppDbContext
    {
        private readonly string _connectionString;

        public AppDbContext(DataBaseSettingsProvider dbSettingsProvider)
        {
            _connectionString = dbSettingsProvider.GetDatabaseSettings("shut") ?? throw new ArgumentNullException(nameof(_connectionString));
        }

        public IDbConnection CreateConnection()
        {
            return new NpgsqlConnection(_connectionString);

        }
    }
}
