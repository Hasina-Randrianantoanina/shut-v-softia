using Microsoft.AspNetCore.Mvc;
using SHUT.Core.Application.Reseau;
using SHUT.Core.Domain.Reseau;

namespace SHUT.Api.Endpoints
{
    public static class EnregistreurEndpoint
    {
        public static RouteGroupBuilder MapEnregistreursApi(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetEnregistreurs);
            group.MapGet("/{id:int}", GetEnregistreurById);
            group.MapPost("/", PostEnregistreur);
            group.MapPut("/{id:int}", PutEnregistreur);
            group.MapDelete("/{id:int}", DeleteEnregistreur);
            group.MapGet("/versions/{liaison}", GetEnregistreurVersionsByLiaison);
            group.MapPut("/{id:int}/dernierTransfert", UpdateDernierTransfert);
            return group;
        }

        public static async Task<IResult> GetEnregistreurs(EnregistreurService enregistreurService)
        {
            try
            {
                var enregistreurs = await enregistreurService.GetEnregistreurs();
                return Results.Ok(enregistreurs);
            }
            catch (Exception ex)
            {
                return Results.Json(new { message = "Une erreur s'est produite lors de la récupération des enregistreurs.", details = ex.Message }, statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        public static async Task<IResult> GetEnregistreurById(int id, EnregistreurService enregistreurService)
        {
            try
            {
                var enregistreur = await enregistreurService.GetEnregistreurById(id);
                return enregistreur != null ? Results.Ok(enregistreur) : Results.NotFound(new { message = $"Aucun enregistreur trouvé avec l'ID {id}." });
            }
            catch (Exception ex)
            {
                return Results.Json(new { message = $"Une erreur s'est produite lors de la récupération de l'enregistreur avec l'ID {id}.", details = ex.Message }, statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        public static async Task<IResult> PostEnregistreur([FromBody] Enregistreurs enregistreur, EnregistreurService enregistreurService)
        {
            try
            {
                if (enregistreur == null)
                {
                    return Results.BadRequest(new { message = "Les données de l'enregistreur sont invalides." });
                }

                var id = await enregistreurService.AddEnregistreur(enregistreur);
                return Results.Created($"/enregistreurs/{id}", id);
            }
            catch (Exception ex)
            {
                return Results.Json(new { message = "Une erreur s'est produite lors de l'ajout de l'enregistreur.", details = ex.Message }, statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        public static async Task<IResult> PutEnregistreur(int id, [FromBody] Enregistreurs enregistreur, EnregistreurService enregistreurService)
        {
            try
            {
                if (enregistreur == null)
                {
                    return Results.BadRequest(new { message = "Les données de l'enregistreur sont invalides." });
                }

                enregistreur.Id = id;
                var result = await enregistreurService.UpdateEnregistreur(enregistreur);
                return result ? Results.NoContent() : Results.NotFound(new { message = $"Aucun enregistreur trouvé avec l'ID {id}." });
            }
            catch (Exception ex)
            {
                return Results.Json(new { message = $"Une erreur s'est produite lors de la mise à jour de l'enregistreur avec l'ID {id}.", details = ex.Message }, statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        public static async Task<IResult> DeleteEnregistreur(int id, EnregistreurService enregistreurService)
        {
            try
            {
                var result = await enregistreurService.DeleteEnregistreur(id);
                return result ? Results.NoContent() : Results.NotFound(new { message = $"Aucun enregistreur trouvé avec l'ID {id}." });
            }
            catch (Exception ex)
            {
                return Results.Json(new { message = $"Une erreur s'est produite lors de la suppression de l'enregistreur avec l'ID {id}.", details = ex.Message }, statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        public static async Task<IResult> GetEnregistreurVersionsByLiaison(string liaison, EnregistreurService enregistreurService)
        {
            try
            {
                if (string.IsNullOrEmpty(liaison))
                {
                    return Results.BadRequest(new { message = "Le paramètre 'liaison' est requis." });
                }

                var versions = await enregistreurService.GetEnregistreurVersionsByLiaison(liaison);
                return Results.Ok(versions);
            }
            catch (Exception ex)
            {
                return Results.Json(new { message = $"Une erreur s'est produite lors de la récupération des versions d'enregistreur pour la liaison '{liaison}'.", details = ex.Message }, statusCode: StatusCodes.Status500InternalServerError);
            }
        }
public static async Task<IResult> UpdateDernierTransfert(int id, [FromBody] string dernierTransfert, EnregistreurService enregistreurService)
{
    try
    {
        if (DateTime.TryParse(dernierTransfert, out DateTime parsedDate))
        {
            var result = await enregistreurService.UpdateDernierTransfert(id, parsedDate);
            return result ? Results.NoContent() : Results.NotFound($"Aucun enregistreur trouvé avec l'ID {id}.");
        }
        else
        {
            return Results.BadRequest("Format de date invalide");
        }
    }
    catch (Exception ex)
    {
        return Results.Problem($"Une erreur s'est produite lors de la mise à jour de la date de dernier transfert: {ex.Message}");
    }
}
    }
}