
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IsodaqOperations.Entities
{
    public class Alerte
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("acquitter")]
        public bool? Acquitter { get; set; }

        [Column("commentaire")]
        [StringLength(250)]
        public string? Commentaire { get; set; }

        [Column("date_alerte")]
        public DateTime DateAlerte { get; set; }

        [Column("description_alerte")]
        [StringLength(250)]
        public string? DescriptionAlerte { get; set; }

        [Column("parametre_action")]
        [StringLength(250)]
        public string? ParametreAction { get; set; }

        [Column("parametre_alerte")]
        [StringLength(250)]
        public string? ParametreAlerte { get; set; }

        [Column("station_id")]
        public int? StationId { get; set; }

        [Column("type")]
        public int? Type { get; set; }

        [Column("utilisateur_id")]
        public int UtilisateurId { get; set; }
    }
}
