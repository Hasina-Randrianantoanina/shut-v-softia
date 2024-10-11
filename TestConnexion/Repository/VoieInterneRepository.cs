using Dapper;
using Npgsql;
using TestConnexion.Entities;

namespace TestConnexion.Repository
{
    public class VoieInterneRepository :IVoieInterneRepository
    {
        private readonly NpgsqlConnection _connectionString = new NpgsqlConnection("Host=localhost;Database=stations_test;Username=postgres;Password=rrrrr;");

        public async Task<VoieInterne> GetOneAsync(string initialesStation, int numero)
        {
            string query = $"""
                SELECT
                adresse_bes AS AdresseBES,
                info,
                seuil_bas AS SeuilBas,
                seuil_haut AS SeuilHaut,
                delta AS ValeurDelta,
                groupe,
                numero,
                initiales_station AS InitialesStation,
                defaut
                FROM voies_internes
                WHERE initiales_station = '{initialesStation}' AND numero = {numero}
                """;
            return await _connectionString.QueryFirstOrDefaultAsync<VoieInterne>(query);
        }

        public async Task<List<VoieInterne>> GetAllAsync()
        {
            string query = """
                SELECT
                adresse_bes AS AdresseBES,
                info,
                seuil_bas AS SeuilBas,
                seuil_haut AS SeuilHaut,
                delta AS ValeurDelta,
                groupe,
                numero,
                initiales_station AS InitialesStation,
                defaut
                FROM voies_internes
                """;
            return (List<VoieInterne>)await _connectionString.QueryAsync<VoieInterne>(query);
        }
        public async Task<List<VoieInterne>> GetAllOfAStationAsync(string initialesStation)
        {
            string query = $"""
                SELECT
                adresse_bes AS AdresseBES,
                info,
                seuil_bas AS SeuilBas,
                seuil_haut AS SeuilHaut,
                delta AS ValeurDelta,
                groupe,
                numero,
                initiales_station AS InitialesStation,
                defaut
                FROM voies_internes
                WHERE initiales_station = '{initialesStation}'
                """;
            return (List<VoieInterne>)await _connectionString.QueryAsync<VoieInterne>(query);
        }

        public async Task<int> AddAsync(VoieInterne station)
        {
            return 0; // Not used here
        }

        public async Task<int> UpdateAsync(VoieInterne voie)
        {
            string query = $"""
                UPDATE voies_internes SET
                adresse_bes = @AdresseBES,
                info = @Info,
                seuil_bas = @SeuilBas,
                seuil_haut = @SeuilHaut,
                delta = @ValeurDelta,
                groupe = @Groupe,
                defaut = @Defaut
                WHERE initiales_station = '{voie.InitialesStation}' AND numero = {voie.Numero}
                """;
            return await _connectionString.ExecuteAsync(query, voie);
        }

        public async ValueTask DisposeAsync()
        {
            await _connectionString.DisposeAsync();
        }
    }
}
