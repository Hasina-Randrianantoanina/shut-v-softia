
namespace StenOperations.Models.Commands
{
    public class CommandContext
    {
        public string Operation { get; set; } = "";
        public int Repetitions { get; set; } = 0;
        public bool RazMemoire { get; set; }
        public bool FicBinRecu { get; set; }
        public int JourCourant { get; set; }
        public bool DiffLTR { get; set; } = false;
        public string NomFicMan { get; set; } = "";
        public string NomFicBin { get; set; } = "";
        public DateTime DateDebut { get; set; }
        public DateTime DateFin { get; set; }
        public bool RepetitionsDates { get; set; }
        public bool DefautDateGo { get; set; } = false;
        public string RootDirPath { get; set; } = "";
    }
}
