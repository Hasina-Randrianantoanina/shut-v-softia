using Microsoft.AspNetCore.Mvc;
using SHUT.Core.Application.Defaut;

namespace SHUT.Api.Endpoints
{
    public static class AlerteEndpoint
    {
        public static RouteGroupBuilder MapAlerteApi(this RouteGroupBuilder group)
        {
            group.MapGet("/Usage", GetAllAlertes);
            group.MapGet("/Observation", GetAllAlertesObs);
            group.MapPut("/{id:int}", UpdateAlerteCommentaire);
            return group;
        }

        public static async Task<IResult> GetAllAlertes(AlerteService alerteService)
        {
            try
            {
                var alertes = await alerteService.GetAllAlertes();
                return Results.Ok(alertes);
            }
            catch (Exception ex)
            {
                return Results.Json(new { message = "Une erreur s'est produite lors de la récupération des alertes d'usage.", details = ex.Message }, statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        public static async Task<IResult> GetAllAlertesObs(AlerteService alerteService)
        {
            try
            {
                var alertes = await alerteService.GetAllAlertesObs();
                return Results.Ok(alertes);
            }
            catch (Exception ex)
            {
                return Results.Json(new { message = "Une erreur s'est produite lors de la récupération des alertes d'observation.", details = ex.Message }, statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        public static async Task<IResult> UpdateAlerteCommentaire(int id, [FromBody] AlerteUpdateModel model, AlerteService alerteService)
        {
            try
            {
                if (model == null)
                {
                    return Results.BadRequest(new { message = "Les données de mise à jour sont invalides." });
                }

                var result = await alerteService.UpdateAlerteCommentaire(id, model.Commentaire, model.UtilisateurId);
                return result ? Results.NoContent() : Results.NotFound(new { message = $"Aucune alerte trouvée avec l'ID {id}." });
            }
            catch (Exception ex)
            {
                return Results.Json(new { message = $"Une erreur s'est produite lors de la mise à jour du commentaire de l'alerte avec l'ID {id}.", details = ex.Message }, statusCode: StatusCodes.Status500InternalServerError);
            }
        }
    }

    public class AlerteUpdateModel
    {
        public string Commentaire { get; set; }
        public int UtilisateurId { get; set; }
    }
}