using Microsoft.AspNetCore.Mvc;
using SHUT.Core.Application;
using SHUT.Core.Domain;

namespace SHUT.Api.Endpoints
{
    public static class UserEndpoint
    {
        public static RouteGroupBuilder MapUsersApi(this RouteGroupBuilder group)
        {
            group.MapGet("/Users", GetUsers);
            group.MapPost("/User", PostUser);
            return group;
        }

        public static async Task<IEnumerable<User>> GetUsers(
            UserService userService
        )
        {
            var users = await userService.GetUsers();
            return users;
        }

        public static async Task<IResult> PostUser(
            UserService userService,
            [FromBody] User user)
        {
            await userService.AddUser(user);
            return TypedResults.Ok();
        }
    }
}
