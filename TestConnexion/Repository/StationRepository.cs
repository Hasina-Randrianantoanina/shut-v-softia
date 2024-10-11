using Dapper;
using Npgsql;
using TestConnexion.Entities;

namespace TestConnexion.Repository
{
    public class StationRepository : IStationRepository
    {
        private readonly NpgsqlConnection _connectionString = new NpgsqlConnection("Host=localhost;Database=stations_test;Username=postgres;Password=rrrrr;");

        public async Task<Station> GetOneAsync(string initiales)
        {
            string query = $"""
                SELECT
                initiales,
                adresse_ip AS AdresseIP,
                dernier_transfert AS DernierTransfert,
                type_liaison AS TypeLiaison,
                version_enregistreur AS Version
                FROM stations
                WHERE initiales = '{initiales}'
                """;
            return await _connectionString.QueryFirstOrDefaultAsync<Station>(query);
        }

        public async Task<List<Station>> GetAllAsync()
        {
            string query = """
                SELECT
                initiales,
                adresse_ip AS AdresseIP,
                dernier_transfert AS DernierTransfert,
                type_liaison AS TypeLiaison,
                version_enregistreur AS Version
                FROM stations
                """;
            return (List<Station>)await _connectionString.QueryAsync<Station>(query);
        }

        public async Task<int> AddAsync(Station station)
        {
            return 0; // Not used here

            string query = $"""
                INSERT INTO stations (initiales, adresse_ip, dernier_transfert, type_liaison, version_enregistreur)
                VALUES (@Initiales, @AdresseIP, @DernierTransfert, @TypeLiaison, @Version);
                """;
            return await _connectionString.ExecuteAsync(query, station);
        }

        public async Task<int> UpdateAsync(Station station)
        {
            string query = $"""
                UPDATE stations SET
                adresse_ip = @AdresseIP, 
                dernier_transfert = @DernierTransfert,
                type_liaison = @TypeLiaison,
                version_enregistreur = @Version
                WHERE initiales = '{station.Initiales}';
                """;
            return await _connectionString.ExecuteAsync(query, station);
        }

        public async Task<int> DeleteAsync(string initiales)
        {
            return 0; // Not used here

            string query = $"""
                DELETE FROM stations
                WHERE initiales = '{initiales}';
                """;
            return await _connectionString.ExecuteAsync(query);
        }

        public async ValueTask DisposeAsync()
        {
            await _connectionString.DisposeAsync();
        }
    }
}
