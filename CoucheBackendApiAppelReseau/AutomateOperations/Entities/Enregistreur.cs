namespace AutomateOperations.Entities
{
    public class Enregistreur
    {
        public int Id { get; set; }
        public string? AdresseIP { get; set; } = string.Empty;
        public DateTime DernierAppel {  get; set; } = DateTime.MinValue;
        public DateTime DernierEnregistrement { get; set; } = DateTime.MinValue;
        public DateTime DernierTransfert { get; set; } = DateTime.MinValue;
        public string? TypeLiaison { get; set; } = string.Empty ;
        public string? Version { get; set; } =  string.Empty ;
        public string TypeHeure {  get; set; } = string.Empty ;

    }
}
