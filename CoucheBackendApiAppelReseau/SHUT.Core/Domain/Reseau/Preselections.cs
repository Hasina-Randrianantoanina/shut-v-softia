using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SHUT.Core.Domain.Reseau
{
    [Table("preselections", Schema = "reseau")]
    public class Preselections
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("code")]
        public int Code { get; set; }

        [Column("preselection")]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;
    }
}