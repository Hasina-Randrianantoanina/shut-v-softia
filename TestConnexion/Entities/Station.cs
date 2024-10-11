using System.Text;

namespace TestConnexion.Entities
{
    public class Station
    {
        public string Initiales { get; set; } // DB vb6, Table Stations/Stations_resobs, column Initiales
        public string? AdresseIP { get; set; } // DB vb6, Table Stations/Stations_resobs, column AdresseIP
        public DateTime DernierTransfert { get; set; } = DateTime.Parse("01/01/1900 00:00:00"); // DB vb6, Table Stations/Stations_resobs, column DernierTrf
        public string? TypeLiaison { get; set; } // DB vb6, Table Stations/Stations_resobs, column Liaison
        public string? Version { get; set; } // DB vb6, Table Stations/Stations_resobs, column VersionENrg
        public List<VoieInterne> VoiesInternes { get; set; }

        public void PrintStation()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(new String('-', 20) + " Print station " + new String('-', 20));
            builder.AppendLine($"Initiales={Initiales} ");
            if (this.TypeLiaison != null) { builder.AppendLine($"TypeLiaison={TypeLiaison}"); }
            if (this.AdresseIP != null) { builder.AppendLine($"AdresseIP={AdresseIP}"); }
            if (this.Version != null) { builder.AppendLine($"Version={Version}"); }
            builder.AppendLine($"DernierTransfert={DernierTransfert.ToString()}");
            builder.AppendLine(new String('-', 55));
            Console.WriteLine(builder.ToString());
        }

        public override string ToString()
        {
            return $"{Initiales} [adresseIP: {AdresseIP}, liaison: {TypeLiaison}, dernier transfert: {DernierTransfert.ToString()}]";
        }
    }
}
