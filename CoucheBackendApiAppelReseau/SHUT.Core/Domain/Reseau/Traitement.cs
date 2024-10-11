using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SHUT.Core.Domain.Reseau
{
    [Table("traitements", Schema = "reseau")]
    public class Traitement
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("nom")]
        [Required]
        [StringLength(50)]
        public string? Nom { get; set; }

        [Column("parametre1")]
        [StringLength(100)]
        public string? Parametre1 { get; set; }

        [Column("parametre2")]
        [StringLength(100)]
        public string? Parametre2 { get; set; }

        [Column("parametre3")]
        [StringLength(100)]
        public string? Parametre3 { get; set; }

        [Column("parametre4")]
        [StringLength(100)]
        public string? Parametre4 { get; set; }

        [Column("parametre5")]
        [StringLength(100)]
        public string? Parametre5 { get; set; }

        [Column("parametre6")]
        [StringLength(100)]
        public string? Parametre6 { get; set; }

        [Column("parametre7")]
        [StringLength(100)]
        public string? Parametre7 { get; set; }

        [Column("parametre8")]
        [StringLength(100)]
        public string? Parametre8 { get; set; }

        [Column("parametre9")]
        [StringLength(100)]
        public string? Parametre9 { get; set; }

        [Column("parametre10")]
        [StringLength(100)]
        public string? Parametre10 { get; set; }
    }
}