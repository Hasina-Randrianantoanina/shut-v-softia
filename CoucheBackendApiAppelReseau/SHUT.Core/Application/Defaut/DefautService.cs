using SHUT.Core.Data;
using Dapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SHUT.Core.Domain.Defauts;
using SHUT.Core.Domain.Reseau;
using Microsoft.Extensions.Logging;

namespace SHUT.Core.Application.Defaut
{
    public class DefautService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<DefautService> _logger;

        public DefautService(AppDbContext context, ILogger<DefautService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<DefautStationCount>> GetDefautsCapteurLast24Hours()
        {
            try
            {
                var query = @"
                    SELECT 
                        s.initiales AS StationInitiales,
                        s.abonnements AS StationAbonnements,
                        s.preselections AS StationPreselections,
                        vt.libelle AS VoieLibelle,
                        vt.abonnements AS VoieAbonnements,
                        COUNT(*) AS NombreOccurrences,
                        e.version AS EnregistreurVersion,
                        e.liaison AS EnregistreurLiaison
                    FROM defaut.defauts d
                    INNER JOIN reseau.voies_telemesurees vt ON d.voie_telemesuree_id = vt.id
                    INNER JOIN reseau.stations s ON vt.station_id = s.id
                    INNER JOIN reseau.enregistreurs e ON s.enregistreur_id = e.id
                    WHERE d.type = 'CAPTEUR'
                        AND d.appel >= @StartDate
                        AND d.appel <= @EndDate
                    GROUP BY s.initiales, s.abonnements, s.preselections, vt.libelle, vt.abonnements, e.version, e.liaison
                    ORDER BY s.initiales, vt.libelle";

                var parameters = new
                {
                    StartDate = DateTime.UtcNow.AddHours(-10000),
                    EndDate = DateTime.UtcNow
                };

                using var connection = _context.CreateConnection();
                var result = await connection.QueryAsync<DefautStationCount>(query, parameters);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des défauts de type CAPTEUR des dernières 24 heures");
                throw;
            }
        }
    }

    public class DefautStationCount
    {
        public string StationInitiales { get; set; }
        public string VoieLibelle { get; set; }
        public int NombreOccurrences { get; set; }
        public string EnregistreurVersion { get; set; }
        public string EnregistreurLiaison { get; set; }
        public long? StationAbonnements { get; set; }
        public long? StationPreselections { get; set; }
        public long? VoieAbonnements { get; set; }
    }
}