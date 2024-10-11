using Dapper;
using Npgsql;
using TestConnexion.Entities;

namespace TestConnexion.Repository
{
    public class DefautRepository : IDefautRepository
    {
        private readonly NpgsqlConnection _connectionString = new NpgsqlConnection("Host=localhost;Database=stations_test;Username=postgres;Password=rrrrr;");

        public async Task<List<Defaut>> GetAllAsync()
        {
            string query = """
                SELECT
                initiales_station AS InitialesStation,
                numero_voie AS NumeroVoie,
                type,
                debut_defaut AS DebutDefaut,
                fin_defaut AS FinDefaut,
                active,
                commentaire
                FROM defauts
                """;
            return (List<Defaut>)await _connectionString.QueryAsync<Defaut>(query);
        }

        public async Task<Defaut> GetActiveDefautOfThisVoieAsync(string initialesStation, int numeroVoie)
        {
            string query = $"""
                SELECT
                initiales_station AS InitialesStation,
                numero_voie AS NumeroVoie,
                type,
                debut_defaut AS DebutDefaut,
                fin_defaut AS FinDefaut,
                active,
                commentaire
                FROM defauts
                WHERE initiales_station = '{initialesStation}' AND numero_voie = {numeroVoie} AND active
                """;
            return await _connectionString.QueryFirstOrDefaultAsync<Defaut>(query);
        }

        public async Task<int> AddAsync(Defaut defaut)
        {
            // If the defaut already exists in the DB, we update fin_defaut, active and commentaire
            string query = $"""
                INSERT INTO defauts (initiales_station, numero_voie, type, debut_defaut, fin_defaut, active, commentaire)
                VALUES (@InitialesStation, @NumeroVoie, @Type, @DebutDefaut, @FinDefaut, @Active, @Commentaire)
                ON CONFLICT (initiales_station, numero_voie, type, debut_defaut) DO UPDATE
                SET fin_defaut = EXCLUDED.fin_defaut, active = EXCLUDED.active, commentaire = EXCLUDED.commentaire
                ;
                """;
            return await _connectionString.ExecuteAsync(query, defaut);
        }

        public async Task<int> UpdateAsync(Defaut entity)
        {
            return 0;  // Not used here
        }

        public async ValueTask DisposeAsync()
        {
            await _connectionString.DisposeAsync();
        }
    }
}
