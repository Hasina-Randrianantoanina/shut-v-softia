using Microsoft.AspNetCore.Mvc;
using SHUT.Core.Application.Reseau;
using SHUT.Core.Domain.Reseau;
using Microsoft.Extensions.Logging;

namespace SHUT.Api.Endpoints
{
    public static class TraitementEndpoint
    {
        public static RouteGroupBuilder MapTraitementApi(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetTraitements);
            group.MapGet("/{id:int}", GetTraitementById);
            group.MapPut("/{id:int}", UpdateTraitement);
            group.MapGet("/voies", GetVoiesTraitementsByStations);
            return group;
        }

        public static async Task<IResult> GetTraitements(TraitementService traitementService, ILoggerFactory loggerFactory)
        {
            try
            {
                var traitements = await traitementService.GetTraitements();
                return Results.Ok(traitements);
            }
            catch (Exception ex)
            {
                return Results.Problem($"Une erreur s'est produite lors de la récupération des traitements: {ex.Message}");
            }
        }

        public static async Task<IResult> GetTraitementById(int id, TraitementService traitementService, ILoggerFactory loggerFactory)
        {
            try
            {
                var traitement = await traitementService.GetTraitementById(id);
                return traitement != null ? Results.Ok(traitement) : Results.NotFound($"Aucun traitement trouvé avec l'ID {id}.");
            }
            catch (Exception ex)
            {
                return Results.Problem($"Une erreur s'est produite lors de la récupération du traitement avec l'ID {id}: {ex.Message}");
            }
        }

        public static async Task<IResult> UpdateTraitement(int id, [FromBody] Traitement traitement, TraitementService traitementService)
        {
            if (id != traitement.Id)
            {
                return Results.BadRequest("L'ID dans l'URL ne correspond pas à l'ID dans les données du traitement.");
            }

            try
            {
                var result = await traitementService.UpdateTraitement(traitement);
                return result ? Results.NoContent() : Results.NotFound($"Aucun traitement trouvé avec l'ID {id}.");
            }
            catch (Exception ex)
            {
                return Results.Problem($"Une erreur s'est produite lors de la mise à jour du traitement avec l'ID {id}: {ex.Message}");
            }
        }

        public static async Task<IResult> GetVoiesTraitementsByStations([FromQuery] string stationInitiales, TraitementService traitementService, ILoggerFactory loggerFactory)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(stationInitiales))
                {
                    return Results.BadRequest("La liste des initiales de stations ne peut pas être vide.");
                }

                var initialesList = stationInitiales.Split(',').Select(i => i.Trim()).ToList();
                var voiesTraitements = await traitementService.GetVoiesTraitementsByStationInitiales(initialesList);
                return Results.Ok(voiesTraitements);
            }
            catch (Exception ex)
            {
                return Results.Problem($"Une erreur s'est produite lors de la récupération des voies avec traitements: {ex.Message}");
            }
        }
    }
}