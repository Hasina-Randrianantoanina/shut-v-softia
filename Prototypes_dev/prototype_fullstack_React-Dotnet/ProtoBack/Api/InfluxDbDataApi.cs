namespace ProtoBack.Api
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using InfluxDB.Client;
    using Microsoft.AspNetCore.Http.HttpResults;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Routing;

    public static class InfluxDbDataApi
    {
        public static IEndpointRouteBuilder MapInfluxDBApi(this IEndpointRouteBuilder app)
        {
            app.MapGet("/data", GetData).WithName("GetData").WithOpenApi();

            return app;
        }

        public static async Task<Results<Ok<IEnumerable<object>>, BadRequest<string>>> GetData(
            [FromQuery(Name = "start")] DateTime start,
            [FromQuery(Name = "end")] DateTime end,
            InfluxDBClient influxDBClient
        )
        {
            if (start > end)
            {
                var tmp = end;
                end = start;
                start = tmp;
            }

            var fluxQuery =
                $"from(bucket:\"TestShut\") |> range(start: {start:yyyy-MM-dd'T'HH:mm:ss'Z'}, stop: {end:yyyy-MM-dd'T'HH:mm:ss'Z'})";
            var tables = await influxDBClient.GetQueryApi().QueryAsync(fluxQuery, "S");

            var formattedResult = tables
                .SelectMany(table =>
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
                    })
                )
                .ToList();

            return TypedResults.Ok(formattedResult.AsEnumerable());
        }
    }
}
