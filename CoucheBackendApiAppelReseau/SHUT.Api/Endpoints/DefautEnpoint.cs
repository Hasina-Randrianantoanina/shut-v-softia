using Microsoft.AspNetCore.Mvc;
using SHUT.Core.Application.Defaut;

namespace SHUT.Api.Endpoints
{
    public static class DefautEndpoint
    {
        public static RouteGroupBuilder MapDefautApi(this RouteGroupBuilder group)
        {
            group.MapGet("/CapteurLast24Hours", GetDefautsCapteurLast24Hours);
            return group;
        }

        public static async Task<IResult> GetDefautsCapteurLast24Hours(DefautService defautService)
        {
            try
            {
                var defauts = await defautService.GetDefautsCapteurLast24Hours();
                return Results.Ok(defauts);
            }
            catch (Exception ex)
            {
                return Results.Json(new { message = "Une erreur s'est produite lors de la récupération des battements des dernières 24 heures.", details = ex.Message }, statusCode: StatusCodes.Status500InternalServerError);
            }
        }
    }
}