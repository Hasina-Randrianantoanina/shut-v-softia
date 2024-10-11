using SHUT.Core.Data;
using Dapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SHUT.Core.Domain.Defauts;

namespace SHUT.Core.Application.Defaut
{
    public class AlerteService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<AlerteService> _logger;

        public AlerteService(AppDbContext context, ILogger<AlerteService> logger)
        {
            _context = context;
            _logger = logger;
        }

        private IEnumerable<T> ConvertDatesToLocal<T>(IEnumerable<T> alertes) where T : Alertes
        {
            foreach (var alerte in alertes)
            {
                alerte.DateAlerte = alerte.DateAlerte.ToLocalTime();
            }
            return alertes;
        }

        public async Task<IEnumerable<Alertes>> GetAllAlertes()
        {
            try
            {
                var query = @"
                    SELECT 
                        id,
                        acquitter,
                        commentaire,
                        date_alerte AS DateAlerte,
                        description_alerte AS DescriptionAlerte,
                        parametre_action AS ParametreAction,
                        parametre_alerte AS ParametreAlerte,
                        station_id AS StationId,
                        type,
                        utilisateur_id AS UtilisateurId
                    FROM defaut.alertes
                    WHERE station_id IS NULL
                    ORDER BY date_alerte DESC";

                using var connection = _context.CreateConnection();
                var result = await connection.QueryAsync<Alertes>(query);
                return ConvertDatesToLocal(result);
            }
            catch (Exception ex)
            {
                throw new Exception("Impossible de récupérer les alertes.", ex);
            }
        }

        public async Task<IEnumerable<AlertesWithStationInitiales>> GetAllAlertesObs()
        {
            try
            {
                var query = @"
                    SELECT 
                        a.id,
                        a.acquitter,
                        a.commentaire,
                        a.date_alerte AS DateAlerte,
                        a.description_alerte AS DescriptionAlerte,
                        a.parametre_action AS ParametreAction,
                        a.parametre_alerte AS ParametreAlerte,
                        a.station_id AS StationId,
                        a.type,
                        a.utilisateur_id AS UtilisateurId,
                        s.initiales AS StationInitiales
                    FROM defaut.alertes a
                    JOIN reseau.stations s ON a.station_id = s.id
                    WHERE a.station_id IS NOT NULL
                    AND s.actif = true
                    ORDER BY a.date_alerte DESC";

                using var connection = _context.CreateConnection();
                var result = await connection.QueryAsync<AlertesWithStationInitiales>(query);
                return ConvertDatesToLocal(result);
            }
            catch (Exception ex)
            {
                throw new Exception("Impossible de récupérer les alertes avec les informations des stations.", ex);
            }
        }

        public async Task<bool> UpdateAlerteCommentaire(int id, string commentaire, int utilisateurId)
        {
            try
            {
                var query = @"
                    UPDATE defaut.alertes
                    SET commentaire = @Commentaire,
                        utilisateur_id = @UtilisateurId
                    WHERE id = @Id";

                using var connection = _context.CreateConnection();
                var affectedRows = await connection.ExecuteAsync(query, new { Id = id, Commentaire = commentaire, UtilisateurId = utilisateurId });
                return affectedRows > 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Impossible de mettre à jour le commentaire de l'alerte (ID: {id}).", ex);
            }
        }
    }

    public class AlertesWithStationInitiales : Alertes
    {
        public string? StationInitiales { get; set; }
    }
}