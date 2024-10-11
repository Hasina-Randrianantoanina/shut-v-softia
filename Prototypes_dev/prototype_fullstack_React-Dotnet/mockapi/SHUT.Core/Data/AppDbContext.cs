using Npgsql;
using System.Data;
using Microsoft.Extensions.Options;

namespace SHUT.Core.Data
{
    public class AppDbContext
    {
        private readonly DBSettings _dbSettings;
        private readonly string _connectionString;

        public AppDbContext(IOptions<DBSettings> dbSettings)
        {
            _dbSettings = dbSettings.Value;
            _connectionString =
                $"Host={_dbSettings.Server}; Database={_dbSettings.Database}; Username={_dbSettings.UserId}; Password={_dbSettings.Password};"
                ?? throw new ArgumentNullException(nameof(_connectionString));
        }

        public IDbConnection CreateConnection()
        {
            return new NpgsqlConnection(_connectionString);

        }
    }
}
