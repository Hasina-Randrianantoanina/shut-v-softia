using Microsoft.AspNetCore.Mvc;
using SHUT.Core.Application.Defaut;
using SHUT.Core.Domain.Defauts;
using Microsoft.Extensions.Logging;

namespace SHUT.Api.Endpoints
{
    public static class PerteEndpoint
    {
        public static RouteGroupBuilder MapPerteApi(this RouteGroupBuilder group)
        {
            group.MapGet("/{stationId:int}", GetPertesByStation);
            group.MapPut("/{id:int}", UpdatePerte);
            group.MapPost("/", InsertPerte);
            group.MapDelete("/{id:int}", DeletePerte);
            return group;
        }

        public static async Task<IResult> GetPertesByStation(int stationId, PerteService perteService, ILoggerFactory loggerFactory)
        {
            var logger = loggerFactory.CreateLogger("AuthenticationEndpoint");
            try
            {
                var (pertes, enregistreurInfo) = await perteService.GetPertesByStation(stationId);

                var response = new { Pertes = pertes, EnregistreurInfo = enregistreurInfo };

                return Results.Ok(response);
            }
            catch (Exception ex)
            {
                return Results.Problem($"Une erreur s'est produite lors de la récupération des pertes pour la station {stationId}: {ex.Message}");
            }
        }

        public static async Task<IResult> UpdatePerte(int id, [FromBody] Pertes perte, PerteService perteService)
        {
            if (id != perte.Id)
            {
                return Results.BadRequest("L'ID dans l'URL ne correspond pas à l'ID dans les données de la perte.");
            }

            try
            {
                var result = await perteService.UpdatePerte(perte);
                return result ? Results.NoContent() : Results.NotFound($"Aucune perte trouvée avec l'ID {id}.");
            }
            catch (Exception ex)
            {
                return Results.Problem($"Une erreur s'est produite lors de la mise à jour de la perte avec l'ID {id}: {ex.Message}");
            }
        }

        public static async Task<IResult> InsertPerte([FromBody] Pertes perte, PerteService perteService)
        {
            try
            {
                var newId = await perteService.InsertPerte(perte);
                return Results.Created($"/api/perte/{newId}", new { Id = newId });
            }
            catch (Exception ex)
            {
                return Results.Problem($"Une erreur s'est produite lors de l'insertion de la nouvelle perte: {ex.Message}");
            }
        }

        public static async Task<IResult> DeletePerte(int id, PerteService perteService)
        {
            try
            {
                var result = await perteService.DeletePerte(id);
                return result ? Results.NoContent() : Results.NotFound($"Aucune perte trouvée avec l'ID {id}.");
            }
            catch (Exception ex)
            {
                return Results.Problem($"Une erreur s'est produite lors de la suppression de la perte avec l'ID {id}: {ex.Message}");
            }
        }
    }
}