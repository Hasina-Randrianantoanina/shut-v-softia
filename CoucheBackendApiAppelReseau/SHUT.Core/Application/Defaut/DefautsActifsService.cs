using SHUT.Core.Data;
using Dapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using SHUT.Core.Domain.Defauts;
using Microsoft.Extensions.Logging;

namespace SHUT.Core.Application.Defaut
{
    public class DefautsActifsService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<DefautsActifsService> _logger;

        public DefautsActifsService(AppDbContext context, ILogger<DefautsActifsService> logger)
        {
            _context = context;
            _logger = logger;
        }

        private IEnumerable<DefautsActifsDetailed> ConvertDatesToLocal(IEnumerable<DefautsActifsDetailed> defauts)
        {
            foreach (var defaut in defauts)
            {
                defaut.Appel = defaut.Appel.ToLocalTime();
                defaut.StationDernierTransfert = defaut.StationDernierTransfert?.ToLocalTime();
                defaut.DernierAppelDate = defaut.DernierAppelDate?.ToLocalTime();
                defaut.StationDateMaj = defaut.StationDateMaj?.ToLocalTime();
            }
            return defauts;
        }

        public async Task<bool> UpdateDefautActif(int id, string commentaire, string utilisateur)
        {
            try
            {
                var query = @"
                    UPDATE defaut.defauts_actifs
                    SET commentaire = @Commentaire,
                        utilisateur_id = (SELECT id FROM administration.utilisateurs WHERE nom = @Utilisateur)
                    WHERE id = @Id";

                using var connection = _context.CreateConnection();
                var affectedRows = await connection.ExecuteAsync(query, new { Id = id, Commentaire = commentaire, Utilisateur = utilisateur });
                return affectedRows > 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la mise à jour du défaut actif avec l'ID {id}", ex);
            }
        }

        public async Task<IEnumerable<DefautsActifsDetailed>> GetDefautsActifsCapteurUsage()
        {
            try
            {
                var query = @"
                    SELECT 
                        da.id,
                        da.commentaire,
                        da.description_defaut AS DescriptionDefaut,
                        da.type,
                        da.voie_telemesuree_id AS VoieTelemesureeId,
                        da.utilisateur_id AS UtilisateurId,
                        da.appel AS Appel,
                        vt.station_id AS StationId,
                        s.initiales AS StationInitiales,
                        s.reseau AS StationReseau,
                        s.abonnements AS StationAbonnements,
                        s.preselections AS StationPreselection,
                        e.liaison AS EnregistreurLiaison,
                        e.version AS EnregistreurVersion,
                        e.dernier_transfert AS StationDernierTransfert,
                        vt.libelle AS VoieLibelle,
                        vt.priorite AS VoiePriorite,
                        vt.abonnements AS VoieAbonnements,
                        u.nom AS UtilisateurNom
                    FROM defaut.defauts_actifs da
                    INNER JOIN reseau.voies_telemesurees vt ON da.voie_telemesuree_id = vt.id
                    LEFT JOIN reseau.stations s ON vt.station_id = s.id
                    LEFT JOIN reseau.enregistreurs e ON s.enregistreur_id = e.id
                    LEFT JOIN administration.utilisateurs u ON da.utilisateur_id = u.id
                    WHERE da.type = 'CAPTEUR'
                    AND s.reseau = 'USAGE'
                    AND s.actif = true";

                using var connection = _context.CreateConnection();
                var result = await connection.QueryAsync<DefautsActifsDetailed>(query);
                return ConvertDatesToLocal(result);
            }
            catch (Exception ex)
            {
                throw new Exception("Erreur lors de la récupération des défauts actifs de type CAPTEUR du réseau d'usage", ex);
            }
        }

        public async Task<IEnumerable<DefautsActifsDetailed>> GetDefautsActifsCapteurObs()
        {
            try
            {
                var query = @"
                    SELECT 
                        da.id,
                        da.commentaire,
                        da.description_defaut AS DescriptionDefaut,
                        da.type,
                        da.voie_telemesuree_id AS VoieTelemesureeId,
                        da.utilisateur_id AS UtilisateurId,
                        da.appel AS Appel,
                        vt.station_id AS StationId,
                        s.initiales AS StationInitiales,
                        s.reseau AS StationReseau,
                        s.abonnements AS StationAbonnements,
                        s.preselections AS StationPreselection,
                        e.liaison AS EnregistreurLiaison,
                        e.version AS EnregistreurVersion,
                        e.dernier_transfert AS StationDernierTransfert,
                        vt.libelle AS VoieLibelle,
                        vt.priorite AS VoiePriorite,
                        vt.abonnements AS VoieAbonnements,
                        u.nom AS UtilisateurNom
                    FROM defaut.defauts_actifs da
                    INNER JOIN reseau.voies_telemesurees vt ON da.voie_telemesuree_id = vt.id
                    LEFT JOIN reseau.stations s ON vt.station_id = s.id
                    LEFT JOIN reseau.enregistreurs e ON s.enregistreur_id = e.id
                    LEFT JOIN administration.utilisateurs u ON da.utilisateur_id = u.id
                    WHERE da.type = 'CAPTEUR'
                    AND s.reseau = 'OBSERVATION'
                    AND s.actif = true";

                using var connection = _context.CreateConnection();
                var result = await connection.QueryAsync<DefautsActifsDetailed>(query);
                return ConvertDatesToLocal(result);
            }
            catch (Exception ex)
            {
                throw new Exception("Erreur lors de la récupération des défauts actifs de type CAPTEUR du réseau d'observation", ex);
            }
        }

