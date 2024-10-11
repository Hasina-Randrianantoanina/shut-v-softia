namespace AutomateOperations.Entities
{
    public class Station
    {
        public int Id { get; set; }
        public string? Initiales { get; set; } = string.Empty;
        public Enregistreur Enregistreur { get; set; } = new();
        public List<VoieTelemesuree> VoiesTelemesurees { get; set; } = new();
        public List<VoieTOR> VoiesTOR {  get; set; } = new();

        public override string ToString()
        {
            return $"{Initiales} [adresseIP: {Enregistreur.AdresseIP}, liaison: {Enregistreur.TypeLiaison}, dernier appel: {Enregistreur.DernierAppel.ToString()} UTC, dernier transfert: {Enregistreur.DernierTransfert.ToString()} UTC, dernier enregistrement : {Enregistreur.DernierEnregistrement.ToString()} UTC, typeHeure : {Enregistreur.TypeHeure}]";
        }
    }
}
