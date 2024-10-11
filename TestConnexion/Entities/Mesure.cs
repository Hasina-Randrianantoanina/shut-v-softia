using TestConnexion.Entities.Infos;

namespace TestConnexion.Entities
{
    public class Mesure
    {
        public DateTime Time { get; set; }
        public string NomVoie { get; set; }  // M580 version D16
        public int NumeroVoie { get; set; }
        public float ValeurALEchelle { get; set; }
        public Info Info { get; set; }
    }
}
