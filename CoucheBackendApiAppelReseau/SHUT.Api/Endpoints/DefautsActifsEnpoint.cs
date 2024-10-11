using Microsoft.AspNetCore.Mvc;
using SHUT.Core.Application.Defaut;

namespace SHUT.Api.Endpoints
{
    public static class DefautsActifsEndpoint
    {
        public static RouteGroupBuilder MapDefautsActifsApi(this RouteGroupBuilder group)
        {
            group.MapGet("/CapteurUsage", GetDefautsActifsCapteurUsage);
            group.MapGet("/CapteurObservation", GetDefautsActifsCapteurObs);
            group.MapGet("/EtatUsage", GetDefautsActifsEtatUsage);
            group.MapGet("/EtatObservation", GetDefautsActifsEtatObs);
            group.MapGet("/PingUsage", GetDefautsActifsPingUsage);
            group.MapGet("/PingObservation", GetDefautsActifsPingObs);
            group.MapGet("/AutreUsage", GetDefautsActifsAutreUsage);
            group.MapGet("/AutreObservation", GetDefautsActifsAutreObs);
            group.MapPut("/{id:int}", UpdateDefautActif);
            return group;
        }

        // Defauts CAPTEUR
        public static async Task<IResult> GetDefautsActifsCapteurUsage(DefautsActifsService defautsActifsService)
        {
            try
            {
                var defautsActifs = await defautsActifsService.GetDefautsActifsCapteurUsage();
                return Results.Ok(defautsActifs);
            }
            catch (Exception ex)
            {
                return Results.Json(new { message = "Une erreur s'est produite lors de la récupération des défauts actifs capteur usage.", details = ex.Message }, statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        public static async Task<IResult> GetDefautsActifsCapteurObs(DefautsActifsService defautsActifsService)
        {
            try
            {
                var defautsActifs = await defautsActifsService.GetDefautsActifsCapteurObs();
                return Results.Ok(defautsActifs);
            }
            catch (Exception ex)
            {
                return Results.Json(new { message = "Une erreur s'est produite lors de la récupération des défauts actifs capteur observation.", details = ex.Message }, statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        public static async Task<IResult> UpdateDefautActif(int id, [FromBody] DefautActifUpdateModel model, DefautsActifsService defautsActifsService)
        {
            try
            {
                if (model == null)
                {
                    return Results.BadRequest(new { message = "Les données de mise à jour sont invalides." });
                }

                var result = await defautsActifsService.UpdateDefautActif(id, model.Commentaire, model.Utilisateur);
                return result ? Results.NoContent() : Results.NotFound(new { message = $"Aucun défaut actif trouvé avec l'ID {id}." });
            }
            catch (Exception ex)
            {
                return Results.Json(new { message = $"Une erreur s'est produite lors de la mise à jour du défaut actif avec l'ID {id}.", details = ex.Message }, statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        // Defauts ETAT
        public static async Task<IResult> GetDefautsActifsEtatUsage(DefautsActifsService defautsActifsService)
        {
            try
            {
                var defautsActifs = await defautsActifsService.GetDefautsActifsEtatUsage();
                return Results.Ok(defautsActifs);
            }
            catch (Exception ex)
            {
                return Results.Json(new { message = "Une erreur s'est produite lors de la récupération des défauts actifs état usage.", details = ex.Message }, statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        public static async Task<IResult> GetDefautsActifsEtatObs(DefautsActifsService defautsActifsService)
        {
            try
            {
                var defautsActifs = await defautsActifsService.GetDefautsActifsEtatObs();
                return Results.Ok(defautsActifs);
            }
            catch (Exception ex)
            {
                return Results.Json(new { message = "Une erreur s'est produite lors de la récupération des défauts actifs état observation.", details = ex.Message }, statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        // Defaut PING
        public static async Task<IResult> GetDefautsActifsPingUsage(DefautsActifsService defautsActifsService)
        {
            try
            {
                var defautsActifs = await defautsActifsService.GetDefautsActifsPingUsage();
                return Results.Ok(defautsActifs);
            }
            catch (Exception ex)
            {
                return Results.Json(new { message = "Une erreur s'est produite lors de la récupération des défauts actifs ping usage.", details = ex.Message }, statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        public static async Task<IResult> GetDefautsActifsPingObs(DefautsActifsService defautsActifsService)
        {
            try
            {
                var defautsActifs = await defautsActifsService.GetDefautsActifsPingObs();
                return Results.Ok(defautsActifs);
            }
            catch (Exception ex)
            {
                return Results.Json(new { message = "Une erreur s'est produite lors de la récupération des défauts actifs ping observation.", details = ex.Message }, statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        // Autres défauts
        public static async Task<IResult> GetDefautsActifsAutreUsage(DefautsActifsService defautsActifsService)
        {
            try
            {
                var defautsActifs = await defautsActifsService.GetDefautsActifsAutreUsage();
                return Results.Ok(defautsActifs);
            }
            catch (Exception ex)
            {
                return Results.Json(new { message = "Une erreur s'est produite lors de la récupération des autres défauts actifs usage.", details = ex.Message }, statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        public static async Task<IResult> GetDefautsActifsAutreObs(DefautsActifsService defautsActifsService)
        {
            try
            {
                var defautsActifs = await defautsActifsService.GetDefautsActifsAutreObs();
                return Results.Ok(defautsActifs);
            }
            catch (Exception ex)
            {
                return Results.Json(new { message = "Une erreur s'est produite lors de la récupération des autres défauts actifs observation.", details = ex.Message }, statusCode: StatusCodes.Status500InternalServerError);
            }
        }
    }

    public class DefautActifUpdateModel
    {
        public string? Commentaire { get; set; }
        public string? Utilisateur { get; set; }
    }
}