using Microsoft.AspNetCore.Mvc;
using SHUT.Core.Application.Administration;
using static SHUT.Core.DTOs;
using Microsoft.Extensions.Logging;

namespace SHUT.Api.Endpoints
{
    public static class AuthenticationEndpoint
    {
        public static RouteGroupBuilder MapAuthentication(this RouteGroupBuilder group)
        {
            group.MapPost("/User", AuthenticateUser);
            return group;
        }

        public static async Task<IResult> AuthenticateUser(
            AuthenticationService authenticationService,
            [FromBody] UserDTO userD,
            ILoggerFactory loggerFactory)
        {
            try
            {
                var user = await authenticationService.ResolveUserIdentity(userD.Mail, userD.Password);
                if (user == null)
                {
                    return Results.Unauthorized();
                }

                UserCredentials credentials = await authenticationService.GenerateToken(user);
                return Results.Ok(new
                {
                    user = new
                    {
                        id = user.Id,
                        email = user.Email,
                        nom = user.Nom,
                        profilId = user.ProfilId,
                        profilName = user.ProfilName,
                        actif = user.Actif
                    },
                    token = credentials.Token
                });
            }
            catch (Exception ex)
            {
                return Results.Problem("Une erreur s'est produite lors de l'authentification");
            }
        }
    }
}