using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SHUT.Core.Domain
{
    public class Station
    {
        public int Id { get; set; }
        public string Initiales { get; set; } = null!;
        public string Nom {get;set;} = null!;
        public Reseau Reseau { get;set;} 
    }
    public enum Reseau
    {
        Usage,
        Observation
    }
}