        public async Task<IEnumerable<DefautsActifsDetailed>> GetDefautsActifsEtatUsage()
        {
            try
            {
                var query = @"
                    SELECT 
                        da.id,
                        da.commentaire,
                        da.description_defaut AS DescriptionDefaut,
                        da.type,
                        da.voie_tor_id AS VoieTorId,
                        da.utilisateur_id AS UtilisateurId,
                        da.appel AS Appel,
                        vt.station_id AS StationId,
                        s.initiales AS StationInitiales,
                        s.reseau AS StationReseau,
                        s.abonnements AS StationAbonnements,
                        s.preselections AS StationPreselection,
                        e.liaison AS EnregistreurLiaison,
                        e.version AS EnregistreurVersion,
                        e.dernier_transfert AS StationDernierTransfert,
                        vt.libelle AS VoieLibelle,
                        u.nom AS UtilisateurNom
                    FROM defaut.defauts_actifs da
                    INNER JOIN reseau.voies_tor vt ON da.voie_Tor_id = vt.id
                    LEFT JOIN reseau.stations s ON vt.station_id = s.id
                    LEFT JOIN reseau.enregistreurs e ON s.enregistreur_id = e.id
                    LEFT JOIN administration.utilisateurs u ON da.utilisateur_id = u.id
                    WHERE da.type = 'ETAT'
                    AND s.reseau = 'USAGE'
                    AND s.actif = true";

                using var connection = _context.CreateConnection();
                var result = await connection.QueryAsync<DefautsActifsDetailed>(query);
                return ConvertDatesToLocal(result);
            }
            catch (Exception ex)
            {
                throw new Exception("Erreur lors de la récupération des défauts actifs de type ETAT du réseau d'usage", ex);
            }
        }

        public async Task<IEnumerable<DefautsActifsDetailed>> GetDefautsActifsEtatObs()
        {
            try
            {
                var query = @"
                    SELECT 
                        da.id,
                        da.commentaire,
                        da.description_defaut AS DescriptionDefaut,
                        da.type,
                        da.voie_tor_id AS VoieTorId,
                        da.utilisateur_id AS UtilisateurId,
                        da.appel AS Appel,
                        vt.station_id AS StationId,
                        s.initiales AS StationInitiales,
                        s.reseau AS StationReseau,
                        s.abonnements AS StationAbonnements,
                        s.preselections AS StationPreselection,
                        e.liaison AS EnregistreurLiaison,
                        e.version AS EnregistreurVersion,
                        e.dernier_transfert AS StationDernierTransfert,
                        vt.libelle AS VoieLibelle,
                        u.nom AS UtilisateurNom
                    FROM defaut.defauts_actifs da
                    INNER JOIN reseau.voies_tor vt ON da.voie_Tor_id = vt.id
                    LEFT JOIN reseau.stations s ON vt.station_id = s.id
                    LEFT JOIN reseau.enregistreurs e ON s.enregistreur_id = e.id
                    LEFT JOIN administration.utilisateurs u ON da.utilisateur_id = u.id
                    WHERE da.type = 'ETAT'
                    AND s.reseau = 'OBSERVATION'
                    AND s.actif = true";

                using var connection = _context.CreateConnection();
                var result = await connection.QueryAsync<DefautsActifsDetailed>(query);
                return ConvertDatesToLocal(result);
            }
            catch (Exception ex)
            {
                throw new Exception("Erreur lors de la récupération des défauts actifs de type ETAT du réseau d'observation", ex);
            }
        }

