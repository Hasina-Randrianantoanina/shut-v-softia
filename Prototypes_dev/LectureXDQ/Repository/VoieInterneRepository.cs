using Dapper;
using Npgsql;
using LectureXDQ.Entities;

namespace LectureXDQ.Repository
{
    public class VoieInterneRepository : IVoieInterneRepository
    {
        private readonly NpgsqlConnection _connection = new NpgsqlConnection("Host=localhost;Database=shutweb_local;Username=postgres;Password=rrrrr;");

        public async Task<List<VoieInterne>> GetAllOfAStationAsync(int stationId)
        {
            string query = $"""
                SELECT
                vtm.id,
                vtm.numero,
                vtm.delta,
                vtm.virgule,
                da.voie_telemesuree_id,
                da.id,
                da.appel,
                da.type
                FROM reseau.voies_telemesurees AS vtm
                LEFT JOIN defaut.defauts_actifs AS da
                ON da.voie_telemesuree_id = vtm.id
                WHERE vtm.station_id = {stationId}
                ORDER BY vtm.numero
                """;
            List<VoieInterne> result = (List<VoieInterne>)await _connection.QueryAsync<VoieInterne, DefautActif, VoieInterne>(
                query,
                (voieInterne, defautActif) => {
                    if (defautActif.Id != 0) { voieInterne.DefautActif = defautActif; }
                    return voieInterne;
                },
                splitOn: "voie_telemesuree_id");
            return result;
        }

        public async ValueTask DisposeAsync()
        {
            await _connection.DisposeAsync();
        }
    }
}
