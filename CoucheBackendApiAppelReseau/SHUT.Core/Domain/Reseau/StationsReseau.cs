using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SHUT.Core.Domain.Reseau
{
    [Table("stations", Schema = "reseau")]
    public class StationsReseau
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("actif")]
        public bool Actif { get; set; }

        [Column("bassin_versant")]
        [StringLength(50)]
        public string? BassinVersant { get; set; }

        [Column("enregistreur_id")]
        public int? EnregistreurId { get; set; }

        [Column("initiales")]
        [StringLength(10)]
        public string Initiales { get; set; } = string.Empty;

        [Column("nom")]
        [StringLength(50)]
        public string? Nom { get; set; }

        [Column("numero")]
        public int? Numero { get; set; }

        [Column("reseau")]
        [StringLength(20)]
        public string? Reseau { get; set; }

        //public Enregistreurs Enregistreur { get; set; } 

        [Column("abonnements")]
        public long? Abonnements { get; set; }

        [Column("preselections")]
        public long? Preselections { get; set; }

    }
}