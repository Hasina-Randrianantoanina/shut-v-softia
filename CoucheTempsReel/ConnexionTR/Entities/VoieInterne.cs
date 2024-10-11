namespace ConnexionTR.Entities
{
    public class VoieInterne
    {
        //public string InitialesStation {  get; set; }
        public int Numero { get; set; }
        public int AdresseBES { get; set; }
        public int Info { get; set; }
        public float SeuilBas { get; set; }
        public float SeuilHaut { get; set; }
        public float ValeurDelta { get; set; }
        public int Groupe { get; set; }

        // VisuNet specific
        public int VoieEnregistree { get; set; } // If Info bit 0 => Numero
        public bool Capteur { get; set; } // Info bit 1
        public bool EnregistrementMinuit { get; set; } // Info bit 2
        public bool SensSeuil { get; set; } // Info bit 3
        public bool EnregistrementStandard { get; set; } // Info bit 4

        public override string ToString()
        {
            string s = $"Voie ANA num = {Numero}, adr = {AdresseBES}, info = {Info}, sb = {SeuilBas}, sh = {SeuilHaut}, d = {ValeurDelta},";
            s+= $" gr = {Groupe}, ve = {VoieEnregistree}, cap = {Capteur}, enrgMinuit = {EnregistrementMinuit}, sensSeuil = {SensSeuil}, enrgStd = {EnregistrementStandard}";
            return s;
        }
    }
}
