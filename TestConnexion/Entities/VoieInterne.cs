namespace TestConnexion.Entities
{
    public class VoieInterne
    {
        public string InitialesStation {  get; set; }
        public int Numero {  get; set; }
        public int AdresseBES { get; set; }
        public int Info { get; set; }
        public float SeuilBas { get; set; }
        public float SeuilHaut { get; set; }
        public float ValeurDelta { get; set; }
        public int Groupe { get; set; }
        public bool Defaut { get; set; }
    }
}
