using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System.Net.Sockets;

namespace SHUT.Core.Data
{
    public class InfluxService
    {
        private readonly InfluxSettings _settings;
        public InfluxService(IOptions<InfluxSettings> settings)
        {
            _settings = settings.Value;
        }

        public void Write(Action<WriteApi> action)
        {
            
            using var client = new InfluxDBClient(new InfluxDBClientOptions.Builder()
                        .Url(_settings.InfluxUrl)
                        .AuthenticateToken(_settings.Token.ToCharArray())
                        .Org(_settings.Org)
                        .Bucket(_settings.Bucket)
                        .Build());
            using var write = client.GetWriteApi();
            action(write);
        }

        public async Task<T> QueryAsync<T>(Func<QueryApi, Task<T>> action)
        {
            using var client = new InfluxDBClient(new InfluxDBClientOptions.Builder()
                        .Url(_settings.InfluxUrl)
                        .AuthenticateToken(_settings.Token.ToCharArray())
                        .Org(_settings.Org)
                        .Bucket(_settings.Bucket)
                        .Build()); 
            var query = client.GetQueryApi();
            return await action(query);
        }
    }
}
