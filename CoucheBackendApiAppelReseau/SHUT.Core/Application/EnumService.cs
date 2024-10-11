// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Threading.Tasks;
// using SHUT.Core.Domain.Reseau;
// using SHUT.Core.Domain.Administration;

// namespace SHUT.Core.Application
// {
//     public class EnumService
//     {
//         public IEnumerable<string> GetProfilTypeValues()
//         {
//             return Enum.GetValues(typeof(ProfilType))
//                 .Cast<Unite>()
//                 .Select(v => v.GetStringValue());
//         }
//         public IEnumerable<string> GetUniteValues()
//         {
//             return Enum.GetValues(typeof(Unite))
//                 .Cast<Unite>()
//                 .Select(v => v.GetStringValue());
//         }

//         public IEnumerable<string> GetPrioriteValues()
//         {
//             return Enum.GetValues(typeof(Priorite))
//                 .Cast<Priorite>()
//                 .Select(v => v.GetStringValue());
//         }

//         public IEnumerable<string> GetTypeVoieTORValues()
//         {
//             return Enum.GetValues(typeof(TypeVoieTOR))
//                 .Cast<TypeVoieTOR>()
//                 .Select(v => v.ToString());
//         }
//     }
// }