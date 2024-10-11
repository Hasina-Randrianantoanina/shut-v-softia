using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using InfluxDB.Client;
using SHUT.Core.Application;

namespace SHUT.Api.Endpoints
{
    public static class VisualisationEndpoint
    {
        public static RouteGroupBuilder MapInfluxDataApi(this RouteGroupBuilder group)
        {
            group.MapGet("/influx", GetInfluxData);
            //group.MapGet("/timescale", GetTimeScaleData);
            return group;
        }

        public static async Task<IResult> GetInfluxData(
            [FromQuery(Name = "start")] DateTime start,
            [FromQuery(Name = "end")] DateTime end,
            VisualisationService visuService
        )
        {
            
            var formattedResult = visuService.GetInfluxDAta(start, end);

            return Results.Ok(formattedResult);
        }
    }
}
