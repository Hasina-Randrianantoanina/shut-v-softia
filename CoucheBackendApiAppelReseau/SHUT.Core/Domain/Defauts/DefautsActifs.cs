using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SHUT.Core.Domain.Defauts
{
    [Table("defauts_actifs", Schema = "defaut")]
    public class DefautsActifs
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("appel")]
        public DateTime? Appel { get; set; }

        [Column("commentaire")]
        [StringLength(250)]
        public string? Commentaire { get; set; }

        [Column("description_defaut")]
        [StringLength(250)]
        public string? DescriptionDefaut { get; set; }

        [Column("station_id")]
        public int? StationId { get; set; }

        [Column("type")]
        [StringLength(100)]
        public string? Type { get; set; } = string.Empty;

        [Column("voie_telemesuree_id")]
        public int? VoieTelemesureeId { get; set; }

        [Column("voie_tor_id")]
        public int? VoieTorId { get; set; }

        [Column("utilisateur_id")]
        public int UtilisateurId { get; set; }
    }
}