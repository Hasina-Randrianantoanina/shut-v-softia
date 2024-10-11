

using SHUT.Core.Domain.Historique;

namespace StenOperations.Helpers
{
    public class ContextMesure
    {
        public Models.Entities.Station Station { get; set; } = new Models.Entities.Station();
        public DateTime CurrentTime { get; set; }
        public List<HeaderMesure> Headers { get; private set; } = new List<HeaderMesure>();
        public List<EventMesure> Events { get; private set; } = new List<EventMesure>();
    }

    public class EventMesure
    {
        public DateTime Time { get; set; }
        public string TypeEvt { get; set; } = "";
        public List<Mesure> Mesures { get; set; } = new List<Mesure>();
    }

    public class Mesure
    {
        public DateTime Time { get; set; }
        public string CodeVoie { get; set; } = "";
        public string NomVoie { get; set; } = "";
        public int NumeroVoie { get; set; }
        public string ValeurBrute { get; set; } = "";
        public int Virgule { get; set; }
        public string TypeEvt { get; set; } = "";

        /// <summary>
        /// Return the correct value, including the position of the comma
        /// </summary>
        /// <returns></returns>
        public double Valeur
        {
            get { return GetValeur(ValeurBrute); }
        }

        protected virtual double GetValeur(string valeurBrute)
        {
            var valeur = 0.0;
            if (Virgule != 0 && !string.IsNullOrEmpty(valeurBrute))
            {
                // ValeurBrute.Length - (-Virgule): -Virgule car cette valeur Virgule est négative
                var val = valeurBrute.Insert(valeurBrute.Length - (-Virgule), ",");
                double.TryParse(val, out valeur);
            }
            //else if (!string.IsNullOrEmpty(valeurBrute))
            //{
            //    double.TryParse(valeurBrute, out valeur);
            //}
            return valeur;
        }

    }

    public class ZeroCentMesure : Mesure
    {
        public string ZeroBrute { get; set; } = "";
        public string CentBrute { get; set; } = "";

        //protected override double GetValeur(string valeurBrute)
        //{
        //    var valeur = 0.0;
        //    if (Virgule != 0 && !string.IsNullOrEmpty(valeurBrute))
        //    {
        //        // Virgule : valeur récupérée directement depuis la base
        //        var val = valeurBrute.Insert(valeurBrute.Length - Virgule, ",");
        //        double.TryParse(val, out valeur);
        //    }
        //    return valeur;
        //}

        public double ValeurZero
        {
            get { return GetValeur(ZeroBrute); }
        }

        public double ValeurCent
        {
            get { return GetValeur(CentBrute); }
        }
    }

    public class DateMesure : Mesure
    {
        public DateTime Date
        {
            get
            {
                DateTime.TryParse(ValeurBrute, out var date);
                return date;
            }
        }
    }

    public class HeaderMesure
    {
        public string Code { get; set; } = "";
        public string Libelle { get; set; } = "";
        public int Virgule { get; set; } = 0;
    }

}
