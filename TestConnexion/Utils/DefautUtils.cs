using TestConnexion.Entities;
using TestConnexion.Entities.Infos;
using TestConnexion.Log;

namespace TestConnexion.Utils
{
    public static class DefautUtils
    {
        public static async Task<List<Defaut>> HandleMesureWithDefaut(List<Mesure> mesuresWithDefaut, List<VoieInterne> voiesInternes, FileLogger fileLogger)
        {
            string initiales = voiesInternes[0].InitialesStation;
            List<Defaut> defautsOfThisStation = new List<Defaut>();

            for (int numeroVoie = 1; numeroVoie < 33; numeroVoie++)
            {
                // For this voie, getting all the mesures with DebutDefaut and/or FinDefaut
                List<Mesure> mesuresWithDefautOfThisVoie = new List<Mesure>();
                mesuresWithDefautOfThisVoie.AddRange(mesuresWithDefaut.Where(m => { return m.NumeroVoie == numeroVoie; }));

                if (mesuresWithDefautOfThisVoie.Count == 0) { continue; } // No mesure with defauts since the last call

                // For this voie, looking for an active defaut from the last call
                Defaut defaut = await DataBaseUtils.GetActiveDefautOfThisVoieAsync(initiales, numeroVoie, fileLogger);

                for (int i = 0; i < mesuresWithDefautOfThisVoie.Count; i++)
                {
                    defaut = HandlingOneMesureWithDefaut(mesuresWithDefautOfThisVoie[i], defaut, initiales);
                    if (defaut != null && defaut.FinDefaut != null) { defautsOfThisStation.Add(defaut); } // Adding the defaut if not active
                    if (i == mesuresWithDefautOfThisVoie.Count -1) // Last mesure of this voie
                    {
                        bool active = defaut != null && defaut.FinDefaut == null;
                        voiesInternes[numeroVoie-1].Defaut = active; // True if there is a defaut in the last mesure
                        if (active) { defautsOfThisStation.Add(defaut!); }
                    }
                } // End for loop i
            } // End for loop numeroVoie
            HandleMissingEndOfDefauts(defautsOfThisStation);
            return defautsOfThisStation;
        }

        private static Defaut HandlingOneMesureWithDefaut(Mesure mesure, Defaut defaut, string initiales)
        {
            if (mesure.Info is InfoAnalogicCapteur)
            {
                InfoAnalogicCapteur info = (InfoAnalogicCapteur)mesure.Info;
                if (info.DebutDefaut)
                {
                    defaut = new Defaut { InitialesStation = initiales, NumeroVoie = mesure.NumeroVoie, DebutDefaut = mesure.Time, Active = true };
                }
                if (info.FinDefaut && defaut != null)
                {
                    defaut.FinDefaut = mesure.Time;
                    defaut.Active = false;
                }
            }
            else
            {
                InfoAnalogicCalcul info = (InfoAnalogicCalcul)mesure.Info;
                if (info.DebutDefaut)
                {
                    defaut = new Defaut { InitialesStation = initiales, NumeroVoie = mesure.NumeroVoie, DebutDefaut = mesure.Time, Active = true };
                }
                if (info.FinDefaut && defaut != null)
                {
                    defaut.FinDefaut = mesure.Time;
                    defaut.Active = false;
                }
            }
            return defaut;
        }

        private static void HandleMissingEndOfDefauts(List<Defaut> defautsOfThisStation)
        {
            // Could occur if there are missing mesures with defauts, useful to avoid multiple active defaut in the same voie
            for (int i = 0; i < defautsOfThisStation.Count -1; i++)
            {
                if (defautsOfThisStation[i].Active && defautsOfThisStation[i].NumeroVoie == defautsOfThisStation[i+1].NumeroVoie) 
                {
                    defautsOfThisStation[i].Active = false; 
                }
            }
        }
    }
}
