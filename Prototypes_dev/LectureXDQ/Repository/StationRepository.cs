using Dapper;
using Npgsql;
using LectureXDQ.Entities;

namespace LectureXDQ.Repository
{
    public class StationRepository : IStationRepository
    {
        private readonly NpgsqlConnection _connection = new NpgsqlConnection("Host=localhost;Database=shutweb_local;Username=postgres;Password=rrrrr;");

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
            await _connection.DisposeAsync();
        }
    }
}
