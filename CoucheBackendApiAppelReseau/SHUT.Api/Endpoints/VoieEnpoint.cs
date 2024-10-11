using Microsoft.AspNetCore.Mvc;
using SHUT.Core.Application.Reseau;
using SHUT.Core.Domain.Reseau;

namespace SHUT.Api.Endpoints
{
    public static class VoieEndpoint
    {
        public static RouteGroupBuilder MapVoiesApi(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetVoies);
            group.MapGet("/{id:int}", GetVoieById);
            group.MapPost("/telemesuree", PostVoieTelemesuree);
            group.MapPost("/tor", PostVoieTor);
            group.MapPut("/telemesuree/{id:int}", PutVoieTelemesuree);
            group.MapPut("/tor/{id:int}", PutVoieTor);
            group.MapDelete("/{id:int}", DeleteVoie);
            group.MapGet("/ByStationInitiales/{initiales}", GetVoiesByStationInitiales);
            group.MapGet("/ByStationId/{stationId:int}", GetVoiesByStationId);
            group.MapPut("/{id:int}/abonnements", UpdateAbonnementsVoie);
            return group;
        }

        public static async Task<IResult> GetVoies(VoiesService voiesService)
        {
            try
            {
                var (voiesTelemesurees, voiesTor) = await voiesService.GetVoies();
                return Results.Ok(new { VoiesTelemesurees = voiesTelemesurees, VoiesTor = voiesTor });
            }
            catch (Exception ex)
            {
                return Results.Json(new { message = "Une erreur s'est produite lors de la récupération des voies.", details = ex.Message }, statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        public static async Task<IResult> GetVoieById(int id, VoiesService voiesService)
        {
            try
            {
                var (voieTelemesuree, voieTor) = await voiesService.GetVoieById(id);
                if (voieTelemesuree != null)
                    return Results.Ok(voieTelemesuree);
                if (voieTor != null)
                    return Results.Ok(voieTor);
                return Results.NotFound(new { message = $"Aucune voie trouvée avec l'ID {id}." });
            }
            catch (Exception ex)
            {
                return Results.Json(new { message = $"Une erreur s'est produite lors de la récupération de la voie avec l'ID {id}.", details = ex.Message }, statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        public static async Task<IResult> PostVoieTelemesuree([FromBody] VoiesTelemesurees voie, VoiesService voiesService)
        {
            try
            {
                if (voie == null)
                    return Results.BadRequest(new { message = "Les données de la voie télemesurée sont invalides." });

                var id = await voiesService.AddVoieTelemesuree(voie);
                return Results.Created($"/voies/telemesuree/{id}", id);
            }
            catch (Exception ex)
            {
                return Results.Json(new { message = "Une erreur s'est produite lors de l'ajout de la voie télemesurée.", details = ex.Message }, statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        public static async Task<IResult> PostVoieTor([FromBody] VoiesTor voie, VoiesService voiesService)
        {
            try
            {
                if (voie == null)
                    return Results.BadRequest(new { message = "Les données de la voie TOR sont invalides." });

                var id = await voiesService.AddVoieTor(voie);
                return Results.Created($"/voies/tor/{id}", id);
            }
            catch (Exception ex)
            {
                return Results.Json(new { message = "Une erreur s'est produite lors de l'ajout de la voie TOR.", details = ex.Message }, statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        public static async Task<IResult> PutVoieTelemesuree(int id, [FromBody] VoiesTelemesurees voie, VoiesService voiesService)
        {
            try
            {
                if (voie == null)
                    return Results.BadRequest(new { message = "Les données de la voie télemesurée sont invalides." });

                voie.Id = id;
                var result = await voiesService.UpdateVoieTelemesuree(voie);
                return result ? Results.NoContent() : Results.NotFound(new { message = $"Aucune voie télemesurée trouvée avec l'ID {id}." });
            }
            catch (Exception ex)
            {
                return Results.Json(new { message = $"Une erreur s'est produite lors de la mise à jour de la voie télemesurée avec l'ID {id}.", details = ex.Message }, statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        public static async Task<IResult> PutVoieTor(int id, [FromBody] VoiesTor voie, VoiesService voiesService)
        {
            try
            {
                if (voie == null)
                    return Results.BadRequest(new { message = "Les données de la voie TOR sont invalides." });

                voie.Id = id;
                var result = await voiesService.UpdateVoieTor(voie);
                return result ? Results.NoContent() : Results.NotFound(new { message = $"Aucune voie TOR trouvée avec l'ID {id}." });
            }
            catch (Exception ex)
            {
                return Results.Json(new { message = $"Une erreur s'est produite lors de la mise à jour de la voie TOR avec l'ID {id}.", details = ex.Message }, statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        public static async Task<IResult> DeleteVoie(int id, VoiesService voiesService)
        {
            try
            {
                var result = await voiesService.DeleteVoie(id);
                return result ? Results.NoContent() : Results.NotFound(new { message = $"Aucune voie trouvée avec l'ID {id}." });
            }
            catch (Exception ex)
            {
                return Results.Json(new { message = $"Une erreur s'est produite lors de la suppression de la voie avec l'ID {id}.", details = ex.Message }, statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        public static async Task<IResult> GetVoiesByStationInitiales(string initiales, VoiesService voiesService)
        {
            try
            {
                if (string.IsNullOrEmpty(initiales))
                    return Results.BadRequest(new { message = "Les initiales de la station sont requises." });

                var (voiesTelemesurees, voiesTor) = await voiesService.GetVoiesByStationInitiales(initiales);
                return Results.Ok(new { VoiesTelemesurees = voiesTelemesurees, VoiesTor = voiesTor });
            }
            catch (Exception ex)
            {
                return Results.Json(new { message = $"Une erreur s'est produite lors de la récupération des voies pour la station avec les initiales {initiales}.", details = ex.Message }, statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        public static async Task<IResult> GetVoiesByStationId(int stationId, VoiesService voiesService)
        {
            try
            {
                var (voiesTelemesurees, voiesTor) = await voiesService.GetVoiesByStationId(stationId);
                return Results.Ok(new { VoiesTelemesurees = voiesTelemesurees, VoiesTor = voiesTor });
            }
            catch (Exception ex)
            {
                return Results.Json(new { message = $"Une erreur s'est produite lors de la récupération des voies pour la station avec l'ID {stationId}.", details = ex.Message }, statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        public static async Task<IResult> UpdateAbonnementsVoie(int id, [FromBody] int abonnements, VoiesService voiesService)
        {
            try
            {
                var result = await voiesService.UpdateVoieAbonnements(id, abonnements);
                if (!result)
                {
                    return Results.BadRequest(new { message = "Échec de la mise à jour des abonnements de la voie." });
                }

                return Results.Ok(new { message = "Abonnements de la voie mis à jour avec succès." });
            }
            catch (KeyNotFoundException)
            {
                return Results.NotFound(new { message = $"Voie avec l'ID {id} non trouvée." });
            }
            catch (Exception ex)
            {
                return Results.Json(
                    new { message = "Une erreur est survenue lors de la mise à jour des abonnements de la voie.", details = ex.Message },
                    statusCode: StatusCodes.Status500InternalServerError
                );
            }
        }
    }
}