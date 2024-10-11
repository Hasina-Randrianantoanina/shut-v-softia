
using InfluxDB.Client.Api.Domain;
using Microsoft.Extensions.Options;


namespace SHUT.Core.Data
{
    public class DataBaseSettingsProvider
    {
        private readonly ConnectionStringOptions _options;

        public DataBaseSettingsProvider(IOptions<ConnectionStringOptions> options)
        {
            _options = options.Value;
        }


        public string? GetDatabaseSettings(string name) =>
            name switch
            {
                "shut" => _options.dbConnectShut,
                "archive" => _options.dbConnectShutArchive,
                _ => null,
            };
        


    }
}
