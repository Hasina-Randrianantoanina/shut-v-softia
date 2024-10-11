using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Connexion.Entities
{
    [Keyless]
    [Table("stations")]
    public class Station
    {
        public string Initiales { get; set; } // DB vb6, Table Stations/Stations_resobs, column Initiales
        public string? TypeLiaison { get; set; } // DB vb6, Table Stations/Stations_resobs, column Liaison
        public string? AdresseIP { get; set; } // DB vb6, Table Stations/Stations_resobs, column AdresseIP
        public string? Version { get; set; } // DB vb6, Table Stations/Stations_resobs, column VersionENrg
        public DateTime DernierTransfertFichiersJours { get; set; } // DB vb6, Table Stations/Stations_resobs, column DernierTrf
        [NotMapped]
        public List<VoieInterne> VoiesInternes { get; set; }

        public void PrintStation()
        {
            string s = new String('-', 20) + " Print station " + new String('-', 20);
            s+= $"\nInitiales={Initiales} ";
            if (this.TypeLiaison != null) { s+= $"\nTypeLiaison={TypeLiaison}"; }
            if (this.AdresseIP != null) { s+= $"\nAdresseIP={AdresseIP}"; }
            if (this.Version != null) { s+= $"\nVersion={Version}"; }
            s+= $"\nDernierTransfertFichiersJours={DernierTransfertFichiersJours.ToString()}";
            s+= "\n" + new String('-', 55);
            Console.WriteLine(s);
        }

        public override string ToString()
        {
            return $"{Initiales} [adresseIP: {AdresseIP}, liaison: {TypeLiaison}, dernier transfert: {DernierTransfertFichiersJours.ToString()}]";
        }
    }
}
