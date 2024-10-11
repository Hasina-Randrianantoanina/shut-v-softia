
using IsodaqOperations.Entities;
using IsodaqOperations.Utils;
using System.Data;
using Dapper;

namespace IsodaqOperations.Repository
{
    public class StationRepository : IStationRepository
    {
        private readonly IDbConnection _connection = DataBaseUtils.AppDbContext.CreateConnection();

        public async Task<Station?> GetOneAsync(string initiales)
        {
            string query = $"""
                SELECT
                s.id,
                s.initiales,
                s.enregistreur_id,
                e.liaison AS TypeLiaison,
                e.version AS Version
                FROM reseau.stations AS s
                INNER JOIN reseau.enregistreurs AS e
                ON s.enregistreur_id = e.id
                WHERE s.initiales = '{initiales}' AND s.enregistreur_id NOTNULL
                """;
            List<Station> result = (List<Station>)await _connection.QueryAsync<Station, Enregistreur, Station>(
                query,
                (station, enregisteur) => {
                    station.Enregistreur = enregisteur;
                    return station;
                },
                splitOn: "enregistreur_id");
            return result.Count == 1 ? (Station)result[0] : null;
        }

        public async ValueTask DisposeAsync()
        {
            await Task.Run(() => _connection.Dispose());
        }
    }
}
