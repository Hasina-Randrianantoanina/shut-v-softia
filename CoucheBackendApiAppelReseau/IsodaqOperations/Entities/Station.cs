using System.Text;

namespace IsodaqOperations.Entities
{
    public class Station
    {
        public int Id { get; set; }
        public string Initiales { get; set; }
        public Enregistreur Enregistreur { get; set; }
        public List<VoieInterne> VoiesInternes { get; set; }

        public void PrintStation()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(new String('-', 20) + " Print station " + new String('-', 20));
            builder.AppendLine($"Id={Id} ");
            builder.AppendLine($"Initiales={Initiales} ");
            if (this.Enregistreur.TypeLiaison != null) { builder.AppendLine($"TypeLiaison={Enregistreur.TypeLiaison}"); }
            if (this.Enregistreur.Version != null) { builder.AppendLine($"Version={Enregistreur.Version}"); }
            builder.AppendLine(new String('-', 55));
            Console.WriteLine(builder.ToString());
        }

        public override string ToString()
        {
            return $"{Initiales} [liaison: {Enregistreur.TypeLiaison}]";
        }
    }
}