        public async Task<IEnumerable<DefautsActifsDetailed>> GetDefautsActifsPingUsage()
        {
            try
            {
                var query = @"
                    WITH LatestAppels AS (
                        SELECT DISTINCT ON (station_id)
                            station_id,
                            date_appel
                        FROM appel.appels
                        ORDER BY station_id, date_appel DESC
                    ),
                    LatestPingDefauts AS (
                        SELECT DISTINCT ON (da.station_id)
                            da.*
                        FROM defaut.defauts_actifs da
                        WHERE da.type = 'PING'
                        ORDER BY da.station_id, da.appel DESC
                    )
                    SELECT 
                        lpd.id,
                        lpd.commentaire,
                        lpd.description_defaut AS DescriptionDefaut,
                        lpd.type,
                        lpd.station_id AS StationId,
                        lpd.utilisateur_id AS UtilisateurId,
                        lpd.appel AS Appel,
                        s.initiales AS StationInitiales,
                        s.reseau AS StationReseau,
                        s.abonnements AS StationAbonnements,
                        s.preselections AS StationPreselection,
                        e.liaison AS EnregistreurLiaison,
                        e.version AS EnregistreurVersion,
                        e.dernier_transfert AS StationDernierTransfert,
                        u.nom AS UtilisateurNom,
                        la.date_appel AS DernierAppelDate
                    FROM LatestPingDefauts lpd
                    LEFT JOIN reseau.stations s ON lpd.station_id = s.id
                    LEFT JOIN reseau.enregistreurs e ON s.enregistreur_id = e.id
                    LEFT JOIN administration.utilisateurs u ON lpd.utilisateur_id = u.id
                    LEFT JOIN LatestAppels la ON s.id = la.station_id
                    WHERE s.reseau = 'USAGE'
                    AND s.actif = true";

                using var connection = _context.CreateConnection();
                var result = await connection.QueryAsync<DefautsActifsDetailed>(query);
                return ConvertDatesToLocal(result);
            }
            catch (Exception ex)
            {
                throw new Exception("Erreur lors de la récupération des défauts actifs de type PING du réseau d'usage", ex);
            }
        }

        public async Task<IEnumerable<DefautsActifsDetailed>> GetDefautsActifsPingObs()
        {
            try
            {
                var query = @"
                    WITH LatestAppels AS (
                        SELECT DISTINCT ON (station_id)
                            station_id,
                            date_appel
                        FROM appel.appels
                        ORDER BY station_id, date_appel DESC
                    ),
                    LatestPingDefauts AS (
                        SELECT DISTINCT ON (da.station_id)
                            da.*
                        FROM defaut.defauts_actifs da
                        WHERE da.type = 'PING'
                        ORDER BY da.station_id, da.appel DESC
                    )
                    SELECT 
                        lpd.id,
                        lpd.commentaire,
                        lpd.description_defaut AS DescriptionDefaut,
                        lpd.type,
                        lpd.station_id AS StationId,
                        lpd.utilisateur_id AS UtilisateurId,
                        lpd.appel AS Appel,
                        s.initiales AS StationInitiales,
                        s.reseau AS StationReseau,
                        s.abonnements AS StationAbonnements,
                        s.preselections AS StationPreselection,
                        e.liaison AS EnregistreurLiaison,
                        e.version AS EnregistreurVersion,
                        e.dernier_transfert AS StationDernierTransfert,
                        u.nom AS UtilisateurNom,
                        la.date_appel AS DernierAppelDate
                    FROM LatestPingDefauts lpd
                    LEFT JOIN reseau.stations s ON lpd.station_id = s.id
                    LEFT JOIN reseau.enregistreurs e ON s.enregistreur_id = e.id
                    LEFT JOIN administration.utilisateurs u ON lpd.utilisateur_id = u.id
                    LEFT JOIN LatestAppels la ON s.id = la.station_id
                    WHERE s.reseau = 'OBSERVATION'
                    AND s.actif = true";

                using var connection = _context.CreateConnection();
                var result = await connection.QueryAsync<DefautsActifsDetailed>(query);
                return ConvertDatesToLocal(result);
            }
            catch (Exception ex)
            {
                throw new Exception("Erreur lors de la récupération des défauts actifs de type PING du réseau d'observation", ex);
            }
        }


