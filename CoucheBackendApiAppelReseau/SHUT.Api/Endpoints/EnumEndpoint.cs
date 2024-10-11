// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Threading.Tasks;
// using SHUT.Core.Application;

// namespace SHUT.Api.Endpoints
// {
//     public static class EnumEndpoint
//     {
//         public static RouteGroupBuilder MapEnumsApi(this RouteGroupBuilder group)
//         {
//             group.MapGet("/unites", GetUniteValues);
//             group.MapGet("/priorites", GetPrioriteValues);
//             group.MapGet("/typeVoieTOR", GetTypeVoieTORValues);
//             group.MapGet("/profils", GetTypeVoieTORValues);
//             return group;
//         }

//         public static IResult GetUniteValues(EnumService enumService)
//         {
//             return Results.Ok(enumService.GetUniteValues());
//         }

//         public static IResult GetPrioriteValues(EnumService enumService)
//         {
//             return Results.Ok(enumService.GetPrioriteValues());
//         }

//         public static IResult GetTypeVoieTORValues(EnumService enumService)
//         {
//             return Results.Ok(enumService.GetTypeVoieTORValues());
//         }

//         public static IResult GetProfilTypeValues(EnumService enumService)
//         {
//             return Results.Ok(enumService.GetProfilTypeValues());
//         }
//     }
// }