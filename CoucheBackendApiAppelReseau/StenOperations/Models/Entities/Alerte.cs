using System;
using System.Collections.Generic;
using System.Linq;

namespace StenOperations.Models.Entities
{
    public class Alerte
    {
        public int Id { get; set; }
        public bool Acquitter { get; set; }
        public string? Commentaire { get; set; }
        public DateTime? DateAlerte { get; set; }
        public string? DescriptionAlerte { get; set; }
        public string? ParametreAction { get; set; }
        public string? ParametreAlerte { get; set; }
        public int StationId { get; set; }
        public int Utilisateur_Id { get; set; }
        public int Old_Numero { get; set; }
        public string Debug_Initiales { get; set; }

    }
}
