using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SHUT.Core.Domain.Reseau
{
    [Table("voies", Schema = "reseau")]
    public class Voies
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("voie_telemesuree_id")]
        public int? VoieTelemesure { get; set; }

        [Column("voie_tor_id")]
        public int? VoieTorId { get; set; }
    }
}