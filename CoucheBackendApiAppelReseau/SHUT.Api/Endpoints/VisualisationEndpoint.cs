using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using InfluxDB.Client;
using SHUT.Core.Application.Historique;

namespace SHUT.Api.Endpoints
{
    public static class VisualisationEndpoint
    {
        public static RouteGroupBuilder MapVisualisationApi(this RouteGroupBuilder group)
        {
            group.MapGet("/Stations", GetStationsAVisualiser)
                 .RequireCors("AllowAll");

            group.MapGet("/Voies", GetVoiesStations)
                 .RequireCors("AllowAll");

            group.MapGet("/Historique", GetHistoricalData)
                 .RequireCors("AllowAll");

            group.MapGet("/HistoriqueMultipleVoies", GetHistoricalDataForMultipleVoies)
                 .RequireCors("AllowAll");

            return group;
        }

        public static async Task<IResult> GetStationsAVisualiser(VisualisationService visuService)
        {
            try
            {
                var stations = await visuService.GetStationsAVisualiser();
                return Results.Ok(stations);
            }
            catch (Exception ex)
            {
                return Results.Json(new { message = "Une erreur s'est produite lors de la récupération des stations à visualiser.", details = ex.Message }, statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        public static async Task<IResult> GetVoiesStations(
            [FromQuery(Name = "stations")] string[] stations,
            VisualisationService visuService)
        {
            try
            {
                if (stations == null || stations.Length == 0)
                {
                    return Results.BadRequest(new { message = "Le paramètre 'stations' est requis." });
                }

                var voiesResult = new Dictionary<string, List<string>>();
                foreach (var station in stations)
                {
                    var voies = await visuService.GetVoiesStation(station);
                    voiesResult[station] = voies;
                }
                return Results.Ok(voiesResult);
            }
            catch (Exception ex)
            {
                return Results.Json(new { message = $"Une erreur s'est produite lors de la récupération des voies pour les stations.", details = ex.Message }, statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        public static async Task<IResult> GetHistoricalData(
            [FromQuery(Name = "station")] string station,
            [FromQuery(Name = "voie")] string voie,
            [FromQuery(Name = "start")] DateTime start,
            [FromQuery(Name = "end")] DateTime end,
            VisualisationService visuService)
        {
            try
            {
                if (string.IsNullOrEmpty(station) || string.IsNullOrEmpty(voie))
                {
                    return Results.BadRequest(new { message = "Les paramètres 'station' et 'voie' sont requis." });
                }

                var formattedResult = await visuService.GetHistoricalData(station, voie, start, end);
                return Results.Ok(formattedResult);
            }
            catch (Exception ex)
            {
                return Results.Json(new { message = $"Une erreur s'est produite lors de la récupération des données historiques pour la station {station} et la voie {voie}.", details = ex.Message }, statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        public static async Task<IResult> GetHistoricalDataForMultipleVoies(
            [FromQuery(Name = "stations")] string[] stations,
            [FromQuery(Name = "voies")] string[] voies,
            [FromQuery(Name = "start")] DateTime start,
            [FromQuery(Name = "end")] DateTime end,
            VisualisationService visuService)
        {
            try
            {
                if (stations == null || stations.Length == 0 || voies == null || voies.Length == 0)
                {
                    return Results.BadRequest(new { message = "Les paramètres 'stations' et 'voies' sont requis." });
                }

                var stationVoiesPairs = new Dictionary<string, string>();
                foreach (var voie in voies)
                {
                    var parts = voie.Split(':');
                    if (parts.Length == 2)
                    {
                        var station = parts[0];
                        var voieName = parts[1];
                        if (stationVoiesPairs.ContainsKey(station))
                        {
                            stationVoiesPairs[station] += "," + voieName;
                        }
                        else
                        {
                            stationVoiesPairs[station] = voieName;
                        }
                    }
                }

                var formattedResult = await visuService.GetHistoricalDataForMultipleStationsAndVoies(stations, stationVoiesPairs, start, end);
                return Results.Ok(formattedResult);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error in GetHistoricalDataForMultipleVoies: {ex.Message}");
                Console.Error.WriteLine($"Stack trace: {ex.StackTrace}");
                return Results.Json(new { message = $"Une erreur s'est produite lors de la récupération des données historiques pour les stations et les voies spécifiées.", details = ex.ToString() }, statusCode: StatusCodes.Status500InternalServerError);
            }
        }
    }
}