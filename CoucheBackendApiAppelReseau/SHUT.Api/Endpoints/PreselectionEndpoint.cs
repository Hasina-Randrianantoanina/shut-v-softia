using Microsoft.AspNetCore.Mvc;
using SHUT.Core.Application.Reseau;
using SHUT.Core.Domain.Reseau;
using Microsoft.Extensions.Logging;

namespace SHUT.Api.Endpoints
{
    public static class PreselectionEndpoint
    {
        public static RouteGroupBuilder MapPreselectionApi(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetPreselections);
            group.MapGet("/{id:int}", GetPreselectionById);
            group.MapPost("/", InsertPreselection);
            return group;
        }

        public static async Task<IResult> GetPreselections(PreselectionService preselectionService, ILoggerFactory loggerFactory)
        {
            try
            {
                var preselections = await preselectionService.GetPreselections();
                return Results.Ok(preselections);
            }
            catch (Exception ex)
            {
                return Results.Problem($"Une erreur s'est produite lors de la récupération des présélections: {ex.Message}");
            }
        }

        public static async Task<IResult> GetPreselectionById(int id, PreselectionService preselectionService, ILoggerFactory loggerFactory)
        {
            try
            {
                var preselection = await preselectionService.GetPreselectionById(id);
                return preselection != null ? Results.Ok(preselection) : Results.NotFound($"Aucune présélection trouvée avec l'ID {id}.");
            }
            catch (Exception ex)
            {
                return Results.Problem($"Une erreur s'est produite lors de la récupération de la présélection avec l'ID {id}: {ex.Message}");
            }
        }

        public static async Task<IResult> InsertPreselection([FromBody] Preselections preselection, PreselectionService preselectionService)
        {
            try
            {
                var newId = await preselectionService.InsertPreselection(preselection);
                return Results.Created($"/preselections/{newId}", new { Id = newId });
            }
            catch (Exception ex)
            {
                return Results.Problem($"Une erreur s'est produite lors de l'insertion de la présélection: {ex.Message}");
            }
        }
    }
}