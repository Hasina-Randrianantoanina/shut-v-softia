using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SHUT.Core.Domain.Reseau
{
    [Table("enregistreurs", Schema = "reseau")]
    public class Enregistreurs
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("adresse_ip")]
        [StringLength(30)]
        public string? AdresseIp { get; set; }

        [Column("dernier_appel")]
        public DateTime? DernierAppel { get; set; }

        [Column("dernier_transfert")]
        public DateTime? DernierTransfert { get; set; }

        [Column("dernier_enregistrement")]
        public DateTime? DernierEnregistrement { get; set; }

        [Column("liaison")]
        [StringLength(10)]
        public string? Liaison { get; set; }

        [Column("mise_a_jour")]
        public DateTime? DateMaj { get; set; }

        [Column("pourcentage_memoire")]
        public int? PourcentageMemoire { get; set; }

        [Column("type_heure")]
        [StringLength(20)]
        public string? TypeHeure { get; set; }

        [Column("version")]
        [StringLength(50)]
        public string? Version { get; set; }
    }
}