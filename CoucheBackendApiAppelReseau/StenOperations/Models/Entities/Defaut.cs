using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StenOperations.Models.Entities
{
    public class Defaut
    {
        public int Id { get; set; }
        public bool Actif { get; set; }
        public DateTime Appel { get; set; }
        public string? Commentaire { get; set; }
        public DateTime Debut { get; set; }
        public int Enregistreur_Id { get; set; }
        public DateTime Fin { get; set; }
        public string? Type { get; set; }
        public int Utilisateur_Id { get; set; }
        public string? Version_Enregistreur { get; set; }
        public int Voie_Telemesuree_Id { get; set; }
        public int Voie_Tor_Id { get; set; }
        public string? Old_Libelle_Voie { get; set; }
        public int Old_Numero { get; set; }
        public bool? Debug_Validation { get; set; }
        public string? Debug_Initiales { get; set; }
    }

    public class DefautActif
    {
        public int Id { get; set; }
        public DateTime Appel { get; set; }
        public string? Commentaire { get; set; }
        public string? Description_defaut { get; set; }
        public string? Etat_des_taches { get; set; }
        public int Station_Id { get; set; }
        public string? Type { get; set; }
        public int Voie_Telemesuree_Id { get; set; }
        public int Voie_Tor_Id { get; set; }
        public int Utilisateur_Id { get; set; }
    }
}
