using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SHUT.Core.Application;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static SHUT.Core.DTOs;

namespace SHUT.Api.Endpoints
{
    public static class AuthenticationEndpoint
    {
        public static RouteGroupBuilder MapAuthentication(this RouteGroupBuilder group)
        {
            group.MapPost("/User", AuthneticatieUser);
            return group;
        }

        public static async Task<UserCredentials> AuthneticatieUser(
            AuthenticationService authenticationService,
            [FromBody]UserDTO userD
        )
        {
            UserD user = await authenticationService.resolveUserIdentity(userD);
            UserCredentials credentials = await authenticationService.generateToken(user);
            return credentials;
            
        }


    }
}
