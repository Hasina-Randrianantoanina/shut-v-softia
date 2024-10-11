using SHUT.Core.Data;
using Dapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
using SHUT.Core.Domain.Reseau;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace SHUT.Core.Application.Reseau
{
    public class VoiesService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<VoiesService> _logger;

        public VoiesService(AppDbContext context, ILogger<VoiesService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<(IEnumerable<VoiesTelemesurees> VoiesTelemesurees, IEnumerable<VoiesTor> VoiesTor)> GetVoies()
        {
            try
            {
                var queryTelemesurees = "SELECT * FROM reseau.voies_telemesurees";
                var queryTor = "SELECT * FROM reseau.voies_tor";

                using var connection = _context.CreateConnection();
                var voiesTelemesurees = await connection.QueryAsync<VoiesTelemesurees>(queryTelemesurees);
                var voiesTor = await connection.QueryAsync<VoiesTor>(queryTor);

                return (voiesTelemesurees, voiesTor);
            }
            catch (Exception ex)
            {
                throw new Exception("Erreur lors de la récupération des voies", ex);
            }
        }

        public async Task<(VoiesTelemesurees? VoieTelemesuree, VoiesTor? VoieTor)> GetVoieById(int id)
        {
            try
            {
                var queryTelemesuree = "SELECT * FROM reseau.voies_telemesurees WHERE id = @Id";
                var queryTor = "SELECT * FROM reseau.voies_tor WHERE id = @Id";

                using var connection = _context.CreateConnection();
                var voieTelemesuree = await connection.QuerySingleOrDefaultAsync<VoiesTelemesurees>(queryTelemesuree, new { Id = id });
                var voieTor = await connection.QuerySingleOrDefaultAsync<VoiesTor>(queryTor, new { Id = id });

                return (voieTelemesuree, voieTor);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la récupération de la voie avec l'ID {id}", ex);
            }
        }


        public async Task<bool> UpdateVoieTelemesuree(VoiesTelemesurees voie)
        {
            try
            {
                var query = @"
                    UPDATE reseau.voies_telemesurees
                    SET adresse_bes = @AdresseBes, delta = @Delta, groupe = @Groupe, info = @Info, 
                        libelle = @Libelle, seuil_bas = @SeuilBas, seuil_haut = @SeuilHaut, 
                        priorite = @Priorite, station_id = @StationId, unite = @Unite, 
                        numero = @Numero, ordre = @Ordre, voie_enregistree = @VoieEnregistree, 
                        voie_stockee = @VoieStockee, actif = @Actif, virgule = @Virgule,
                        parametre1 = @Parametre1, parametre2 = @Parametre2, parametre3 = @Parametre3,
                        parametre4 = @Parametre4, parametre5 = @Parametre5, parametre6 = @Parametre6,
                        parametre7 = @Parametre7, parametre8 = @Parametre8, parametre9 = @Parametre9,
                        parametre10 = @Parametre10, traitement_id = @TraitementId, abonnements = @Abonnements
                    WHERE id = @Id";

                using var connection = _context.CreateConnection();
                var affectedRows = await connection.ExecuteAsync(query, voie);
                return affectedRows > 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la mise à jour de la voie télémesurée avec l'ID {voie.Id}", ex);
            }
        }

        public async Task<bool> UpdateVoieTor(VoiesTor voie)
        {
            try
            {
                var query = @"
                    UPDATE reseau.voies_tor
                    SET adresse_bes = @AdresseBes, info = @Info, libelle = @Libelle, 
                        numero = @Numero, ordre = @Ordre, station_id = @StationId, 
                        type = @Type, actif = @Actif
                    WHERE id = @Id";

                using var connection = _context.CreateConnection();
                var affectedRows = await connection.ExecuteAsync(query, voie);
                return affectedRows > 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la mise à jour de la voie TOR avec l'ID {voie.Id}", ex);
            }
        }

        public async Task<bool> DeleteVoie(int id)
        {
            try
            {
                var queryTelemesuree = "DELETE FROM reseau.voies_telemesurees WHERE id = @Id";
                var queryTor = "DELETE FROM reseau.voies_tor WHERE id = @Id";

                using var connection = _context.CreateConnection();
                var affectedRowsTelemesuree = await connection.ExecuteAsync(queryTelemesuree, new { Id = id });
                var affectedRowsTor = await connection.ExecuteAsync(queryTor, new { Id = id });

                return (affectedRowsTelemesuree + affectedRowsTor) > 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la suppression de la voie avec l'ID {id}", ex);
            }
        }

        public async Task<(IEnumerable<VoiesTelemesurees> VoiesTelemesurees, IEnumerable<VoiesTor> VoiesTor)> GetVoiesByStationInitiales(string initiales)
        {
            try
            {
                var queryTelemesurees = @"
                    SELECT vt.* 
                    FROM reseau.voies_telemesurees vt
                    JOIN reseau.stations s ON vt.station_id = s.id
                    WHERE s.initiales = @Initiales";

                        var queryTor = @"
                    SELECT vtor.* 
                    FROM reseau.voies_tor vtor
                    JOIN reseau.stations s ON vtor.station_id = s.id
                    WHERE s.initiales = @Initiales";

                using var connection = _context.CreateConnection();
                var voiesTelemesurees = await connection.QueryAsync<VoiesTelemesurees>(queryTelemesurees, new { Initiales = initiales });
                var voiesTor = await connection.QueryAsync<VoiesTor>(queryTor, new { Initiales = initiales });

                return (voiesTelemesurees, voiesTor);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la récupération des voies pour la station avec les initiales {initiales}", ex);
            }
        }

        public async Task<(IEnumerable<VoiesTelemesurees> VoiesTelemesurees, IEnumerable<VoiesTor> VoiesTor)> GetVoiesByStationId(int stationId)
        {
            try
            {
                var queryTelemesurees = "SELECT * FROM reseau.voies_telemesurees WHERE station_id = @StationId";
                var queryTor = "SELECT * FROM reseau.voies_tor WHERE station_id = @StationId";

                using var connection = _context.CreateConnection();
                var voiesTelemesurees = await connection.QueryAsync<VoiesTelemesurees>(queryTelemesurees, new { StationId = stationId });
                var voiesTor = await connection.QueryAsync<VoiesTor>(queryTor, new { StationId = stationId });

                return (voiesTelemesurees, voiesTor);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la récupération des voies pour la station avec l'ID {stationId}", ex);
            }
        }

        public async Task<bool> CreateVoiesFromModel(int modelStationId, int newStationId, string newStationInitiales, IDbConnection connection, IDbTransaction transaction)
        {
            try
            {
                // _logger.LogInformation($"Début de la création des voies. ModelStationId: {modelStationId}, NewStationId: {newStationId}, NewStationInitiales: {newStationInitiales}");

                var modelVoiesTelemesurees = await GetVoiesTelesureesByStationId(modelStationId);
                var modelVoiesTor = await GetVoiesTorByStationId(modelStationId);

                // _logger.LogInformation($"Nombre de voies télémesurées trouvées: {modelVoiesTelemesurees.Count()}");
                // _logger.LogInformation($"Nombre de voies TOR trouvées: {modelVoiesTor.Count()}");

                foreach (var modelVoie in modelVoiesTelemesurees)
                {
                    var newVoie = new VoiesTelemesurees
                    {
                        AdresseBes = modelVoie.AdresseBes,
                        Delta = modelVoie.Delta,
                        Groupe = modelVoie.Groupe,
                        Info = modelVoie.Info,
                        Libelle = modelVoie.Libelle != null ? UpdateLibelle(modelVoie.Libelle, newStationInitiales) : "",
                        SeuilBas = modelVoie.SeuilBas,
                        SeuilHaut = modelVoie.SeuilHaut,
                        Priorite = modelVoie.Priorite,
                        StationId = newStationId,
                        Unite = modelVoie.Unite,
                        Numero = modelVoie.Numero,
                        Ordre = modelVoie.Ordre,
                        VoieEnregistree = modelVoie.VoieEnregistree,
                        VoieStockee = modelVoie.VoieStockee,
                        Actif = modelVoie.Actif,
                        Virgule = modelVoie.Virgule,
                        TraitementId = modelVoie.TraitementId,
                        Parametre1 = modelVoie.Parametre1,
                        Parametre2 = modelVoie.Parametre2,
                        Parametre3 = modelVoie.Parametre3,
                        Parametre4 = modelVoie.Parametre4,
                        Parametre5 = modelVoie.Parametre5,
                        Parametre6 = modelVoie.Parametre6,
                        Parametre7 = modelVoie.Parametre7,
                        Parametre8 = modelVoie.Parametre8,
                        Parametre9 = modelVoie.Parametre9,
                        Parametre10 = modelVoie.Parametre10,
                        Abonnements = modelVoie.Abonnements
                    };
                    var newId = await AddVoieTelemesuree(newVoie, connection, transaction);
                    // _logger.LogInformation($"Nouvelle voie télémesurée créée avec l'ID: {newId} et le libellé: {newVoie.Libelle}");
                }

                foreach (var modelVoie in modelVoiesTor)
                {
                    var newVoie = new VoiesTor
                    {
                        AdresseBes = modelVoie.AdresseBes,
                        Info = modelVoie.Info,
                        Libelle = modelVoie.Libelle != null ? UpdateLibelle(modelVoie.Libelle, newStationInitiales) : "",
                        Numero = modelVoie.Numero,
                        Ordre = modelVoie.Ordre,
                        StationId = newStationId,
                        Type = modelVoie.Type,
                        Actif = modelVoie.Actif
                    };
                    var newId = await AddVoieTor(newVoie, connection, transaction);
                    // _logger.LogInformation($"Nouvelle voie TOR créée avec l'ID: {newId} et le libellé: {newVoie.Libelle}");
                }

                // _logger.LogInformation($"Toutes les voies ont été créées avec succès. Nombre total: {modelVoiesTelemesurees.Count() + modelVoiesTor.Count()}");

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la création des voies à partir du modèle", ex);
            }
        }

        private string UpdateLibelle(string oldLibelle, string newInitiales)
        {
            try
            {
                var lastUnderscoreIndex = oldLibelle.LastIndexOf('_');
                if (lastUnderscoreIndex == -1)
                {
                    return newInitiales;
                }

                var prefix = oldLibelle.Substring(0, lastUnderscoreIndex + 1);
                return prefix + newInitiales;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la mise à jour du libellé: {oldLibelle}", ex);
            }
        }

        private async Task<IEnumerable<VoiesTelemesurees>> GetVoiesTelesureesByStationId(int stationId)
        {
            try
            {
                var query = "SELECT * FROM reseau.voies_telemesurees WHERE station_id = @StationId";
                using var connection = _context.CreateConnection();
                return await connection.QueryAsync<VoiesTelemesurees>(query, new { StationId = stationId });
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la récupération des voies télémesurées pour la station avec l'ID {stationId}", ex);
            }
        }

        private async Task<IEnumerable<VoiesTor>> GetVoiesTorByStationId(int stationId)
        {
            try
            {
                var query = "SELECT * FROM reseau.voies_tor WHERE station_id = @StationId";
                using var connection = _context.CreateConnection();
                return await connection.QueryAsync<VoiesTor>(query, new { StationId = stationId });
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la récupération des voies TOR pour la station avec l'ID {stationId}", ex);
            }
        }

        private async Task<int> AddVoieTelemesuree(VoiesTelemesurees voie, IDbConnection? connection = null, IDbTransaction? transaction = null)
        {
            try
            {
                var query = @"
                    INSERT INTO reseau.voies_telemesurees (adresse_bes, delta, groupe, info, libelle, seuil_bas, seuil_haut, priorite, station_id, unite, numero, ordre, voie_enregistree, voie_stockee, actif, virgule, parametre1, parametre2, parametre3, parametre4, parametre5, parametre6, parametre7, parametre8, parametre9, parametre10, traitement_id, abonnements)
                    VALUES (@AdresseBes, @Delta, @Groupe, @Info, @Libelle, @SeuilBas, @SeuilHaut, @Priorite, @StationId, @Unite, @Numero, @Ordre, @VoieEnregistree, @VoieStockee, @Actif, @Virgule, @Parametre1, @Parametre2, @Parametre3, @Parametre4, @Parametre5, @Parametre6, @Parametre7, @Parametre8, @Parametre9, @Parametre10, @TraitementId, @Abonnements)
                    RETURNING id";

                if (connection != null)
                {
                    return await connection.ExecuteScalarAsync<int>(query, voie, transaction);
                }

                using var conn = _context.CreateConnection();
                return await conn.ExecuteScalarAsync<int>(query, voie);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de l'ajout d'une voie télémesurée");
                throw;
            }
        }
        public async Task<int> AddVoieTelemesuree(VoiesTelemesurees voie)
        {
            try
            {
                return await AddVoieTelemesuree(voie, null, null);
            }
            catch (Exception ex)
            {
                throw new Exception("Erreur lors de l'ajout de la voie télémesurée", ex);
            }
        }

        private async Task<int> AddVoieTor(VoiesTor voie, IDbConnection? connection = null, IDbTransaction? transaction = null)
        {
            try
            {
                var query = @"
                    INSERT INTO reseau.voies_tor (adresse_bes, info, libelle, numero, ordre, station_id, type, actif)
                    VALUES (@AdresseBes, @Info, @Libelle, @Numero, @Ordre, @StationId, @Type, @Actif)
                    RETURNING id";

                if (connection != null)
                {
                    return await connection.ExecuteScalarAsync<int>(query, voie, transaction);
                }

                using var conn = _context.CreateConnection();
                return await conn.ExecuteScalarAsync<int>(query, voie);
            }
            catch (Exception ex)
            {
                throw new Exception("Erreur lors de l'ajout d'une voie TOR", ex);
            }
        }

        public async Task<int> AddVoieTor(VoiesTor voie)
        {
            try
            {
                return await AddVoieTor(voie, null, null);
            }
            catch (Exception ex)
            {
                throw new Exception("Erreur lors de l'ajout de la voie TOR", ex);
            }
        }

        public async Task<bool> UpdateVoieAbonnements(int voieId, int abonnements)
        {
            using var connection = _context.CreateConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();

            try
            {
                var updateAbonnementsQuery = @"
                    UPDATE reseau.voies_telemesurees
                    SET abonnements = @Abonnements
                    WHERE id = @VoieId";

                var affectedRows = await connection.ExecuteAsync(updateAbonnementsQuery,
                    new { Abonnements = abonnements, VoieId = voieId }, transaction);

                transaction.Commit();
                return affectedRows > 0;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                _logger.LogError(ex, "Erreur lors de la mise à jour des abonnements pour la voie {VoieId}", voieId);
                throw;
            }
        }

    }
}