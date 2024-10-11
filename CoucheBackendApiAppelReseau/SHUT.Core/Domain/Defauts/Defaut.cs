using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SHUT.Core.Domain.Defauts
{
    [Table("defauts", Schema = "defaut")]
    public class Defaut
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("actif")]
        public bool Actif { get; set; }

        [Column("appel")]
        public DateTime? Appel { get; set; }

        [Column("commentaire")]
        [StringLength(250)]
        public string? Commentaire { get; set; }

        [Column("debut")]
        public DateTime? Debut { get; set; }

        [Column("fin")]
        public DateTime? Fin { get; set; }

        [Column("type")]
        [StringLength(50)]
        public string? Type { get; set; }

        [Column("voie_telemesuree_id")]
        public int? VoieTelemesureeId { get; set; }

        [Column("voie_tor_id")]
        public int? VoieTorId { get; set; }

        [Column("utilisateur_id")]
        public int? UtilisateurId { get; set; }
    }
}