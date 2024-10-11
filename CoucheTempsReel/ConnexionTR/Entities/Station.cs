using System.Text;

namespace ConnexionTR.Entities
{
    public class Station
    {
        public int Id { get; set; }
        public string Initiales { get; set; }
        public Enregistreur EnregistreurBase { get; set; }
        public List<VoieInterne> VoiesInternesBase { get; set; }
        public Enregistreur EnregistreurStation { get; set; } = new Enregistreur();
        public List<VoieInterne> VoiesInternesStation { get; set; }

        public void PrintStation() 
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(new String('-', 20) + " Print station " + new String('-', 20));
            builder.AppendLine($"Id={Id} ");
            builder.AppendLine($"Initiales={Initiales} ");
            if (this.EnregistreurBase.TypeLiaison != null) { builder.AppendLine($"TypeLiaison={EnregistreurBase.TypeLiaison}"); }
            if (this.EnregistreurBase.AdresseIP != null) { builder.AppendLine($"AdresseIP={EnregistreurBase.AdresseIP}"); }
            if (this.EnregistreurBase.Version != null) { builder.AppendLine($"Version={EnregistreurBase.Version}"); }
            builder.AppendLine(new String('-', 55));
            Console.WriteLine(builder.ToString());
        }

        public override string ToString()
        {
            return $"{Initiales} [adresseIP: {EnregistreurBase.AdresseIP}, liaison: {EnregistreurBase.TypeLiaison}]";
        }
    }
}
