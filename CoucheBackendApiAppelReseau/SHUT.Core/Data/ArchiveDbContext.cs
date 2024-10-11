using Microsoft.Extensions.Options;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SHUT.Core.Data
{
    public class ArchiveDbContext
    {
        //private readonly DBSettingsArchive _dbSettings;
        private readonly string _connectionString;

        public ArchiveDbContext(DataBaseSettingsProvider dbSettingsProvider)
        {
            _connectionString = dbSettingsProvider.GetDatabaseSettings("archive") ?? throw new ArgumentNullException(nameof(_connectionString));

        }

        public IDbConnection CreateConnection()
        {
            return new NpgsqlConnection(_connectionString);

        }
    }
}
