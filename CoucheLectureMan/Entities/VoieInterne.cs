namespace CoucheLectureMan.Entities
{
    public class VoieInterne
    {
        public string Libelle { get; set; } // DB vb6, Table voies_internes/voies_resobs, column libel
        public int NumeroVoie { get; set; } // DB vb6, Table voies_internes/voies_resobs, column num
        public int Virgule { get; set; } // DB vb6, Table voies_internes/voies_resobs, column virgule

        public override string ToString()
        {
            return $"Libelle: {Libelle}, NumeroVoie: {NumeroVoie}, Virgule: {Virgule}";
        }
    }
}
