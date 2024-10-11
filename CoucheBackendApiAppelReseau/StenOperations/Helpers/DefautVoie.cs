using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StenOperations.Helpers
{
    public enum TypeDefaut
    {
        VOIE_INTERNE,
        VOIE_ETAT,
        INCONNU
    }

    public class DefautVoie
    {
        /// <summary>
        /// Retourne ou assigne la date de défaut
        /// </summary>
        public DateTime DateDefaut { get; set; }

        public DateTime? DateDebut { get; set; } = null;
        public DateTime? DateFin { get; set; } = null;

        private string raw = "";
        public string RawData 
        { 
            get { return raw; }
            set
            {
                raw = value;
                ProcessRawData(raw);
            }
        }
        public int NumeroVoie { get; private set; } = 0;
        public bool IsStartDefaut { get; private set; } = false;

        public TypeDefaut TypeDefaut { get; private set; } = TypeDefaut.INCONNU;

        /// <summary>
        /// Drapeau indiquant que le défaut est actif ou non
        /// c'est à dire que DateFin n'est pas définie
        /// </summary>
        public bool Actif 
        {
            get 
            {
                return DateDebut != null && DateDebut != DateTime.MinValue &&
                    (DateFin == null || DateFin == DateTime.MinValue);
            }
        }

        private void ProcessRawData(string rawData)
        {
            if (string.IsNullOrEmpty(rawData) || rawData.Length != 4)
                return;
            var code = rawData.Substring(0, 2);
            var voie = rawData.Substring(2);
            int.TryParse(voie, out var numVoie);
            switch (code)
            {
                case "10":
                    // Disparition defaut voie etat
                    {
                        NumeroVoie = numVoie; // NumVoie correspond voieEtat.ETmodule
                        IsStartDefaut = false;
                        DateFin = DateDefaut;
                        TypeDefaut = TypeDefaut.VOIE_ETAT;
                    }
                    break;
                case "11":
                    // Apparition defaut voie etat
                    {
                        NumeroVoie = numVoie; // NumVoie correspond voieEtat.ETmodule
                        IsStartDefaut = true;
                        DateDebut = DateDefaut;
                        TypeDefaut = TypeDefaut.VOIE_ETAT;
                    }
                    break;
                case "20":
                    // Disparition defaut voie interne
                    {
                        NumeroVoie = numVoie + 1; // NumVoie doit être incrémentée de 1: voie interne
                        IsStartDefaut = false;
                        DateFin = DateDefaut;
                        TypeDefaut = TypeDefaut.VOIE_INTERNE;
                    }
                    break;
                case "21":
                    // Apparition defaut voie interne
                    {
                        NumeroVoie = numVoie + 1; // NumVoie doit être incrémentée de 1: voie interne
                        IsStartDefaut = true;
                        DateDebut = DateDefaut;
                        TypeDefaut = TypeDefaut.VOIE_INTERNE;
                    }
                    break;
                default:
                    break;
            };
        }
    }
}
