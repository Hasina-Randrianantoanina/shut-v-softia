
using Dapper;
using IsodaqOperations.Entities;
using IsodaqOperations.Utils;
using System.Data;

namespace IsodaqOperations.Repository
{
    public class MesureRepository
    {
        private readonly IDbConnection _connection = DataBaseUtils.ArchiveDbContext.CreateConnection();

        public async Task<bool> PersistMesures(List<Mesure> mesures, string stationInitiales)
        {
            var schemaExists = await EnsureSchemaExists(stationInitiales);
            if (!schemaExists) { return false; }
            List<string> NomVoies = mesures.Select(x => x.NomVoie).Distinct().ToList();
            bool resultInsert = true;
            foreach (var voie in NomVoies)
            {
                (bool TableExists, DateTime? LatestTimeStamp) resultTable = await EnsureTableExists(stationInitiales, voie);
                if(!resultTable.TableExists) { return false;}
                // construire les intervalles temps des mesures a persister
                // deduire les mesures a persisiter non existantes dans la base
                IEnumerable<Mesure> MesuresVoies = mesures.Where(x => x.NomVoie.Equals(voie) && (resultTable.LatestTimeStamp ?? DateTime.MinValue) < x.Time).OrderBy(x => x.Time);
                string table = $"{stationInitiales.ToLower()}.{voie.ToLower()}";
                var sql = $"INSERT INTO {table} (horodate, evenement, mesure) VALUES (@Time, @Evenement, @ValeurRelative)";
                var rowsAffected = MesuresVoies.Count() > 0 ? await _connection.ExecuteAsync(sql, MesuresVoies) : 1;
                resultInsert = resultInsert && rowsAffected > 0;
            }
            return resultInsert;
        }
        public async Task<(bool, DateTime?)> EnsureTableExists(string stationInitiales, string NomVoie)
        {
            string query = $"CREATE TABLE IF NOT EXISTS {stationInitiales.ToLower()}.{NomVoie.ToLower()} (horodate TIMESTAMP WITHOUT TIME ZONE NOT NULL, evenement TEXT, mesure DOUBLE PRECISION, mesure_brute TEXT);";
            try
            {
                await _connection.ExecuteAsync(query);
            }
            catch (Exception)
            {
                return (false, null);
            }

            query = $"SELECT max(horodate) from {stationInitiales.ToLower()}.{NomVoie.ToLower()}";
            try
            {
                var result = await _connection.QueryAsync<DateTime>(query);
                return (true, result.FirstOrDefault());
            }
            catch (Exception)
            {
                return (false, null);
            }

        }
        public async Task<bool> EnsureSchemaExists(string stationInitiales)
        {
            string query = $"CREATE SCHEMA IF NOT EXISTS {stationInitiales.ToLower()}";
            try
            {
                await _connection.ExecuteAsync(query);
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }

    }
}
