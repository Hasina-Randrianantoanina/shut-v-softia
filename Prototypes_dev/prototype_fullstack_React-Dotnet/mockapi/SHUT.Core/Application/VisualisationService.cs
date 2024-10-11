using InfluxDB.Client;
using SHUT.Core.Data;

namespace SHUT.Core.Application
{
    public class VisualisationService
    {
        private readonly InfluxService _service;

        public VisualisationService(InfluxService service)
        {
            _service = service;
        }
        public async Task<IEnumerable<object>> GetInfluxDAta(DateTime start, DateTime end)
        {
            if (start > end)
            {
                var tmp = end;
                end = start;
                start = tmp;
            }
            var results = await _service.QueryAsync(async query =>
            {
                var flux = $"from(bucket:\"TestShut\") |> range(start: {start:yyyy-MM-dd'T'HH:mm:ss'Z'}, stop: {end:yyyy-MM-dd'T'HH:mm:ss'Z'})";
                var tables = await query.QueryAsync(flux, "S");
                return tables.SelectMany(table =>
                    table.Records.Select(record =>
                        {
                        var instant = record.GetTime();
                        var timestamp = instant.HasValue
                            ? instant.Value.ToDateTimeUtc().ToString("yyyy-MM-ddTHH:mm:ssZ")
                            : null;

                        return new
                        {
                            Timestamp = timestamp,
                            Value = record.GetValueByKey("_value"),
                            Tag = record.GetValueByKey("tag_name")
                        } as object;
                    }));
            });

            var formattedResult = results.ToList();
            return formattedResult;
        }
    }
}
