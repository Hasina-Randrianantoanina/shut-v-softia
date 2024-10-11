using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StenOperations.Models.Entities
{
    public class VoiesTor
    {
        public int Id { get; set; }
        public int Numero { get; set; }
        public int Ordre { get; set; }
        public string Libelle { get; set; }
        public string TypeTor { get; set; }
    }
}
