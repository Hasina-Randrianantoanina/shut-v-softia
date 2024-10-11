using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SHUT.Core.Domain.Appels
{
    [Table("appels", Schema = "appel")]
    public class Appel
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("date_appel")]
        public DateTime DateAppel { get; set; }

        [Column("station_id")]
        public int StationId { get; set; }

        [Column("statut")]
        [StringLength(50)]
        public string? Statut { get; set; } = string.Empty;
    }
}