using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using InfluxDB.Client;
using SHUT.Core.Application;

namespace SHUT.Api.Endpoints
{
    public static class VisualisationEndpoint
    {
        public static RouteGroupBuilder MapVisualisationApi(this RouteGroupBuilder group)
        {
            group.MapGet("/Stations", GetStationsAVisualiser)
                 .RequireCors("AllowAll");
            
            group.MapGet("/Voies", GetVoiesStation)
                 .RequireCors("AllowAll");
            
            group.MapGet("/Historique", GetHistoricalData)
                 .RequireCors("AllowAll");
            
            group.MapGet("/HistoriqueMultipleVoies", GetHistoricalDataForMultipleVoies)
                 .RequireCors("AllowAll");

            return group;
        }

        public static async Task<IResult> GetStationsAVisualiser(VisualisationService visuService)
        {
            var stations = await visuService.GetStationsAVisualiser();
            return Results.Ok(stations);
        }

        public static async Task<IResult> GetVoiesStation(
            [FromQuery(Name = "station")] string station,
            VisualisationService visuService)
        {
            var stations = await visuService.GetVoiesStation(station);
            return Results.Ok(stations);
        }

        public static async Task<IResult> GetHistoricalData(
            [FromQuery(Name = "station")] string station,
            [FromQuery(Name = "voie")] string voie,
            [FromQuery(Name = "start")] DateTime start,
            [FromQuery(Name = "end")] DateTime end,
            VisualisationService visuService)
        {
            var formattedResult = await visuService.GetHistoricalData(station, voie, start, end);
            return Results.Ok(formattedResult);
        }

        public static async Task<IResult> GetHistoricalDataForMultipleVoies(
            [FromQuery(Name = "station")] string station,
            [FromQuery(Name = "voies")] string [] voies,
            [FromQuery(Name = "start")] DateTime start,
            [FromQuery(Name = "end")] DateTime end,
            VisualisationService visuService)
        {
            // List<string> voiesList = voies.Split(",").ToList();
            var formattedResult = await visuService.GetHistoricalDataForMultipleVoies(station, voies.ToList(), start, end);
            return Results.Ok(formattedResult);
        }

        

        
    }
}
