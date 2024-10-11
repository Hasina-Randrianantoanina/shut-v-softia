
using IsodaqOperations.Entities;
using IsodaqOperations.Utils;
using System.Data;
using Dapper;

namespace IsodaqOperations.Repository
{
    public class DefautActifRepository : IDefautActifRepository
    {
        private readonly IDbConnection _connection = DataBaseUtils.AppDbContext.CreateConnection();
        public async Task<int> AddAsync(DefautActif defaut, int voieTelemesureeId)
        {
            string query = $"""
                INSERT INTO defaut.defauts_actifs (appel, voie_telemesuree_id, type)
                VALUES (@Appel, {voieTelemesureeId}, @Type)
                ON CONFLICT (voie_telemesuree_id, type) DO NOTHING
                ;
                """;
            Console.WriteLine("\n" + query + "\n");
            return await _connection.ExecuteAsync(query, defaut);
        }

        public async Task<int> DeleteAsync(int voieTelemesureeId)
        {
            string query = $"""
                DELETE FROM defaut.defauts_actifs
                WHERE voie_telemesuree_id = {voieTelemesureeId}
                """;
            Console.WriteLine("\n" + query + "\n");
            return await _connection.ExecuteAsync(query);
        }

        public async ValueTask DisposeAsync()
        {
            await Task.Run(() => _connection.Dispose());
        }

    }
}
