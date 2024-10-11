using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SHUT.Core.Domain.Administration
{
    [Table("profils", Schema = "administration")]
    public class Profil
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("code")]
        public int Code { get; set; }

        [Column("profil")]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;
    }
}