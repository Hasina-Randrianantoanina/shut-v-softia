using SHUT.Core.Data;
using Dapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SHUT.Core.Domain.Defauts;

namespace SHUT.Core.Application.Defaut
{
    public class PerteService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<PerteService> _logger;

        public PerteService(AppDbContext context, ILogger<PerteService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<(IEnumerable<Pertes> Pertes, EnregistreurInfo EnregistreurInfo)> GetPertesByStation(int stationId)
        {
            try
            {
                using var connection = _context.CreateConnection();

                // Requête pour les pertes
                var pertesQuery = @"
                    SELECT 
                        p.id, 
                        p.appel, 
                        p.cause, 
                        p.commentaire, 
                        p.critique, 
                        p.date_acquisition AS DateAcquisition, 
                        p.date_enregistrement AS DateEnregistrement, 
                        p.date_go AS DateGo,
                        p.date_stop AS DateStop,  
                        p.date_init AS DateInit, 
                        p.debut, 
                        p.defaut, 
                        p.diff_horloge AS DiffHorloge, 
                        p.duree, 
                        p.etat_des_taches AS EtatDesTaches, 
                        p.fin, 
                        p.horloge, 
                        p.remede,
                        p.type,  
                        p.station_id AS StationId, 
                        p.utilisateur_id AS UtilisateurId,
                        p.old_nom_utilisateur AS OldNomUtilisateur,
                        p.old_numero AS OldNumero
                    FROM defaut.pertes p
                    WHERE p.station_id = @StationId
                    ORDER BY p.debut DESC;";

                var pertes = await connection.QueryAsync<Pertes>(pertesQuery, new { StationId = stationId });

                // Convertir les dates UTC en heure locale pour les pertes
                foreach (var perte in pertes)
                {
                    perte.Appel = perte.Appel?.ToLocalTime();
                    perte.DateAcquisition = perte.DateAcquisition?.ToLocalTime();
                    perte.DateEnregistrement = perte.DateEnregistrement?.ToLocalTime();
                    perte.DateGo = perte.DateGo?.ToLocalTime();
                    perte.DateStop = perte.DateStop?.ToLocalTime();
                    perte.DateInit = perte.DateInit?.ToLocalTime();
                    perte.Debut = perte.Debut?.ToLocalTime();
                    perte.Fin = perte.Fin?.ToLocalTime();
                    perte.Horloge = perte.Horloge?.ToLocalTime();
                }

                // Requête pour les informations de l'enregistreur
                var enregistreurQuery = @"
                    SELECT e.id AS Id, e.version, e.dernier_appel AS DernierAppel, e.dernier_transfert AS DernierTransfert, 
                        e.dernier_enregistrement AS DernierEnregistrement, e.liaison, e.mise_a_jour AS MiseAJour
                    FROM reseau.stations s
                    JOIN reseau.enregistreurs e ON s.enregistreur_id = e.id
                    WHERE s.id = @StationId";

                var enregistreurInfo = await connection.QuerySingleOrDefaultAsync<EnregistreurInfo>(enregistreurQuery, new { StationId = stationId });

                // Convertir les dates UTC en heure locale pour l'enregistreur
                if (enregistreurInfo != null)
                {
                    enregistreurInfo.DernierAppel = enregistreurInfo.DernierAppel?.ToLocalTime();
                    enregistreurInfo.DernierTransfert = enregistreurInfo.DernierTransfert?.ToLocalTime();
                    enregistreurInfo.DernierEnregistrement = enregistreurInfo.DernierEnregistrement?.ToLocalTime();
                    enregistreurInfo.MiseAJour = enregistreurInfo.MiseAJour?.ToLocalTime();
                }

                return (pertes, enregistreurInfo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur pertes et enregistreur info pour station {StationId}", stationId);
                throw;
            }
        }
        public async Task<bool> UpdatePerte(Pertes perte)
        {
            try
            {
                // Convertir les dates locales en UTC
                perte.Appel = perte.Appel?.ToUniversalTime();
                perte.DateAcquisition = perte.DateAcquisition?.ToUniversalTime();
                perte.DateEnregistrement = perte.DateEnregistrement?.ToUniversalTime();
                perte.DateGo = perte.DateGo?.ToUniversalTime();
                perte.DateStop = perte.DateStop?.ToUniversalTime();
                perte.DateInit = perte.DateInit?.ToUniversalTime();
                perte.Debut = perte.Debut?.ToUniversalTime();
                perte.Fin = perte.Fin?.ToUniversalTime();
                perte.Horloge = perte.Horloge?.ToUniversalTime();

                var query = @"
                    UPDATE defaut.pertes
                    SET appel = @Appel, cause = @Cause, commentaire = @Commentaire, critique = @Critique,
                        date_acquisition = @DateAcquisition, date_enregistrement = @DateEnregistrement,
                        date_go = @DateGo, date_stop = @DateStop, date_init = @DateInit, debut = @Debut, defaut = @Defaut,
                        type = @Type, diff_horloge = @DiffHorloge, duree = @Duree, etat_des_taches = @EtatDesTaches,
                        fin = @Fin, horloge = @Horloge, remede = @Remede, utilisateur_id = @UtilisateurId
                    WHERE id = @Id";

                using var connection = _context.CreateConnection();
                var affectedRows = await connection.ExecuteAsync(query, perte);
                return affectedRows > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la mise à jour de la perte {PerteId}", perte.Id);
                throw;
            }
        }

        public async Task<int> InsertPerte(Pertes perte)
        {
            try
            {
                // Convertir les dates locales en UTC
                perte.Appel = perte.Appel?.ToUniversalTime();
                perte.DateAcquisition = perte.DateAcquisition?.ToUniversalTime();
                perte.DateEnregistrement = perte.DateEnregistrement?.ToUniversalTime();
                perte.DateGo = perte.DateGo?.ToUniversalTime();
                perte.DateStop = perte.DateStop?.ToUniversalTime();
                perte.DateInit = perte.DateInit?.ToUniversalTime();
                perte.Debut = perte.Debut?.ToUniversalTime();
                perte.Fin = perte.Fin?.ToUniversalTime();
                perte.Horloge = perte.Horloge?.ToUniversalTime();

                var query = @"
                    INSERT INTO defaut.pertes (appel, cause, commentaire, critique, date_acquisition,
                        date_enregistrement, date_go, date_stop, date_init, debut, defaut, diff_horloge, duree,
                        etat_des_taches, fin, horloge, remede, station_id, utilisateur_id, type)
                    VALUES (@Appel, @Cause, @Commentaire, @Critique, @DateAcquisition, @DateEnregistrement,
                        @DateGo, @DateStop, @DateInit, @Debut, @Defaut, @DiffHorloge, @Duree, @EtatDesTaches, @Fin,
                        @Horloge, @Remede, @StationId, @UtilisateurId, @Type)
                    RETURNING id";

                using var connection = _context.CreateConnection();
                return await connection.ExecuteScalarAsync<int>(query, perte);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la création de la perte");
                throw;
            }
        }
        public async Task<bool> DeletePerte(int perteId)
        {
            try
            {
                var query = "DELETE FROM defaut.pertes WHERE id = @Id";

                using var connection = _context.CreateConnection();
                var affectedRows = await connection.ExecuteAsync(query, new { Id = perteId });
                return affectedRows > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la suppression de la perte {PerteId}", perteId);
                throw;
            }
        }
    }

    public class EnregistreurInfo
    {
        public int Id { get; set; }
        public string? Version { get; set; }
        public DateTime? DernierAppel { get; set; }
        public DateTime? DernierTransfert { get; set; }
        public DateTime? DernierEnregistrement { get; set; }
        public string? Liaison { get; set; }
        public DateTime? MiseAJour { get; set; }
    }
}