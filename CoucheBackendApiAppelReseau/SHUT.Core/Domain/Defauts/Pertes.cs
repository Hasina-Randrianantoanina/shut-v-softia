using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SHUT.Core.Domain.Defauts
{
    [Table("pertes", Schema = "defaut")]
    public class Pertes
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("appel")]
        public DateTime? Appel { get; set; }

        [Column("cause")]
        [StringLength(250)]
        public string? Cause { get; set; }

        [Column("commentaire")]
        [StringLength(250)]
        public string? Commentaire { get; set; }

        [Column("critique")]
        public bool? Critique { get; set; }

        [Column("date_acquisition")]
        public DateTime? DateAcquisition { get; set; }

        [Column("date_enregistrement")]
        public DateTime? DateEnregistrement { get; set; }

        [Column("date_go")]
        public DateTime? DateGo { get; set; }

        [Column("date_stop")]
        public DateTime? DateStop { get; set; }

        [Column("date_init")]
        public DateTime? DateInit { get; set; }

        [Column("debut")]
        public DateTime? Debut { get; set; }

        [Column("defaut")]
        [StringLength(250)]
        public string? Defaut { get; set; }

        [Column("diff_horloge")]
        public double? DiffHorloge { get; set; }

        [Column("duree")]
        public int? Duree { get; set; }

        [Column("etat_des_taches")]
        [StringLength(20)]
        public string? EtatDesTaches { get; set; }

        [Column("fin")]
        public DateTime? Fin { get; set; }

        [Column("horloge")]
        public DateTime? Horloge { get; set; }

        [Column("remede")]
        [StringLength(250)]
        public string? Remede { get; set; }

        [Column("station_id")]
        public int? StationId { get; set; }

        [Column("utilisateur_id")]
        public int? UtilisateurId { get; set; }

        [Column("type")]
        [StringLength(50)]
        public string? Type { get; set; }

        [Column("old_nom_utilisateur")]
        [StringLength(50)]
        public string? OldNomUtilisateur { get; set; }

        [Column("old_numero")]
        public int? OldNumero { get; set; }
    }
}