namespace CoucheLectureMan.Entities
{
    public class Station
    {
        public string Initiales { get; set; } // DB vb6, Table Stations/Stations_resobs, column Initiales
        public int Numero { get; set; } // DB vb6, Table Stations/Stations_resobs, column n_sta
        public string TypeLiaison { get; set; } // DB vb6, Table Stations/Stations_resobs, column Liaison
        public string Version { get; set; } // DB vb6, Table Stations/Stations_resobs, column VersionENrg
        public List<VoieInterne> VoiesInternes { get; set; }
        public List<Mesure> Mesures {  get; set; } = new List<Mesure> ();
    }
}
