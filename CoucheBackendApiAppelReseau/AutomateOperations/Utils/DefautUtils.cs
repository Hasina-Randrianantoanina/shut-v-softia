using AutomateOperations.Entities;
using AutomateOperations.Entities.Infos;

namespace AutomateOperations.Utils
{
    public static class DefautUtils
    {

        public static void SetDefautsActifs(List<VoieTelemesuree> voiesTelemesurees, List<Defaut> defautsActifs)
        {
            foreach (VoieTelemesuree voie in voiesTelemesurees)
            {
                voie.DefautActif = null;
                foreach (Defaut defaut in defautsActifs)
                {
                    if (defaut.VoieTelemesureeId == voie.Id)
                    {
                        voie.DefautActif = defaut;
                        voie.DefautActif.TimeMesure = DateTime.MaxValue; // Force true when filter in DataBaseUtils.InsertAllDefautsOfThisStation()
                        break;
                    }
                }
            }
        }

        public static (List<Defaut>? defautsOfThisStation, List<Defaut>? defautsActifs, Erreur? erreur) CalculateDefautsOfThisStation(List<Mesure> mesuresWithDefaut, List<VoieTelemesuree> voiesTelemesurees, string versionEnregistreur, DateTime appelUTC)
        {
            List<Defaut> defautsOfThisStation = new List<Defaut>();
            List<Defaut> defautsActifs = new List<Defaut>();

            try
            {
                foreach (VoieTelemesuree voie in voiesTelemesurees) // Only voies in database are handled
                {
                    if (voie.Numero > 32) { break; }

                    // For this voie, getting all the mesures with DebutDefaut and/or FinDefaut
                    List<Mesure> mesuresWithDefautOfThisVoie = mesuresWithDefaut.Where(m => (m.NumeroVoie == voie.Numero)).ToList();
                    if (mesuresWithDefautOfThisVoie.Count == 0) // No mesure with defauts since the last call
                    {
                        if (voie.DefautActif != null) { defautsActifs.Add(voie.DefautActif); }
                        continue;
                    } 

                    Defaut? lastDefaut = voie.DefautActif;
                    for (int i = 0; i < mesuresWithDefautOfThisVoie.Count; i++)
                    {
                        if (IsMesureDebutDefaut(mesuresWithDefautOfThisVoie[i]))
                        {
                            if (lastDefaut != null) { AddDefautWithoutEnd(lastDefaut, defautsOfThisStation); } // Special case : DebutDefaut after another DebutDefaut
                            lastDefaut = CreateNewDefaut(mesuresWithDefautOfThisVoie[i], voie, versionEnregistreur, appelUTC);
                            if (i == mesuresWithDefautOfThisVoie.Count -1)  //Special case : last mesure of mesuresWithDefautOfThisVoie
                            { 
                                defautsOfThisStation.Add(lastDefaut);
                                defautsActifs.Add(lastDefaut);
                            }
                            // lastDefaut not null
                        }
                        if (IsMesureFinDefaut(mesuresWithDefautOfThisVoie[i]))
                        {
                            if (lastDefaut != null) { AddDefaut(mesuresWithDefautOfThisVoie[i], lastDefaut, defautsOfThisStation); }
                            else {  AddDefautWithoutStart(mesuresWithDefautOfThisVoie[i], defautsOfThisStation, voie, versionEnregistreur, appelUTC); } // Special case : FinDefaut after FinDefaut
                            lastDefaut = null;
                        }
                    } // End for loop i
                } // End foreach voie

                return (defautsOfThisStation, defautsActifs, null);
            }
            catch (Exception ex)
            {
                return (null, null, ErrorUtils.HandleDefautsHandlingError(900, ex));
            }
        }

        private static bool IsMesureDebutDefaut(Mesure mesure)
        {
            if (mesure.Info is InfoAnalogicCapteur)
            {
                InfoAnalogicCapteur info = (InfoAnalogicCapteur)mesure.Info;
                if (info.DebutDefaut) { return true; }
            }
            else
            {
                InfoAnalogicCalcul info = (InfoAnalogicCalcul)mesure.Info;
                if (info.DebutDefaut) { return true; }
            }
            return false;
        }

        private static bool IsMesureFinDefaut(Mesure mesure)
        {
            if (mesure.Info is InfoAnalogicCapteur)
            {
                InfoAnalogicCapteur info = (InfoAnalogicCapteur)mesure.Info;
                if (info.FinDefaut) { return true; }
            }
            else
            {
                InfoAnalogicCalcul info = (InfoAnalogicCalcul)mesure.Info;
                if (info.FinDefaut) { return true; }
            }
            return false;
        }

        private static void AddDefautWithoutEnd(Defaut defaut, List<Defaut> defautsOfThisStation)
        {
            defaut.Actif = false;
            defautsOfThisStation.Add(defaut);
        }

        private static void AddDefautWithoutStart(Mesure mesure, List<Defaut> defautsOfThisStation, VoieTelemesuree voie, string versionEnregistreur, DateTime appelUTC)
        {
            Defaut defaut = new Defaut
            {
                Actif = false,
                Appel = appelUTC,
                Type = "CAPTEUR",
                DebutDefaut = null,
                FinDefaut = mesure.Time,
                VersionEnregistreur = versionEnregistreur,
                VoieTelemesureeId = voie.Id,
                TimeMesure = mesure.Time,
            };
            defautsOfThisStation.Add(defaut);
        }

        private static Defaut CreateNewDefaut(Mesure mesure, VoieTelemesuree voie, string versionEnregistreur, DateTime appelUTC)
        {
            return new Defaut
            {
                Actif = true,
                Appel = appelUTC,
                Type = "CAPTEUR",
                DebutDefaut = mesure.Time,
                FinDefaut = null,
                VersionEnregistreur = versionEnregistreur,
                VoieTelemesureeId = voie.Id,
                TimeMesure = mesure.Time,
            };
        }

        private static void AddDefaut(Mesure mesure, Defaut lastDefaut, List<Defaut> defautsOfThisStation)
        {
                // Normal case
                lastDefaut.Actif = false;
                lastDefaut.FinDefaut = mesure.Time;
                defautsOfThisStation.Add(lastDefaut);
        }

    }
}
