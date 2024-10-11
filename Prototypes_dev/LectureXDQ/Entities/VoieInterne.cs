namespace LectureXDQ.Entities
{
    public class VoieInterne
    {
        public int Id { get; set; }
        public int Numero { get; set; }
        public int Delta { get; set; }
        public int Virgule { get; set; }
        public DefautActif? DefautActif { get; set; }
        public override string ToString()
        {
            string s = $"Voie ANA num = {Numero}, delta = {Delta}, virgule = {Virgule}, realDelta = {Delta * Math.Pow(10,Virgule - 4)}, activeDefaut = {DefautActif!.Active}";
            return s;
        }
    }
}
