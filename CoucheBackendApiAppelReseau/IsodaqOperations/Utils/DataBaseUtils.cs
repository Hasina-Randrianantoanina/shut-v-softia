using IsodaqOperations.Entities;
using IsodaqOperations.Repository;
using SHUT.Core.Data;

namespace IsodaqOperations.Utils
{
    public static class DataBaseUtils
    {
        public static AppDbContext AppDbContext;
        public static ArchiveDbContext ArchiveDbContext;
        public static async Task<Station?> GetStation(string initiales)
        {
            try
            {
                await using StationRepository repo = new StationRepository();
                Station? station = await repo.GetOneAsync(initiales);
                return station;
            }
            catch (Exception ex)
            {
                PrintUtils.PrintConsoleException(ex, $"Error when retriving station {initiales}");
                throw; // To break main
            }
        }

        public static async Task<List<VoieInterne>> GetVoiesInternes(int stationId)
        {
            try
            {
                await using VoieInterneRepository repo = new VoieInterneRepository();
                return await repo.GetAllOfAStationAsync(stationId);
            }
            catch (Exception ex)
            {
                PrintUtils.PrintConsoleException(ex, $"Error when retrieving voies_telemesurees with stationId = {stationId}");
                throw; // To break main
            }
        }

        public static async Task<bool> InsertMesures(List<Mesure> Mesures, string initialesStation)
        {
            try
            {
                MesureRepository repo = new MesureRepository();
                return await repo.PersistMesures(Mesures, initialesStation);
            }
            catch (Exception ex)
            {
                PrintUtils.PrintConsoleException(ex, $"Error when inserting Mesures");
                throw; // To break main
            }
        }

        public static async Task<bool> InsertAlerte(double seuil, string niveau, int stationId)
        {
            try
            {
                AlerteRepository repo = new AlerteRepository();
                return await repo.PersistAlertes(seuil, niveau, stationId);
            }
            catch (Exception ex)
            {
                PrintUtils.PrintConsoleException(ex, $"Error when inserting Mesures");
                throw; // To break main
            }
        }

        public static async Task<int> InsertDefautActif(DefautActif defaut, int voieTelemesureeId)
        {
            try
            {
                await using DefautActifRepository repo = new DefautActifRepository();
                return await repo.AddAsync(defaut, voieTelemesureeId);
            }
            catch (Exception ex)
            {
                PrintUtils.PrintConsoleException(ex, $"Error when inserting defaut actif for voie TM id = {voieTelemesureeId}");
                throw; // To break main
                return 0; // Do not break main ?
            }
        }

        public static async Task<int> DeleteDefautActif(int voieTelemesureeId)
        {
            try
            {
                await using DefautActifRepository repo = new DefautActifRepository();
                return await repo.DeleteAsync(voieTelemesureeId);
            }
            catch (Exception ex)
            {
                PrintUtils.PrintConsoleException(ex, $"Error when deleting defaut actif for voie TM id = {voieTelemesureeId}");
                throw; // To break main
                return 0; // Do not break main ?
            }
        }
    }
}
