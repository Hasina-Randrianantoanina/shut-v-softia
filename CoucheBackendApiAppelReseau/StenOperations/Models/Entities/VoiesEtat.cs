using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StenOperations.Models.Entities
{
    public class VoiesEtat
    {
        public int Id { get; set; }
        public int ETordre { get; set; }
        public int ETnum { get; set; }
        public string? ETmodule { get; set; }
        public int ETetat { get; set; }
        public string? ETlibel { get; set; }
    }
}
