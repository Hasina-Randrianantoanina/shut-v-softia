using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SHUT.Core.Domain.Reseau
{
    
    [Table("voies_tor", Schema = "reseau")]
    public class VoiesTor
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("adresse_bes")]
        public int? AdresseBes { get; set; }

        [Column("info")]
        public int? Info { get; set; }

        [Column("libelle")]
        [StringLength(50)]
        public string? Libelle { get; set; }

        [Column("numero")]
        public int? Numero { get; set; }

        [Column("ordre")]
        public int? Ordre { get; set; }

        [Column("station_id")]
        public int StationId { get; set; }

        [Column("type")]
        [StringLength(20)]
        public string? Type { get; set; }

        [Column("actif")]
        public bool? Actif { get; set; }
    }
}