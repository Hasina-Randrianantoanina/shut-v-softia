using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SHUT.Core.Application;
using SHUT.Core.Application.Reseau;
using SHUT.Core.Domain.Reseau;

namespace SHUT.Api.Endpoints
{
    public static class StationEndpoint
    {
        public static RouteGroupBuilder MapStationsApi(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetStations);
            group.MapGet("/{id:int}", GetStationById);
            group.MapPut("/{id:int}", PutStation);
            group.MapDelete("/{id:int}", DeleteStation);
            group.MapGet("/stationsEnrg", GetStationsWithEnregistreur);
            group.MapPost("/AddStationWithEnrg", PostStationWithEnregistreur);
            group.MapPost("/CreateFromModel", CreateStationFromModel);
            group.MapGet("/AvailableNumbers", GetAvailableStationNumbers);
            group.MapGet("/details", GetStationDetails);
            group.MapGet("/detailsObs", GetStationDetailsObs);
            group.MapPost("/AppelerStation", AppelerStations);
            group.MapPut("/{id:int}/abonnements", UpdateAbonnements);
            group.MapPut("/{id:int}/preselections", UpdatePreselections);
            return group;
        }

        public static async Task<IResult> GetStations(StationService stationService)
        {
            try
            {
                var stations = await stationService.GetStations();
                return Results.Ok(stations);
            }
            catch (Exception ex)
            {
                return Results.Json(
                    new
                    {
                        message = "Une erreur est survenue lors de la récupération des stations.",
                        details = ex.Message,
                    },
                    statusCode: StatusCodes.Status500InternalServerError
                );
            }
        }

        public static async Task<IResult> GetStationById(int id, StationService stationService)
        {
            try
            {
                var station = await stationService.GetStationById(id);
                return station != null
                    ? Results.Ok(station)
                    : Results.NotFound(new { message = $"Station avec l'ID {id} non trouvée." });
            }
            catch (Exception ex)
            {
                return Results.Json(
                    new
                    {
                        message = $"Une erreur est survenue lors de la récupération de la station avec l'ID {id}.",
                        details = ex.Message,
                    },
                    statusCode: StatusCodes.Status500InternalServerError
                );
            }
        }

