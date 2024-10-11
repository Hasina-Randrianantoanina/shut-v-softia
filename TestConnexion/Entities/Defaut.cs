namespace TestConnexion.Entities
{
    public class Defaut
    {
        public string InitialesStation { get; set; }
        public int NumeroVoie { get; set; }
        public int Type { get; set; } = 26; // Only defaut voie interne for automates
        public DateTime DebutDefaut {  get; set; }
        public DateTime? FinDefaut { get; set; }
        public bool Active { get; set; }
        public string? Commentaire { get; set; }

    }
}
