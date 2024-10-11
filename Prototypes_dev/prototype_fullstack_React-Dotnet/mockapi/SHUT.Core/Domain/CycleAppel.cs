namespace SHUT.Core.Domain
{
    public class CycleAppel
    {
        public List<Enregistreur> Enregistreurs { get; set; } = new List<Enregistreur>();
        public List<int> HeuresAppel { get; set; } = new List<int>();
    }
}
