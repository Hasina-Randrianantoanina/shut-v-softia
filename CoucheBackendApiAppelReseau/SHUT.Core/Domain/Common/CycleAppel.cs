namespace SHUT.Core.Domain.Common
{
    public class CycleAppel
    {
        public Dictionary<HeureAppel, List<Enregistreur>> StationsAAppeler { get; set; } = new();
    }
    public class HeureAppel
    {
        public int Heure { get; set; }
        public int Minute { get; set; } 
    }
}
