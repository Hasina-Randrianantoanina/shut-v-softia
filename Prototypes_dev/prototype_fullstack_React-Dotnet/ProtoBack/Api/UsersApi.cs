namespace ProtoBack.Api
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http.HttpResults;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Routing;
    using Microsoft.EntityFrameworkCore;
    using ProtoBack.Data;
    using ProtoBack.Models;
    using ProtoBack.Repositories;

    public static class UsersApi
    {
        public static IEndpointRouteBuilder MapUserApi(this IEndpointRouteBuilder app)
        {
        app.MapGet("/users/ef", GetUsersEF).WithName("GetUsersEF").WithOpenApi();
        app.MapPost("/users/ef", PostUserEF).WithName("PostUserEF").WithOpenApi();
        app.MapPost("/users/login/ef", LoginUserEF).WithName("LoginUserEF").WithOpenApi();

        app.MapGet("/users/dapper", GetUsersDapper).WithName("GetUsersDapper").WithOpenApi();
        app.MapPost("/users/dapper", PostUserDapper).WithName("PostUserDapper").WithOpenApi();
        app.MapPost("/users/login/dapper", LoginUserDapper).WithName("LoginUserDapper").WithOpenApi();

            return app;
        }

        public static async Task<Results<Ok<IEnumerable<User>>, BadRequest>> GetUsersEF(ApplicationDbContext context)
        {
            var users = await context.Users.ToListAsync();
            if (users == null || users.Count == 0)
            {
                return TypedResults.BadRequest();
            }
            return TypedResults.Ok(users.AsEnumerable());
        }

        public static async Task<Results<CreatedAtRoute<User>, BadRequest>> PostUserEF(ApplicationDbContext context, User user)
        {
            context.Users.Add(user);
            await context.SaveChangesAsync();
            return TypedResults.CreatedAtRoute(user, "GetUsersEF", new { id = user.Num });
        }

        public static async Task<Results<Ok<User>, UnauthorizedHttpResult, BadRequest>> LoginUserEF(ApplicationDbContext context, [FromQuery] string nom, [FromQuery] string passe)
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.Nom == nom && u.Passe == passe);
            if (user == null)
            {
                return TypedResults.Unauthorized();
            }

            return TypedResults.Ok(user);
        }

        public static async Task<Results<Ok<IEnumerable<User>>, BadRequest>> GetUsersDapper(UserRepository repository)
        {
            var users = await repository.GetUsersDapper();
            if (users == null || !users.Any())
            {
                return TypedResults.BadRequest();
            }
            return TypedResults.Ok(users);
        }

        public static async Task<Results<CreatedAtRoute<User>, BadRequest>> PostUserDapper(UserRepository repository, User user)
        {
            await repository.AddUserDapper(user);
            return TypedResults.CreatedAtRoute(user, "GetUsersDapper", new { id = user.Num });
        }

        public static async Task<Results<Ok<User>, UnauthorizedHttpResult, BadRequest>> LoginUserDapper(UserRepository repository, [FromQuery] string nom, [FromQuery] string passe)
        {
            var user = await repository.LoginUserDapper(nom, passe);
            if (user == null)
            {
                return TypedResults.Unauthorized();
            }

            return TypedResults.Ok(user);
        }
    }
}