        public async Task<IEnumerable<DefautsActifsDetailed>> GetDefautsActifsAutreUsage()
        {
            try
            {
                var query = @"
                    WITH LatestAppels AS (
                        SELECT DISTINCT ON (station_id)
                            station_id,
                            date_appel,
                            statut
                        FROM appel.appels
                        ORDER BY station_id, date_appel DESC
                    ),
                    LatestNullDefauts AS (
                        SELECT DISTINCT ON (da.station_id)
                            da.*
                        FROM defaut.defauts_actifs da
                        WHERE da.type IS NULL
                        ORDER BY da.station_id, da.appel DESC
                    )
                    SELECT 
                        lnd.id,
                        lnd.commentaire,
                        lnd.description_defaut AS DescriptionDefaut,
                        lnd.type,
                        lnd.station_id AS StationId,
                        lnd.utilisateur_id AS UtilisateurId,
                        lnd.appel AS Appel,
                        s.initiales AS StationInitiales,
                        s.reseau AS StationReseau,
                        e.liaison AS EnregistreurLiaison,
                        e.version AS EnregistreurVersion,
                        e.dernier_transfert AS StationDernierTransfert,
                        e.mise_a_jour AS StationDateMaj,
                        u.nom AS UtilisateurNom,
                        la.date_appel AS DernierAppelDate,
                        la.statut AS DernierAppelStatut
                    FROM LatestNullDefauts lnd
                    LEFT JOIN reseau.stations s ON lnd.station_id = s.id
                    LEFT JOIN reseau.enregistreurs e ON s.enregistreur_id = e.id
                    LEFT JOIN administration.utilisateurs u ON lnd.utilisateur_id = u.id
                    LEFT JOIN LatestAppels la ON s.id = la.station_id
                    WHERE s.reseau = 'USAGE'
                    AND s.actif = true";

                using var connection = _context.CreateConnection();
                var result = await connection.QueryAsync<DefautsActifsDetailed>(query);
                return ConvertDatesToLocal(result);
            }
            catch (Exception ex)
            {
                throw new Exception("Erreur lors de la récupération des défauts actifs de type AUTRE du réseau d'usage", ex);
            }
        }


        public async Task<IEnumerable<DefautsActifsDetailed>> GetDefautsActifsAutreObs()
        {
            try
            {
                var query = @"
                    WITH LatestAppels AS (
                        SELECT DISTINCT ON (station_id)
                            station_id,
                            date_appel,
                            statut
                        FROM appel.appels
                        ORDER BY station_id, date_appel DESC
                    ),
                    LatestNullDefauts AS (
                        SELECT DISTINCT ON (da.station_id)
                            da.*
                        FROM defaut.defauts_actifs da
                        WHERE da.type IS NULL
                        ORDER BY da.station_id, da.appel DESC
                    )
                    SELECT 
                        lnd.id,
                        lnd.commentaire,
                        lnd.description_defaut AS DescriptionDefaut,
                        lnd.type,
                        lnd.station_id AS StationId,
                        lnd.utilisateur_id AS UtilisateurId,
                        lnd.appel AS Appel,
                        s.initiales AS StationInitiales,
                        s.reseau AS StationReseau,
                        e.liaison AS EnregistreurLiaison,
                        e.version AS EnregistreurVersion,
                        e.dernier_transfert AS StationDernierTransfert,
                        e.mise_a_jour AS StationDateMaj,
                        u.nom AS UtilisateurNom,
                        la.date_appel AS DernierAppelDate,
                        la.statut AS DernierAppelStatut
                    FROM LatestNullDefauts lnd
                    LEFT JOIN reseau.stations s ON lnd.station_id = s.id
                    LEFT JOIN reseau.enregistreurs e ON s.enregistreur_id = e.id
                    LEFT JOIN administration.utilisateurs u ON lnd.utilisateur_id = u.id
                    LEFT JOIN LatestAppels la ON s.id = la.station_id
                    WHERE s.reseau = 'OBSERVATION'
                    AND s.actif = true";

                using var connection = _context.CreateConnection();
                var result = await connection.QueryAsync<DefautsActifsDetailed>(query);
                return ConvertDatesToLocal(result);
            }
            catch (Exception ex)
            {
                throw new Exception("Erreur lors de la récupération des défauts actifs de type AUTRE du réseau d'observation", ex);
            }
        }
    }

    public class DefautsActifsDetailed
    {
        public int Id { get; set; }
        public DateTime Appel { get; set; }
        public string? Commentaire { get; set; }
        public string? DescriptionDefaut { get; set; }
        public string? Type { get; set; }
        public int VoieTelemesureeId { get; set; }
        public int VoieTorId { get; set; }
        public int? UtilisateurId { get; set; }
        public int StationId { get; set; }
        public string? StationInitiales { get; set; }
        public string? StationReseau { get; set; }
        public long? StationAbonnements { get; set; }
        public long? StationPreselections { get; set; }
        public DateTime? StationDernierTransfert { get; set; }
        public string? VoieLibelle { get; set; }
        public string? VoiePriorite { get; set; }
        public long? VoieAbonnements { get; set; }
        public string? UtilisateurNom { get; set; }
        public DateTime? DernierAppelDate { get; set; }
        public DateTime? StationDateMaj { get; set; }
        public string? DernierAppelStatut { get; set; }
        public string? EnregistreurLiaison { get; set; }
        public string? EnregistreurVersion { get; set; }
    }
}