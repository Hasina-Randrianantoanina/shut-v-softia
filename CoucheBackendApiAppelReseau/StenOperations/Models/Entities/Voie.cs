using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StenOperations.Models.Entities
{
    public class Voie
    {
        public int Id { get; set; }
        public int VoieTelemesureId { get; set; }
        public int VoieTorId { get; set; }
        public int StationId { get; set; }
        public string Libelle { get; set; }
    }
}
