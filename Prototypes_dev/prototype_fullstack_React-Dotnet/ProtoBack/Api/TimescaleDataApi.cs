using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProtoBack.Data;
using ProtoBack.Models.Timescale;

namespace ProtoBack.Api
{
    public static class TimescaleDataApi
    {
        public static IEndpointRouteBuilder MapTimescaleApi(this IEndpointRouteBuilder app)
        {
            var api = app.MapGroup("api");

            // Query data
            api.MapGet("/items", GetAllData)
                .WithName("GetItems")
                .WithOpenApi();
            api.MapGet("/anas", GetAnasData)
                .WithName("GetAnas")
                .WithOpenApi();
            api.MapGet("/tors", GetTorsData)
                .WithName("GetTors")
                .WithOpenApi();
            api.MapGet("/tors2", GetTorsData2)
                .WithName("GetTors2")
                .WithOpenApi();

            return app;
        }

        public static async Task<Results<Ok<TsAna[]>, BadRequest<string>>> GetAllData(TimescaleContext context)
        {
            var items = await context.TsAnas.ToArrayAsync();
            return TypedResults.Ok(items);
        }

        public static async Task<Results<Ok<TsAna[]>, BadRequest<string>>> GetAnasData(
            [FromQuery(Name = "libelles")] string libelles,
            [FromQuery(Name = "start")] DateTime start,
            [FromQuery(Name = "end")] DateTime end,
            TimescaleContext context)
        {
            if (start > end)
            {
                var tmp = end;
                end = start;
                start = tmp;
            }

            TsAna[] items = null;
            if (string.IsNullOrEmpty(libelles) || string.IsNullOrWhiteSpace(libelles))
            {
                items = await context.TsAnas
                    .Where(ana => ana.DateTime >= start.ToUniversalTime() && ana.DateTime < end.ToUniversalTime())
                    .ToArrayAsync();
            }
            else
            {
                var arrLibelles = libelles.Split(new char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                items = await context.TsAnas
                    .Where(ana => arrLibelles.Contains(ana.Libelle) && ana.DateTime >= start.ToUniversalTime() && ana.DateTime < end.ToUniversalTime())
                    .ToArrayAsync();
            }

            //var sql = "SELECT * FROM ts_ana WHERE date_time >= {0} AND date_time < {1} ORDER BY date_time;";
            //var items = await context.TsAnas.FromSqlRaw(sql, start.ToUniversalTime(), end.ToUniversalTime()).ToArrayAsync();
            return TypedResults.Ok(items);
        }

        public static async Task<Results<Ok<TsTor[]>, BadRequest<string>>> GetTorsData(
            [FromQuery(Name = "libelles")] string libelles,
            [FromQuery(Name = "start")] DateTime start,
            [FromQuery(Name = "end")] DateTime end,
            TimescaleContext context)
        {
            if (start > end)
            {
                var tmp = end;
                end = start;
                start = tmp;
            }
            TsTor[] items = null;
            if (string.IsNullOrEmpty(libelles) || string.IsNullOrWhiteSpace(libelles))
            {
                var sql = "SELECT * FROM ts_tor WHERE date_time >= {0} AND date_time < {1} ORDER BY date_time;";
                items = await context.TsTors.FromSqlRaw(sql, start.ToUniversalTime(), end.ToUniversalTime()).ToArrayAsync();
            }
            else
            {
                var allLibelles = libelles.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                var formattedLibelles = string.Format("({0})", string.Join(',', allLibelles.Select(l => string.Format($"'{l}'"))));
                var formattedStart = string.Format($"TO_TIMESTAMP('{start.ToString("yyyy-MM-dd HH:mm:ss")}', 'YYYY-MM-DD HH24:MI:SS')");
                var formattedEnd = string.Format($"TO_TIMESTAMP('{end.ToString("yyyy-MM-dd HH:mm:ss")}', 'YYYY-MM-DD HH24:MI:SS')");
                var sql = string.Format("SELECT * FROM ts_tor WHERE libelle IN {0} AND date_time >= {1} AND date_time < {2} ORDER BY date_time;",
                    formattedLibelles, formattedStart, formattedEnd);
                items = await context.TsTors.FromSqlRaw(sql).ToArrayAsync();
            }
            //var sql = $"SELECT * FROM ts_tor WHERE date_time > '{start.ToString("yyyy-MM-dd HH:mm:ss")}' AND " + 
            //    $"date_time < TO_TIMESTAMP('{start.ToString("yyyy-MM-dd HH:mm:ss")}', 'YYYY-MM-DD HH24:MI:SS') + INTERVAL '{hourInterval} hour{(hourInterval == 1 ? "" : "s")}' ORDER BY date_time;";
            //var items = await context.TsTors.FromSqlRaw(sql).ToArrayAsync();
            return TypedResults.Ok(items);
        }

        public static async Task<Results<Ok<TsTor[]>, BadRequest<string>>> GetTorsData2(
            [FromQuery(Name = "start")] DateTime start,
            [FromQuery(Name = "nbmonth")] int month,
            TimescaleContext context)
        {
            //var sql = "SELECT * FROM ts_tor WHERE date_time >= {0} AND date_time < {1} ORDER BY date_time;";
            //var items = await context.TsTors.FromSqlRaw(sql, start.ToUniversalTime(), end.ToUniversalTime()).ToArrayAsync();
            var sql = $"SELECT * FROM ts_tor WHERE date_time > '{start.ToString("yyyy-MM-dd HH:mm:ss")}' AND " +
                $"date_time < TO_TIMESTAMP('{start.ToString("yyyy-MM-dd HH:mm:ss")}', 'YYYY-MM-DD HH24:MI:SS') + INTERVAL '{month} month{(month == 1 ? "" : "s")}' ORDER BY date_time;";
            var items = context.TsTors.FromSqlRaw(sql).ToArray();
            return TypedResults.Ok(items);
        }
    }
}
