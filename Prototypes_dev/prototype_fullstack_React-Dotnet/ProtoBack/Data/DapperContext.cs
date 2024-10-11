namespace ProtoBack.Data
{
    using System.Data;
    using Microsoft.Extensions.Configuration;
    using Npgsql;

    public class DapperContext
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public DapperContext(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString =
                _configuration.GetConnectionString("dbConnect")
                ?? throw new ArgumentNullException(nameof(_connectionString));
        }

        public IDbConnection CreateConnection()
        {
            return new NpgsqlConnection(_connectionString);
        }
    }
}
