using Microsoft.AspNetCore.Mvc;
using SHUT.Core.Application.Administration;
using SHUT.Core.Domain.Administration;

namespace SHUT.Api.Endpoints
{
    public static class UserEndpoint
    {
        public static RouteGroupBuilder MapUsersApi(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetUsers).WithName("GetUsers");
            group.MapPost("/", PostUser).WithName("CreateUser").RequireAuthorization("ALL");
            group.MapPut("/{id}", PutUser).WithName("UpdateUser").RequireAuthorization("ALL");
            group.MapDelete("/{id}", DeleteUser).WithName("DeleteUser").RequireAuthorization("ALL");
            return group;
        }

        public static async Task<IResult> GetUsers(UserService userService)
        {
            try
            {
                var users = await userService.GetUsers();
                return Results.Ok(users);
            }
            catch (Exception ex)
            {
                return Results.Json(new { message = "Une erreur s'est produite lors de la récupération des utilisateurs.", details = ex.Message }, statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        public static async Task<IResult> PostUser(
            UserService userService,
            [FromBody] User user)
        {
            if (user == null)
            {
                return Results.BadRequest(new { message = "Les données de l'utilisateur sont invalides." });
            }

            try
            {
                var userId = await userService.AddUser(user);
                user.Id = userId; 
                return Results.Created($"/Users/{userId}", user);
            }
            catch (Exception ex)
            {
                return Results.Json(new { message = "Une erreur s'est produite lors de l'ajout de l'utilisateur.", details = ex.Message }, statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        public static async Task<IResult> PutUser(
            int id,
            UserService userService,
            [FromBody] User user)
        {
            if (user == null || id != user.Id)
            {
                return Results.BadRequest(new { message = "Les données de l'utilisateur sont invalides ou l'ID ne correspond pas." });
            }

            try
            {
                var result = await userService.UpdateUser(user);
                if (result == 0)
                {
                    return Results.NotFound(new { message = $"Aucun utilisateur trouvé avec l'ID {id}." });
                }
                return Results.Ok(new { message = "Utilisateur mis à jour avec succès", user = user });
            }
            catch (Exception ex)
            {
                return Results.Json(new { message = "Une erreur s'est produite lors de la mise à jour de l'utilisateur.", details = ex.Message }, statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        public static async Task<IResult> DeleteUser(
            int id,
            UserService userService)
        {
            try
            {
                var result = await userService.DeleteUser(id);
                if (result == 0)
                {
                    return Results.NotFound(new { message = $"Aucun utilisateur trouvé avec l'ID {id}." });
                }
                return Results.Ok(new { message = "Utilisateur supprimé avec succès" });
            }
            catch (Exception ex)
            {
                return Results.Json(new { message = "Une erreur s'est produite lors de la suppression de l'utilisateur.", details = ex.Message }, statusCode: StatusCodes.Status500InternalServerError);
            }
        }
    }
}