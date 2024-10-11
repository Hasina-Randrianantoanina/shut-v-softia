using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SHUT.Core.Domain
{
    public class User
    {
        public int Num { get; set; }
        public string Nom { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;//Enum
        public string Statut { get; set; } = string.Empty;
    }
}
