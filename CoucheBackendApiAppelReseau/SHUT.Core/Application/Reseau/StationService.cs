using SHUT.Core.Data;
using Dapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
using SHUT.Core.Domain.Reseau;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace SHUT.Core.Application.Reseau
{
    public class StationService
    {
        private readonly AppDbContext _context;
        private readonly EnregistreurService _enregistreurService;
        private readonly VoiesService _voiesService;
        private readonly ILogger<StationService> _logger;

        public StationService(AppDbContext context, EnregistreurService enregistreurService, VoiesService voiesService, ILogger<StationService> logger)
        {
            _context = context;
            _enregistreurService = enregistreurService;
            _voiesService = voiesService;
            _logger = logger;
        }

        private StationDetails ConvertDatesToLocal(StationDetails station)
        {
            station.DernierAppel = station.DernierAppel?.ToLocalTime();
            station.DernierEnregistrement = station.DernierEnregistrement?.ToLocalTime();
            station.DernierTransfert = station.DernierTransfert?.ToLocalTime();
            station.DateMaj = station.DateMaj?.ToLocalTime();
            station.DernierAppelDate = station.DernierAppelDate?.ToLocalTime();
            return station;
        }

        private IEnumerable<StationDetails> ConvertDatesToLocal(IEnumerable<StationDetails> stations)
        {
            foreach (var station in stations)
            {
                ConvertDatesToLocal(station);
            }
            return stations;
        }

        // Récupère toutes les stations
        public async Task<IEnumerable<StationsReseau>> GetStations()
        {
            try
            {
                var query = @"SELECT id, initiales, nom, enregistreur_id AS EnregistreurId, numero, 
                          bassin_versant AS BassinVersant, actif, reseau, abonnements, preselections
                          FROM reseau.stations";
                using var connection = _context.CreateConnection();
                var result = await connection.QueryAsync<StationsReseau>(query);

                if (!result.Any())
                {
                    throw new InvalidOperationException("Aucune station n'a été trouvée dans la base de données.");
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur dans GetStations: échec de la récupération des stations");
                throw;
            }
        }

        // Récupère une station spécifique par son ID
        public async Task<StationsReseau> GetStationById(int id)
        {
            try
            {
                var query = @"SELECT id, initiales, nom, enregistreur_id AS EnregistreurId, numero, 
                          bassin_versant AS BassinVersant, actif, reseau, abonnements, preselections
                          FROM reseau.stations 
                          WHERE id = @Id";
                using var connection = _context.CreateConnection();
                var result = await connection.QuerySingleOrDefaultAsync<StationsReseau>(query, new { Id = id });

                if (result == null)
                {
                    throw new KeyNotFoundException($"Aucune station trouvée avec l'ID: {id}");
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur dans GetStationById: échec de la récupération de la station (ID: {Id})", id);
                throw;
            }
        }

        // Ajoute une nouvelle station avec son enregistreur associé
        public async Task<(int StationId, bool IpExists)> AddStationWithEnregistreur(StationWithEnregistreur stationWithEnregistreur)
        {
            using var connection = _context.CreateConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();

            try
            {
                await VerifyStationData(stationWithEnregistreur, connection, transaction);

                string newIp = $"192.168.{stationWithEnregistreur.Numero}.{(stationWithEnregistreur.Liaison == "IP" ? "1" : "9")}";

                var enregistreur = new Enregistreurs
                {
                    AdresseIp = newIp,
                    Liaison = stationWithEnregistreur.Liaison,
                    Version = stationWithEnregistreur.EnregistreurVersion,
                    DateMaj = DateTime.UtcNow,
                    PourcentageMemoire = stationWithEnregistreur.PourcentageMemoire,
                    TypeHeure = stationWithEnregistreur.TypeHeure
                };
                var enregistreurId = await _enregistreurService.AddEnregistreur(enregistreur, connection, transaction);

                var station = new StationsReseau
                {
                    Initiales = stationWithEnregistreur.Initiales,
                    Nom = stationWithEnregistreur.Nom,
                    EnregistreurId = enregistreurId,
                    Numero = stationWithEnregistreur.Numero,
                    BassinVersant = stationWithEnregistreur.BassinVersant,
                    Actif = stationWithEnregistreur.Actif,
                    Reseau = stationWithEnregistreur.Reseau,
                    Abonnements = stationWithEnregistreur.Abonnements,
                    Preselections = stationWithEnregistreur.Preselections
                };
                var stationId = await AddStation(station, connection, transaction);

                transaction.Commit();
                return (stationId, false);
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                _logger.LogError(ex, "Erreur lors de l'ajout de la station avec enregistreur");
                throw;
            }
        }
        // Méthode pour ajouter une station
        private async Task<int> AddStation(StationsReseau station, IDbConnection connection, IDbTransaction transaction)
        {
            try
            {
                var query = @"
                    INSERT INTO reseau.stations (bassin_versant, enregistreur_id, initiales, nom, numero, actif, reseau, abonnements, preselections)
                    VALUES (@BassinVersant, @EnregistreurId, @Initiales, @Nom, @Numero, @Actif, @Reseau, @Abonnements, @Preselections)
                    RETURNING id";
                var newId = await connection.ExecuteScalarAsync<int>(query, station, transaction);
                return newId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur dans AddStation: échec de l'ajout de la station (Initiales: {Initiales})", station.Initiales);
                throw;
            }
            //TODO : Creer le schema correspondant dans la base d'histo et les tables de voies associées
        }


        public async Task<int> CreateStationFromModel(int modelStationId, StationWithEnregistreur newStation)
        {
            using var connection = _context.CreateConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();

            try
            {
                await VerifyStationData(newStation, connection, transaction);

                string newIp = $"192.168.{newStation.Numero}.9";

                var newEnregistreur = new Enregistreurs
                {
                    AdresseIp = newIp,
                    Liaison = newStation.Liaison,
                    Version = newStation.EnregistreurVersion,
                    DateMaj = DateTime.UtcNow,
                    PourcentageMemoire = newStation.PourcentageMemoire,
                    TypeHeure = newStation.TypeHeure
                };
                var newEnregistreurId = await _enregistreurService.AddEnregistreur(newEnregistreur, connection, transaction);

                newStation.EnregistreurId = newEnregistreurId;
                var newStationId = await AddStation(newStation, connection, transaction);

                var createdStationQuery = @"SELECT id FROM reseau.stations WHERE id = @Id";
                var createdStation = await connection.QuerySingleOrDefaultAsync<StationsReseau>(createdStationQuery, new { Id = newStationId }, transaction);

                var voiesCreated = await _voiesService.CreateVoiesFromModel(modelStationId, newStationId, newStation.Initiales, connection, transaction);

                transaction.Commit();
                return newStationId;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new Exception($"Erreur dans CreateStationFromModel: échec de la création de la station à partir du modèle (ModelStationId: {modelStationId}, NewStationInitiales: {newStation.Initiales})", ex);
            }
        }

        // Met à jour une station existante
        public async Task<bool> UpdateStation(StationsReseau station, Enregistreurs enregistreur)
        {
            using var connection = _context.CreateConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();

            try
            {
                var currentStation = await GetStationById(station.Id);
                if (currentStation == null)
                {
                    throw new KeyNotFoundException($"Aucune station trouvée avec l'ID : {station.Id}");
                }

                await VerifyStationData(station, connection, transaction, isUpdate: true, currentStationId: station.Id);

                var ipCheckQuery = @"
                    SELECT COUNT(*) 
                    FROM reseau.stations s
                    JOIN reseau.enregistreurs e ON s.enregistreur_id = e.id
                    WHERE s.actif = true AND e.adresse_ip = @AdresseIp AND s.id != @StationId";
                var count = await connection.ExecuteScalarAsync<int>(ipCheckQuery, new { AdresseIp = enregistreur.AdresseIp, StationId = station.Id }, transaction);

                if (count > 0)
                {
                    throw new InvalidOperationException("Cette adresse IP est déjà utilisée par une autre station active.");
                }

                var updateStationQuery = @"
                    UPDATE reseau.stations
                    SET initiales = @Initiales, 
                        nom = @Nom, 
                        numero = @Numero, 
                        bassin_versant = @BassinVersant,
                        actif = @Actif,
                        reseau = @Reseau,
                        abonnements = @Abonnements,
                        preselections = @Preselections
                    WHERE id = @Id";
                var affectedRows = await connection.ExecuteAsync(updateStationQuery, station, transaction);

                var updateEnregistreurQuery = @"
                    UPDATE reseau.enregistreurs
                    SET adresse_ip = @AdresseIp,
                        liaison = @Liaison,
                        version = @Version,
                        mise_a_jour = @DateMaj,
                        dernier_transfert = @DernierTransfert,
                        dernier_appel = @DernierAppel,
                        dernier_enregistrement = @DernierEnregistrement,
                        pourcentage_memoire = @PourcentageMemoire,
                        type_heure = @TypeHeure
                    WHERE id = @Id";
                var enregistreurAffectedRows = await connection.ExecuteAsync(updateEnregistreurQuery, enregistreur, transaction);

                transaction.Commit();

                return affectedRows > 0 && enregistreurAffectedRows > 0;
            }
            catch (KeyNotFoundException)
            {
                transaction.Rollback();
                throw;
            }
            catch (ArgumentException)
            {
                transaction.Rollback();
                throw;
            }
            catch (InvalidOperationException)
            {
                transaction.Rollback();
                throw;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new Exception($"Erreur inattendue dans UpdateStation: {ex.Message}", ex);
            }
        }


        public async Task<bool> UpdateStationAbonnements(int stationId, int abonnements)
        {
            using var connection = _context.CreateConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();

            try
            {
                var updateAbonnementsQuery = @"
                    UPDATE reseau.stations
                    SET abonnements = @Abonnements
                    WHERE id = @StationId";

                var affectedRows = await connection.ExecuteAsync(updateAbonnementsQuery,
                    new { Abonnements = abonnements, StationId = stationId }, transaction);

                transaction.Commit();
                return affectedRows > 0;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                _logger.LogError(ex, "Erreur lors de la mise à jour des abonnements pour la station {StationId}", stationId);
                throw;
            }
        }

        public async Task<bool> UpdateStationPreselections(int stationId, long preselections)
        {
            using var connection = _context.CreateConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();

            try
            {
                var updatePreselectionsQuery = @"
                    UPDATE reseau.stations
                    SET preselections = @Preselections
                    WHERE id = @StationId";

                var affectedRows = await connection.ExecuteAsync(updatePreselectionsQuery,
                    new { Preselections = preselections, StationId = stationId }, transaction);

                transaction.Commit();
                return affectedRows > 0;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                _logger.LogError(ex, "Erreur lors de la mise à jour des présélections pour la station {StationId}", stationId);
                throw;
            }
        }

        // MMéthode de vérification des données de la station et de l'enregistreur associé
        private async Task VerifyStationData(StationsReseau station, IDbConnection connection, IDbTransaction transaction, bool isUpdate = false, int currentStationId = 0)
        {
            if (station.Numero > 255)
            {
                throw new ArgumentException("Le numéro de la station ne peut pas dépasser 255.");
            }

            if (string.IsNullOrEmpty(station.Initiales) || station.Initiales.Length < 2 || station.Initiales.Length > 3 || !station.Initiales.All(c => char.IsLetterOrDigit(c)))
            {
                throw new ArgumentException("Les initiales de la station doivent contenir 2 ou 3 caractères alphanumériques.");
            }

            var existingActiveStationQuery = "SELECT * FROM reseau.stations WHERE numero = @Numero AND actif = true";
            if (isUpdate)
            {
                existingActiveStationQuery += " AND id != @CurrentStationId";
            }
            var existingActiveStation = await connection.QuerySingleOrDefaultAsync<StationsReseau>(
                existingActiveStationQuery,
                new { Numero = station.Numero, CurrentStationId = currentStationId },
                transaction
            );

            if (existingActiveStation != null)
            {
                throw new InvalidOperationException("Une station active avec ce numéro existe déjà.");
            }

            var existingStationWithInitialesQuery = "SELECT * FROM reseau.stations WHERE initiales = @Initiales";
            if (isUpdate)
            {
                existingStationWithInitialesQuery += " AND id != @CurrentStationId";
            }
            var existingStationWithInitiales = await connection.QuerySingleOrDefaultAsync<StationsReseau>(
                existingStationWithInitialesQuery,
                new { Initiales = station.Initiales, CurrentStationId = currentStationId },
                transaction
            );

            if (existingStationWithInitiales != null)
            {
                throw new InvalidOperationException("Une station avec ces initiales existe déjà.");
            }
        }

        // Supprime une station par son ID
        public async Task<bool> DeleteStation(int id)
        {
            try
            {
                var query = "DELETE FROM reseau.stations WHERE id = @Id";
                using var connection = _context.CreateConnection();
                var affectedRows = await connection.ExecuteAsync(query, new { Id = id });
                return affectedRows > 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur dans DeleteStation: échec de la suppression de la station (ID: {id})", ex);
            }
        }

        // Récupère toutes les stations avec les informations de leur enregistreur associé
        public async Task<IEnumerable<StationWithEnregistreur>> GetStationsWithEnregistreur()
        {
            try
            {
                var query = @"
                    SELECT s.id, s.initiales, s.nom, 
                        s.enregistreur_id AS EnregistreurId, s.numero, 
                        s.bassin_versant AS BassinVersant, s.actif, s.reseau,
                        s.abonnements, s.preselections,
                        e.version AS EnregistreurVersion,
                        e.adresse_ip AS AdresseIp,
                        e.liaison AS Liaison,
                        e.pourcentage_memoire AS PourcentageMemoire,
                        e.type_heure AS TypeHeure
                    FROM reseau.stations s
                    LEFT JOIN reseau.enregistreurs e ON s.enregistreur_id = e.id
                    ORDER BY s.initiales ASC";

                using var connection = _context.CreateConnection();
                var result = await connection.QueryAsync<StationWithEnregistreur>(query);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur dans GetStationsWithEnregistreur: échec de la récupération des stations avec enregistreurs");
                throw;
            }
        }

        // Récupère les numéro de station disponibles
        public async Task<IEnumerable<int>> GetAvailableStationNumbers()
        {
            try
            {
                var query = @"
                    SELECT number
                    FROM generate_series(1, 255) AS number
                    WHERE number NOT IN (
                        SELECT numero FROM reseau.stations WHERE actif = true
                        UNION
                        SELECT CAST(SPLIT_PART(e.adresse_ip, '.', 3) AS INTEGER)
                        FROM reseau.stations s
                        JOIN reseau.enregistreurs e ON s.enregistreur_id = e.id
                        WHERE s.actif = true AND (e.liaison = 'AP' OR e.liaison = 'IP')
                    )
                    ORDER BY number";

                using var connection = _context.CreateConnection();
                var result = await connection.QueryAsync<int>(query);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Erreur dans GetAvailableStationNumbers : échec de la récupèration des numéros de station disponibles", ex);
            }
        }

        // Récupère les détails des stations actives du réseau USAGE avec leurs informations associées (appel, defauts, enregistreur)        
        public async Task<IEnumerable<StationDetails>> GetStationDetails()
        {
            try
            {
                var query = @"
                    WITH latest_appel AS (
                        SELECT DISTINCT ON (station_id)
                            station_id,
                            date_appel,
                            statut
                        FROM 
                            appel.appels
                        ORDER BY 
                            station_id, date_appel DESC
                    ),
                    defauts_etat AS (
                        SELECT DISTINCT s.id AS station_id, TRUE AS has_defaut_etat
                        FROM reseau.stations s
                        JOIN reseau.voies_tor vt ON vt.station_id = s.id
                        JOIN defaut.defauts_actifs da ON da.voie_tor_id = vt.id
                        WHERE da.type = 'ETAT'
                    ),
                    defauts_capteur AS (
                        SELECT DISTINCT s.id AS station_id, TRUE AS has_defaut_capteur
                        FROM reseau.stations s
                        JOIN reseau.voies_telemesurees vt ON vt.station_id = s.id
                        JOIN defaut.defauts_actifs da ON da.voie_telemesuree_id = vt.id
                        WHERE da.type = 'CAPTEUR' AND vt.voie_enregistree > 0
                    ),
                    defauts_ping AS (
                        SELECT DISTINCT station_id, TRUE AS has_defaut_ping
                        FROM defaut.defauts_actifs
                        WHERE type = 'PING'
                    )
                    SELECT 
                        s.id AS StationId,
                        s.initiales AS Initiales,
                        s.nom AS Nom,
                        s.abonnements AS Abonnements,
                        s.preselections AS Preselections,
                        e.dernier_appel AS DernierAppel,
                        e.dernier_enregistrement AS DernierEnregistrement,
                        e.dernier_transfert AS DernierTransfert,
                        e.mise_a_jour AS DateMaj,
                        e.liaison AS EnregistreurLiaison,
                        e.version AS EnregistreurVersion,
                        e.pourcentage_memoire AS PourcentageMemoire,
                        e.type_heure AS TypeHeure,
                        la.date_appel AS DernierAppelDate,
                        la.statut AS Statut,
                        COALESCE(de.has_defaut_etat, FALSE) AS DefautEtat,
                        COALESCE(dc.has_defaut_capteur, FALSE) AS DefautCapteur,
                        COALESCE(dp.has_defaut_ping, FALSE) AS DefautPing,
                        CASE 
                            WHEN de.has_defaut_etat IS NULL AND dc.has_defaut_capteur IS NULL AND dp.has_defaut_ping IS NULL THEN TRUE
                            ELSE FALSE
                        END AS DefautAutre
                    FROM 
                        reseau.stations s
                    LEFT JOIN 
                        reseau.enregistreurs e ON s.enregistreur_id = e.id
                    LEFT JOIN 
                        latest_appel la ON s.id = la.station_id
                    LEFT JOIN
                        defauts_etat de ON s.id = de.station_id
                    LEFT JOIN
                        defauts_capteur dc ON s.id = dc.station_id
                    LEFT JOIN
                        defauts_ping dp ON s.id = dp.station_id
                    WHERE 
                        s.actif = true
                    AND s.reseau = 'USAGE'
                    ORDER BY 
                        s.initiales ASC";

                using var connection = _context.CreateConnection();
                var result = await connection.QueryAsync<StationDetails>(query);
                return ConvertDatesToLocal(result);
            }
            catch (Exception ex)
            {
                throw new Exception("Erreur lors de l'exécution de GetStationDetails : échec de la récupération des détails de l'état des stations", ex);
            }
        }

        // Récupère les détails des stations actives du réseau OBSERVATION avec leurs informations associées (appel, defauts, enregistreur)   
        public async Task<IEnumerable<StationDetails>> GetStationDetailsObs()
        {
            try
            {
                var query = @"
                    WITH latest_appel AS (
                        SELECT DISTINCT ON (station_id)
                            station_id,
                            date_appel,
                            statut
                        FROM 
                            appel.appels
                        ORDER BY 
                            station_id, date_appel DESC
                    ),
                    defauts_etat AS (
                        SELECT DISTINCT s.id AS station_id, TRUE AS has_defaut_etat
                        FROM reseau.stations s
                        JOIN reseau.voies_tor vt ON vt.station_id = s.id
                        JOIN defaut.defauts_actifs da ON da.voie_tor_id = vt.id
                        WHERE da.type = 'ETAT'
                    ),
                    defauts_capteur AS (
                        SELECT DISTINCT s.id AS station_id, TRUE AS has_defaut_capteur
                        FROM reseau.stations s
                        JOIN reseau.voies_telemesurees vt ON vt.station_id = s.id
                        JOIN defaut.defauts_actifs da ON da.voie_telemesuree_id = vt.id
                        WHERE da.type = 'CAPTEUR' AND vt.voie_enregistree > 0
                    ),
                    defauts_ping AS (
                        SELECT DISTINCT station_id, TRUE AS has_defaut_ping
                        FROM defaut.defauts_actifs
                        WHERE type = 'PING'
                    )
                    SELECT 
                        s.id AS StationId,
                        s.initiales AS Initiales,
                        s.nom AS Nom,
                        s.abonnements AS Abonnements,
                        s.preselections AS Preselections,
                        e.dernier_appel AS DernierAppel,
                        e.dernier_enregistrement AS DernierEnregistrement,
                        e.dernier_transfert AS DernierTransfert,
                        e.mise_a_jour AS DateMaj,
                        e.liaison AS EnregistreurLiaison,
                        e.version AS EnregistreurVersion,
                        e.pourcentage_memoire AS PourcentageMemoire,
                        e.type_heure AS TypeHeure,
                        la.date_appel AS DernierAppelDate,
                        la.statut AS Statut,
                        COALESCE(de.has_defaut_etat, FALSE) AS DefautEtat,
                        COALESCE(dc.has_defaut_capteur, FALSE) AS DefautCapteur,
                        COALESCE(dp.has_defaut_ping, FALSE) AS DefautPing,
                        CASE 
                            WHEN de.has_defaut_etat IS NULL AND dc.has_defaut_capteur IS NULL AND dp.has_defaut_ping IS NULL THEN TRUE
                            ELSE FALSE
                        END AS DefautAutre
                    FROM 
                        reseau.stations s
                    LEFT JOIN 
                        reseau.enregistreurs e ON s.enregistreur_id = e.id
                    LEFT JOIN 
                        latest_appel la ON s.id = la.station_id
                    LEFT JOIN
                        defauts_etat de ON s.id = de.station_id
                    LEFT JOIN
                        defauts_capteur dc ON s.id = dc.station_id
                    LEFT JOIN
                        defauts_ping dp ON s.id = dp.station_id
                    WHERE 
                        s.actif = true
                    AND s.reseau = 'OBSERVATION'
                    ORDER BY 
                        s.initiales ASC";
                using var connection = _context.CreateConnection();
                var result = await connection.QueryAsync<StationDetails>(query);
                return ConvertDatesToLocal(result);
            }
            catch (Exception ex)
            {
                throw new Exception("Erreur lors de l'exécution de GetStationDetails : échec de la récupération des détails de l'état des stations", ex);
            }
        }

    }


    public class StationWithEnregistreur : StationsReseau
    {
        public string? EnregistreurVersion { get; set; }
        public string? AdresseIp { get; set; }
        public string? Liaison { get; set; }
        public int? PourcentageMemoire { get; set; }
        public string? TypeHeure { get; set; }
    }
    public class StationDetails
    {
        public int StationId { get; set; }
        public string? Initiales { get; set; }
        public string? Nom { get; set; }
        public DateTime? DernierAppel { get; set; }
        public DateTime? DernierEnregistrement { get; set; }
        public DateTime? DernierTransfert { get; set; }
        public DateTime? DateMaj { get; set; }
        public DateTime? DernierAppelDate { get; set; }
        public string? Statut { get; set; }
        public bool? DefautEtat { get; set; }
        public bool? DefautCapteur { get; set; }
        public bool? DefautPing { get; set; }
        public bool? DefautAutre { get; set; }
        public string? EnregistreurLiaison { get; set; }
        public string? EnregistreurVersion { get; set; }
        public long? Abonnements { get; set; }
        public long? Preselections { get; set; }
        public int? PourcentageMemoire { get; set; }
        public string? TypeHeure { get; set; }
    }
}