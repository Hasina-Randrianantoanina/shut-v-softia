namespace ProtoBack.Models
{
    using System.ComponentModel.DataAnnotations;

    public class User
    {
        [Key]
        public int Num { get; set; }

        public string Nom { get; set; } = string.Empty;
        public string Passe { get; set; } = string.Empty;
        public int Groupe { get; set; }
        public int Droits { get; set; }
        public string Page1 { get; set; } = string.Empty;
    }
}