        public static async Task<IResult> PutStation(
            int id,
            [FromBody] StationUpdateRequest request,
            StationService stationService
        )
        {
            try
            {
                if (request == null || request.Station == null || request.Enregistreur == null)
                {
                    return Results.BadRequest(
                        new { message = "Les données de mise à jour sont invalides ou incomplètes" }
                    );
                }

                request.Station.Id = id;
                var result = await stationService.UpdateStation(
                    request.Station,
                    request.Enregistreur
                );
                return Results.Ok(
                    new { message = "Station et enregistreur mis à jour avec succès" }
                );
            }
            catch (KeyNotFoundException)
            {
                return Results.NotFound(new { message = "Station non trouvée" });
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new { message = $"Données invalides : {ex.Message}" });
            }
            catch (InvalidOperationException ex)
            {
                return Results.Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return Results.Json(
                    new
                    {
                        message = "Une erreur interne est survenue lors de la mise à jour de la station.",
                        details = ex.Message,
                    },
                    statusCode: StatusCodes.Status500InternalServerError
                );
            }
        }

        public static async Task<IResult> DeleteStation(int id, StationService stationService)
        {
            try
            {
                var result = await stationService.DeleteStation(id);
                return result
                    ? Results.NoContent()
                    : Results.NotFound(new { message = $"Station avec l'ID {id} non trouvée." });
            }
            catch (Exception ex)
            {
                return Results.Json(
                    new
                    {
                        message = $"Une erreur est survenue lors de la suppression de la station avec l'ID {id}.",
                        details = ex.Message,
                    },
                    statusCode: StatusCodes.Status500InternalServerError
                );
            }
        }

        public static async Task<IResult> GetStationsWithEnregistreur(StationService stationService)
        {
            try
            {
                var stations = await stationService.GetStationsWithEnregistreur();
                return Results.Ok(stations);
            }
            catch (Exception ex)
            {
                return Results.Json(
                    new
                    {
                        message = "Une erreur est survenue lors de la récupération des stations avec enregistreur.",
                        details = ex.Message,
                    },
                    statusCode: StatusCodes.Status500InternalServerError
                );
            }
        }

        public static async Task<IResult> PostStationWithEnregistreur(
            [FromBody] StationWithEnregistreur stationWithEnregistreur,
            StationService stationService
        )
        {
            try
            {
                var (stationId, ipExists) = await stationService.AddStationWithEnregistreur(
                    stationWithEnregistreur
                );
                var result = new { StationId = stationId, IpExists = ipExists };
                return Results.Created($"/stations/{stationId}", result);
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("initiales"))
            {
                return Results.Conflict(
                    new { message = "Une station avec ces initiales existe déjà." }
                );
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("IP"))
            {
                return Results.Conflict(
                    new { message = "Cette adresse IP est déjà utilisée par une autre station." }
                );
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new { message = $"Données invalides : {ex.Message}" });
            }
            catch (Exception ex)
            {
                return Results.Json(
                    new
                    {
                        message = "Une erreur interne est survenue lors de l'ajout de la station avec enregistreur.",
                        details = ex.Message,
                    },
                    statusCode: StatusCodes.Status500InternalServerError
                );
            }
        }

        public static async Task<IResult> CreateStationFromModel(
            [FromBody] CreateStationFromModelRequest request,
            StationService stationService
        )
        {
            try
            {
                var newStationId = await stationService.CreateStationFromModel(
                    request.ModelStationId,
                    request.NewStation
                );
                return Results.Created(
                    $"/stations/{newStationId}",
                    new { StationId = newStationId }
                );
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("modèle"))
            {
                return Results.NotFound(
                    new { message = "Le modèle de station spécifié n'a pas été trouvé." }
                );
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("initiales"))
            {
                return Results.Conflict(
                    new { message = "Une station avec ces initiales existe déjà." }
                );
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new { message = $"Données invalides : {ex.Message}" });
            }
            catch (Exception ex)
            {
                return Results.Json(
                    new
                    {
                        message = "Une erreur interne est survenue lors de la création de la station à partir du modèle.",
                        details = ex.Message,
                    },
                    statusCode: StatusCodes.Status500InternalServerError
                );
            }
        }

        public static async Task<IResult> GetAvailableStationNumbers(StationService stationService)
        {
            try
            {
                var availableNumbers = await stationService.GetAvailableStationNumbers();
                return Results.Ok(availableNumbers);
            }
            catch (Exception ex)
            {
                return Results.Json(
                    new
                    {
                        message = "Une erreur est survenue lors de la récupération des numéros de station disponibles.",
                        details = ex.Message,
                    },
                    statusCode: StatusCodes.Status500InternalServerError
                );
            }
        }

        public static async Task<IResult> GetStationDetails(StationService stationService)
        {
            try
            {
                var stationDetails = await stationService.GetStationDetails();
                return Results.Ok(stationDetails);
            }
            catch (Exception ex)
            {
                return Results.Json(
                    new
                    {
                        message = "Une erreur est survenue lors de la récupération des détails des stations du réseau d'usage",
                        details = ex.Message,
                    },
                    statusCode: StatusCodes.Status500InternalServerError
                );
            }
        }

        public static async Task<IResult> GetStationDetailsObs(StationService stationService)
        {
            try
            {
                var stationDetails = await stationService.GetStationDetailsObs();
                return Results.Ok(stationDetails);
            }
            catch (Exception ex)
            {
                return Results.Json(
                    new
                    {
                        message = "Une erreur est survenue lors de la récupération des détails des stations du réseau d'observation.",
                        details = ex.Message,
                    },
                    statusCode: StatusCodes.Status500InternalServerError
                );
            }
        }

        public static async Task<IResult> AppelerStations(
            OrchestationReseauService orchestrationService,
            StationService stationService,
            [FromBody] List<int> ids
        )
        {
            if (ids == null || !ids.Any())
            {
                return Results.BadRequest(new { message = "Aucun ID de station fourni." });
            }

            var stations = await GetValidStations(stationService, ids);
            if (!stations.Any())
            {
                return Results.NotFound(new { message = "Aucune station valide trouvée." });
            }

            var (success, message) = await orchestrationService.AppelStationsUser(stations);
            if (success)
            {
                return Results.Ok(new { message });
            }
            else
            {
                return Results.BadRequest(new { message });
            }
        }

        private static async Task<List<StationsReseau>> GetValidStations(
            StationService stationService,
            List<int> ids
        )
        {
            var stations = new List<StationsReseau>();
            foreach (int id in ids)
            {
                var station = await stationService.GetStationById(id);
                if (station != null)
                {
                    stations.Add(station);
                }
            }
            return stations;
        }

        public static async Task<IResult> UpdateAbonnements(
            int id,
            [FromBody] int abonnements,
            StationService stationService
        )
        {
            try
            {
                var result = await stationService.UpdateStationAbonnements(id, abonnements);
                if (!result)
                {
                    return Results.BadRequest(
                        new { message = "Échec de la mise à jour des abonnements." }
                    );
                }

                return Results.Ok(new { message = "Abonnements mis à jour avec succès." });
            }
            catch (KeyNotFoundException)
            {
                return Results.NotFound(new { message = $"Station avec l'ID {id} non trouvée." });
            }
            catch (Exception ex)
            {
                return Results.Json(
                    new
                    {
                        message = "Une erreur est survenue lors de la mise à jour des abonnements.",
                        details = ex.Message,
                    },
                    statusCode: StatusCodes.Status500InternalServerError
                );
            }
        }

        public static async Task<IResult> UpdatePreselections(
            int id,
            [FromBody] long preselections,
            StationService stationService
        )
        {
            try
            {
                var result = await stationService.UpdateStationPreselections(id, preselections);
                if (!result)
                {
                    return Results.BadRequest(
                        new { message = "Échec de la mise à jour des présélections." }
                    );
                }

                return Results.Ok(new { message = "Présélections mises à jour avec succès." });
            }
            catch (KeyNotFoundException)
            {
                return Results.NotFound(new { message = $"Station avec l'ID {id} non trouvée." });
            }
            catch (Exception ex)
            {
                return Results.Json(
                    new
                    {
                        message = "Une erreur est survenue lors de la mise à jour des présélections.",
                        details = ex.Message,
                    },
                    statusCode: StatusCodes.Status500InternalServerError
                );
            }
        }
    }

    public class StationUpdateRequest
    {
        public StationsReseau? Station { get; set; }
        public Enregistreurs? Enregistreur { get; set; }
    }

    public class CreateStationFromModelRequest
    {
        public int ModelStationId { get; set; }
        public StationWithEnregistreur? NewStation { get; set; }
    }
}
