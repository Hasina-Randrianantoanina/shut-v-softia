namespace AutomateOperations.Entities
{
    public class VoieTelemesuree
    {
        public int Id { get; set; }
        public int AdresseBES { get; set; }
        public int Groupe { get; set; }
        public int Info { get; set; }
        public string Libelle { get; set; } = string.Empty;
        public int Numero {  get; set; }
        public float SeuilBas { get; set; }
        public float SeuilHaut { get; set; }
        public int StationId { get; set; }
        public float ValeurDelta { get; set; }
        public Defaut DefautActif { get; set; } // Not in db
    }
}
