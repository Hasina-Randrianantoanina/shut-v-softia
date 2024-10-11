namespace AutomateOperations.Utils
{
    public class Erreur
    {
        public DateTime Appel { get; set; } = DateTime.MinValue;
        public int Code { get; set; }
        public string Description { get; set; } = string.Empty;
        public int StationId { get; set; } 
        public string Type { get; set; }

        public Erreur (int code, string type)
        {
            Code = code;
            Type = type;
        }
    }
}
