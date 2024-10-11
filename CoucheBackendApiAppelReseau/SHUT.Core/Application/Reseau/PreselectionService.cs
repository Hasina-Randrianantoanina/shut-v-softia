using SHUT.Core.Data;
using Dapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
using SHUT.Core.Domain.Reseau;
using Microsoft.Extensions.Logging;

namespace SHUT.Core.Application.Reseau
{
    public class PreselectionService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<PreselectionService> _logger;

        public PreselectionService(AppDbContext context, ILogger<PreselectionService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<Preselections>> GetPreselections()
        {
            try
            {
                var query = @"SELECT id, code, preselection as Name
                              FROM reseau.preselections";
                using var connection = _context.CreateConnection();
                return await connection.QueryAsync<Preselections>(query);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des présélections");
                throw;
            }
        }

        public async Task<Preselections> GetPreselectionById(int id)
        {
            try
            {
                var query = @"SELECT id, code, preselection as Name
                              FROM reseau.preselections
                              WHERE id = @Id";
                using var connection = _context.CreateConnection();
                return await connection.QuerySingleOrDefaultAsync<Preselections>(query, new { Id = id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erreur lors de la récupération de la présélection avec l'ID {id}");
                throw;
            }
        }

        public async Task<int> InsertPreselection(Preselections preselection)
        {
            try
            {
                var query = @"INSERT INTO reseau.preselections (code, preselection)
                              VALUES (@Code, @Name)
                              RETURNING id";
                using var connection = _context.CreateConnection();
                return await connection.ExecuteScalarAsync<int>(query, preselection);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de l'insertion d'une nouvelle présélection");
                throw;
            }
        }
    }
}