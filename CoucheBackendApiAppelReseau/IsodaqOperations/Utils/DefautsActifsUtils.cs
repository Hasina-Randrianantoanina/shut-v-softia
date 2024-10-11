using IsodaqOperations.Entities;

namespace IsodaqOperations.Utils
{
    public static class DefautsActifsUtils
    {
        public static void AddInactiveDefaut(List<VoieInterne> voiesInternes, DateTime appel)
        {
            foreach (VoieInterne voie in voiesInternes)
            {
                if (voie.DefautActif == null)
                {
                    voie.DefautActif = new DefautActif
                    {
                        Appel = appel,
                        Type = "CAPTEUR",
                        Active = false
                    };
                }
            }
        }

        public static void ReadDefautsActifs(List<VoieInterne> voieInternes, List<Mesure> mesures)
        { 
            foreach (VoieInterne voie in voieInternes) 
            {
                List<Mesure> mesuresOfThisVoie = mesures.Where(m => { return m.NumeroVoie == voie.Numero; }).ToList();

                for (int i = 0; i < mesuresOfThisVoie.Count; i++)
                {
                    if (mesuresOfThisVoie[i].ValeurRelative == 9999) { voie.DefautActif!.Active = true; }
                    else { voie.DefautActif!.Active = false; }
                }
            }
        }

        public async static Task UpdateOrDeleteDefautsActifs(List<VoieInterne> voiesInternes, DateTime appel, bool printConsole)
        {
            foreach (VoieInterne voie in voiesInternes) 
            {
                DefautActif defaut = voie.DefautActif!;
                if (defaut.Appel.CompareTo(appel) == 0)
                {
                    // No defaut in DB
                    if (defaut.Active)
                    {
                        // Defaut after appel => insert
                        int affectedRows = await DataBaseUtils.InsertDefautActif(defaut, voie.Id);
                        if (printConsole) { Console.WriteLine($"Affected rows by insert => {affectedRows}"); }
                    }
                    else
                    {
                        // No defaut after appel => nothing to delete
                    }
                }
                else
                {
                    // Defaut in DB
                    if (!defaut.Active)
                    {
                        // No defaut after appel => delete
                        int affectedRows = await DataBaseUtils.DeleteDefautActif(voie.Id);
                        if (printConsole) { Console.WriteLine($"Affected rows by delete => {affectedRows}"); }
                    }
                    else
                    {
                        // Defaut after appel => no need to update because we keep the old DateTime appel
                    }
                }
            }

        }

    }
}
