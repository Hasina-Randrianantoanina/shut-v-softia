namespace AutomateOperations.Entities
{
    public class Defaut
    {
        public int Id { get; set; }
        public bool Actif { get; set; }
        public DateTime Appel { get; set; }
        public string Type { get; set; }
        public DateTime? DebutDefaut {  get; set; }
        public DateTime? FinDefaut { get; set; }
        public string VersionEnregistreur { get; set; }
        public int VoieTelemesureeId { get; set; }
        public DateTime TimeMesure { get; set; } // Used by filter in DataBaseUtils.InsertAllDefautsOfThisStation => to avoid inserting defauts already present in db
    }
}
