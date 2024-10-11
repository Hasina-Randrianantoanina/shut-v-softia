using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SHUT.Core.Domain.Administration
{
    [Table("utilisateurs", Schema = "administration")]
    public class User
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("email")]
        [StringLength(250)]
        public string Email { get; set; } = string.Empty;

        [Column("nom")]
        [StringLength(50)]
        public string Nom { get; set; } = string.Empty;

        [Column("profil_id")]
        public int ProfilId { get; set; }

        [Column("actif")]
        public bool Actif { get; set; }

        [NotMapped]
        public string ProfilName { get; set; } = string.Empty;

        [NotMapped]
        public int ProfilCode { get; set; }
    }

}