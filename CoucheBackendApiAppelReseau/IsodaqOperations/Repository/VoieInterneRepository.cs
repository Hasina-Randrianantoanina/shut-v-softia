
using IsodaqOperations.Entities;
using IsodaqOperations.Utils;
using Dapper;
using System.Data;

namespace IsodaqOperations.Repository
{
    public class VoieInterneRepository : IVoieInterneRepository
    {
        private readonly IDbConnection _connection = DataBaseUtils.AppDbContext.CreateConnection();

        public async Task<List<VoieInterne>> GetAllOfAStationAsync(int stationId)
        {
            string query = $"""
                SELECT
                vtm.id,
                vtm.numero,
                vtm.libelle as nom,
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
            await Task.Run(() => _connection.Dispose());
        }
    }
}
