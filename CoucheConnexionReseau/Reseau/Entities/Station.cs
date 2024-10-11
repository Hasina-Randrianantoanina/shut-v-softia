namespace Reseau.Entities
{
    public class Station
    {
        public string Initiales { get; set; } // DB vb6, Table Stations/Stations_resobs, column Initiales
        public string AdresseIP { get; set; } // DB vb6, Table Stations/Stations_resobs, column AdresseIP
        public string TypeLiaison { get; set; } // DB vb6, Table Stations/Stations_resobs, column Liaison
    }
}
