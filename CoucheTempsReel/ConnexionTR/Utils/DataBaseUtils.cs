using ConnexionTR.Entities;
using ConnexionTR.Log;
using ConnexionTR.Repository;

namespace ConnexionTR.Utils
{
    public static class DataBaseUtils
    {

        public static async Task<Station> GetStation(string initiales, FileLogger fileLogger, bool localMode)
        {
            try
            {
                await using StationRepository repo = new StationRepository();
                Station station = await repo.GetOneAsync(initiales);
                if (localMode && station != null) { station.EnregistreurBase.AdresseIP = "127.0.0.1"; }
                return station;
            }
            catch (Exception ex)
            {
                PrintUtils.PrintConsoleException(ex, $"Error when retriving station {initiales}");
                fileLogger.LogException(ex, "Intéraction avec la bdd (Récupération station)", -100);
                return null;
            }
        }

        public static async Task<List<VoieInterne>> GetVoiesInternes(int stationId, FileLogger fileLogger)
        {
            try
            {
                await using VoieInterneRepository repo = new VoieInterneRepository();
                return await repo.GetAllOfAStationAsync(stationId);
            }
            catch (Exception ex)
            {
                PrintUtils.PrintConsoleException(ex, $"Error when retrieving voies_telemesurres with stationId = {stationId}");
                fileLogger.LogException(ex, "Intéraction avec la bdd (Récupération voies_telemesurees)", -101);
                return null;
            }
        }
    }
}
