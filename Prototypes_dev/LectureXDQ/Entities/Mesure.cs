namespace LectureXDQ.Entities
{
    public class Mesure
    {
        public DateTime Time { get; set; }
        public int NumeroVoie { get; set; }
        public float ValeurRelative { get; set; } // ISODAQ => Not NGF
    }
}
