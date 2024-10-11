using SHUT.Core.Data;
using Dapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
using SHUT.Core.Domain.Reseau;
using Microsoft.Extensions.Logging;

namespace SHUT.Core.Application.Reseau
{
    public class TraitementService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<TraitementService> _logger;

        public TraitementService(AppDbContext context, ILogger<TraitementService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<Traitement>> GetTraitements()
        {
            try
            {
                var query = @"SELECT id, nom, parametre1, parametre2, parametre3, parametre4, parametre5, 
                                     parametre6, parametre7, parametre8, parametre9, parametre10
                              FROM reseau.traitements";
                using var connection = _context.CreateConnection();
                return await connection.QueryAsync<Traitement>(query);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des traitements");
                throw;
            }
        }
        public async Task<Traitement> GetTraitementById(int id)
        {
            try
            {
                var query = @"SELECT id, nom, parametre1, parametre2, parametre3, parametre4, parametre5, 
                                     parametre6, parametre7, parametre8, parametre9, parametre10
                              FROM reseau.traitements
                              WHERE id = @Id";
                using var connection = _context.CreateConnection();
                return await connection.QuerySingleOrDefaultAsync<Traitement>(query, new { Id = id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erreur lors de la récupération du traitement avec l'ID {id}");
                throw;
            }
        }

        public async Task<bool> UpdateTraitement(Traitement traitement)
        {
            try
            {
                var query = @"UPDATE reseau.traitements
                              SET nom = @Nom, parametre1 = @Parametre1, parametre2 = @Parametre2, 
                                  parametre3 = @Parametre3, parametre4 = @Parametre4, parametre5 = @Parametre5,
                                  parametre6 = @Parametre6, parametre7 = @Parametre7, parametre8 = @Parametre8,
                                  parametre9 = @Parametre9, parametre10 = @Parametre10
                              WHERE id = @Id";
                using var connection = _context.CreateConnection();
                var affectedRows = await connection.ExecuteAsync(query, traitement);
                return affectedRows > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erreur lors de la mise à jour du traitement avec l'ID {traitement.Id}");
                throw;
            }
        }

        public async Task<IEnumerable<VoieTraitementInfo>> GetVoiesTraitementsByStationInitiales(List<string> stationInitiales)
        {
            try
            {
                var query = @"
                    SELECT 
                        v.id AS VoieId,
                        v.station_id AS StationId,
                        s.initiales AS StationInitiales,
                        v.libelle AS VoieLibelle,
                        v.traitement_id AS TraitementId,
                        t.nom AS TraitementNom,
                        v.parametre1 AS VoieParametre1,
                        t.parametre1 AS TraitementParametre1,
                        v.parametre2 AS VoieParametre2,
                        t.parametre2 AS TraitementParametre2,
                        v.parametre3 AS VoieParametre3,
                        t.parametre3 AS TraitementParametre3,
                        v.parametre4 AS VoieParametre4,
                        t.parametre4 AS TraitementParametre4,
                        v.parametre5 AS VoieParametre5,
                        t.parametre5 AS TraitementParametre5,
                        v.parametre6 AS VoieParametre6,
                        t.parametre6 AS TraitementParametre6,
                        v.parametre7 AS VoieParametre7,
                        t.parametre7 AS TraitementParametre7,
                        v.parametre8 AS VoieParametre8,
                        t.parametre8 AS TraitementParametre8,
                        v.parametre9 AS VoieParametre9,
                        t.parametre9 AS TraitementParametre9,
                        v.parametre10 AS VoieParametre10,
                        t.parametre10 AS TraitementParametre10,
                        e.liaison AS EnregistreurLiaison,
                        e.version AS EnregistreurVersion
                    FROM reseau.voies_telemesurees v
                    JOIN reseau.stations s ON v.station_id = s.id
                    LEFT JOIN reseau.traitements t ON v.traitement_id = t.id
                    LEFT JOIN reseau.enregistreurs e ON s.enregistreur_id = e.id
                    WHERE s.initiales = ANY(@StationInitiales)
                    AND v.traitement_id IS NOT NULL
                    ORDER BY 
                        CASE 
                            WHEN v.traitement_id = 1 THEN 0
                            ELSE 1 
                        END,
                        s.initiales, 
                        v.libelle";

                using var connection = _context.CreateConnection();
                return await connection.QueryAsync<VoieTraitementInfo>(query, new { StationInitiales = stationInitiales });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des voies avec traitements pour les stations spécifiées");
                throw;
            }
        }
    }

    public class VoieTraitementInfo
    {
        public int VoieId { get; set; }
        public int StationId { get; set; }
        public string? StationInitiales { get; set; }
        public string? VoieLibelle { get; set; }
        public int? TraitementId { get; set; }
        public string? TraitementNom { get; set; }
        public string? VoieParametre1 { get; set; }
        public string? TraitementParametre1 { get; set; }
        public string? VoieParametre2 { get; set; }
        public string? TraitementParametre2 { get; set; }
        public string? VoieParametre3 { get; set; }
        public string? TraitementParametre3 { get; set; }
        public string? VoieParametre4 { get; set; }
        public string? TraitementParametre4 { get; set; }
        public string? VoieParametre5 { get; set; }
        public string? TraitementParametre5 { get; set; }
        public string? VoieParametre6 { get; set; }
        public string? TraitementParametre6 { get; set; }
        public string? VoieParametre7 { get; set; }
        public string? TraitementParametre7 { get; set; }
        public string? VoieParametre8 { get; set; }
        public string? TraitementParametre8 { get; set; }
        public string? VoieParametre9 { get; set; }
        public string? TraitementParametre9 { get; set; }
        public string? VoieParametre10 { get; set; }
        public string? TraitementParametre10 { get; set; }
        public string EnregistreurLiaison { get; set; }
        public string EnregistreurVersion { get; set; }

    }
}