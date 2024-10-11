using TestConnexion.Entities;
using TestConnexion.Log;
using TestConnexion.Repository;

namespace TestConnexion.Utils
{
    public static class DataBaseUtils
    {
        public static async Task<Station> GetStation(string initiales, FileLogger fileLogger, bool localMode)
        {
            try 
            {
                await using StationRepository repo = new StationRepository();
                Station station = await repo.GetOneAsync(initiales);
                if (localMode && station != null) { station.AdresseIP = "127.0.0.1"; }
                return station;
            }
            catch (Exception ex) 
            {
                PrintUtils.PrintConsoleException(ex, $"Error when retriving station {initiales}");
                fileLogger.LogException(ex, "Intéraction avec la bdd (Récupération station)", -20);
                return null;
            }
        }

        public static async Task UpdateStation(Station station, FileLogger fileLogger) 
        {
            try
            {
                await using StationRepository repo = new StationRepository();
                await repo.UpdateAsync(station);
            }
            catch (Exception ex) 
            {
                PrintUtils.PrintConsoleException(ex, $"Error when updating station {station.Initiales}");
                fileLogger.LogException(ex, "Intéraction avec la bdd (Mise à jour station)", -21);
                throw; // To break the try/catch in Program.cs
            }
        }

        public static async Task UpdateAllVoiesInternes(List<VoieInterne> voiesInternes, FileLogger fileLogger)
        {
            if (voiesInternes.Count == 0) { return; }

            try
            {
                await using VoieInterneRepository repo = new VoieInterneRepository();
                foreach (VoieInterne voie in voiesInternes)
                {
                    await repo.UpdateAsync(voie);
                }
            }
            catch (Exception ex)
            {
                PrintUtils.PrintConsoleException(ex, $"Error when updating voies internes of {voiesInternes[0].InitialesStation}");
                fileLogger.LogException(ex, "Intéraction avec la bdd (Mise à jour voies internes)", -22);
                throw; // To break the try/catch in Program.cs
            }
        }

        public static async Task<Defaut> GetActiveDefautOfThisVoieAsync(string initiales, int numeroVoie, FileLogger fileLogger)
        {
            try 
            {
                await using DefautRepository repo = new DefautRepository();
                return await repo.GetActiveDefautOfThisVoieAsync(initiales, numeroVoie);
            }
            catch (Exception ex)
            {
                PrintUtils.PrintConsoleException(ex, $"Error when retriving defaut of voie interne n°{numeroVoie} of station {initiales}");
                fileLogger.LogException(ex, $"Intéraction avec la bdd (Récupération défaut actif voie {numeroVoie})", -23);
                throw; // To break the try/catch in Program.cs
            }
        }

        public static async Task InsertAllDefautsOfThisStation(List<Defaut> defauts, FileLogger fileLogger)
        {
            if (defauts.Count == 0) { return; }
            try
            {
                await using DefautRepository repo = new DefautRepository();
                foreach(Defaut defaut in defauts)
                {
                    await repo.AddAsync(defaut);
                }
            }
            catch (Exception ex) 
            {
                PrintUtils.PrintConsoleException(ex, $"Error when inserting defauts of station {defauts[0].InitialesStation}");
                fileLogger.LogException(ex, $"Intéraction avec la bdd (Insertion des défauts)", -24);
                throw; // To break the try/catch in Program.cs
            }
        }

        // Used for connexion test 08/08/2024
        public static Station GetStationTestVM(string initiales, bool localMode)
        {
            string adresseIP;

            switch (initiales)
            {
                case "EN": // Premium Heure locale
                    adresseIP = "192.168.20.1";
                    break;
                case "XY": // M580 D16 Heure locale
                    adresseIP = "192.168.34.1"; // Same as prod
                    break;
                default: // Impossible case
                    return null;
            }

            return new Station
            {
                Initiales = initiales,
                AdresseIP = adresseIP,
                TypeLiaison = "AP",
                DernierTransfert = DateTime.Parse("07/08/2024 00:00:00"),
            };
        }
    }
}
