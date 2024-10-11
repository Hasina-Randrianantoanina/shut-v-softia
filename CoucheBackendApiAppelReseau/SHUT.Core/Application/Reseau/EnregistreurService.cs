using SHUT.Core.Data;
using Dapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
using SHUT.Core.Domain.Reseau;
using Microsoft.Extensions.Logging;

namespace SHUT.Core.Application.Reseau
{
    public class EnregistreurService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<EnregistreurService> _logger;

        public EnregistreurService(AppDbContext context, ILogger<EnregistreurService> logger)
        {
            _context = context;
            _logger = logger;
        }

        private Enregistreurs ConvertDatesToLocal(Enregistreurs enregistreur)
        {
            enregistreur.DateMaj = enregistreur.DateMaj?.ToLocalTime();
            enregistreur.DernierTransfert = enregistreur.DernierTransfert?.ToLocalTime();
            return enregistreur;
        }

        private IEnumerable<Enregistreurs> ConvertDatesToLocal(IEnumerable<Enregistreurs> enregistreurs)
        {
            foreach (var enregistreur in enregistreurs)
            {
                ConvertDatesToLocal(enregistreur);
            }
            return enregistreurs;
        }

        public async Task<IEnumerable<Enregistreurs>> GetEnregistreurs()
        {
            try
            {
                var query = @"SELECT id, mise_a_jour AS DateMaj, version, adresse_ip AS AdresseIp, liaison, 
                          dernier_transfert AS DernierTransfert, dernier_appel AS DernierAppel, 
                          dernier_enregistrement AS DernierEnregistrement, pourcentage_memoire AS PourcentageMemoire, 
                          type_heure AS TypeHeure
                          FROM reseau.enregistreurs";
                using var connection = _context.CreateConnection();
                var result = await connection.QueryAsync<Enregistreurs>(query);
                return ConvertDatesToLocal(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération de tous les enregistreurs");
                throw;
            }
        }

        public async Task<Enregistreurs?> GetEnregistreurById(int id)
        {
            try
            {
                var query = @"SELECT id, mise_a_jour AS DateMaj, version, adresse_ip AS AdresseIp, liaison, 
                          dernier_transfert AS DernierTransfert, dernier_appel AS DernierAppel, 
                          dernier_enregistrement AS DernierEnregistrement, pourcentage_memoire AS PourcentageMemoire, 
                          type_heure AS TypeHeure
                          FROM reseau.enregistreurs WHERE id = @Id";
                using var connection = _context.CreateConnection();
                var result = await connection.QuerySingleOrDefaultAsync<Enregistreurs>(query, new { Id = id });
                return result != null ? ConvertDatesToLocal(result) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération de l'enregistreur avec l'ID {Id}", id);
                throw;
            }
        }

        public async Task<Enregistreurs?> GetEnregistreurByAdresseIp(string adresseIp, IDbConnection? connection = null, IDbTransaction? transaction = null)
        {
            try
            {
                var query = @"
                    SELECT id, mise_a_jour AS DateMaj, version, adresse_ip AS AdresseIp, liaison, dernier_transfert AS DernierTransfert , pourcentage_memoire AS PourcentageMemoire, type_heure AS TypeHeure
                    FROM reseau.enregistreurs 
                    WHERE adresse_ip = @AdresseIp
                    LIMIT 1";

                Enregistreurs? result;
                if (connection != null)
                {
                    result = await connection.QueryFirstOrDefaultAsync<Enregistreurs>(query, new { AdresseIp = adresseIp }, transaction);
                }
                else
                {
                    using var conn = _context.CreateConnection();
                    result = await conn.QueryFirstOrDefaultAsync<Enregistreurs>(query, new { AdresseIp = adresseIp });
                }
                return result != null ? ConvertDatesToLocal(result) : null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la récupération de l'enregistreur avec l'adresse IP {adresseIp}", ex);
            }
        }

        public async Task<int> AddEnregistreur(Enregistreurs enregistreur, IDbConnection? connection = null, IDbTransaction? transaction = null)
        {
            try
            {
                var query = @"
                INSERT INTO reseau.enregistreurs (mise_a_jour, version, adresse_ip, liaison, 
                dernier_transfert, dernier_appel, dernier_enregistrement, pourcentage_memoire, type_heure)
                VALUES (@DateMaj, @Version, @AdresseIp, @Liaison, @DernierTransfert, @DernierAppel, 
                @DernierEnregistrement, @PourcentageMemoire, @TypeHeure)
                RETURNING id";

                if (connection != null && transaction != null)
                {
                    return await connection.ExecuteScalarAsync<int>(query, enregistreur, transaction);
                }
                else
                {
                    using var conn = _context.CreateConnection();
                    return await conn.ExecuteScalarAsync<int>(query, enregistreur);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de l'ajout d'un nouvel enregistreur");
                throw;
            }
        }

        public async Task<bool> UpdateEnregistreur(Enregistreurs enregistreur)
        {
            try
            {
                var query = @"
                UPDATE reseau.enregistreurs
                SET mise_a_jour = @DateMaj, 
                    version = @Version,
                    adresse_ip = @AdresseIp,
                    liaison = @Liaison,
                    dernier_transfert = @DernierTransfert,
                    dernier_appel = @DernierAppel,
                    dernier_enregistrement = @DernierEnregistrement,
                    pourcentage_memoire = @PourcentageMemoire,
                    type_heure = @TypeHeure
                WHERE id = @Id";
                using var connection = _context.CreateConnection();
                var affectedRows = await connection.ExecuteAsync(query, enregistreur);
                return affectedRows > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la mise à jour de l'enregistreur avec l'ID {Id}", enregistreur.Id);
                throw;
            }
        }

        public async Task<bool> DeleteEnregistreur(int id)
        {
            try
            {
                var query = "DELETE FROM reseau.enregistreurs WHERE id = @Id";
                using var connection = _context.CreateConnection();
                var affectedRows = await connection.ExecuteAsync(query, new { Id = id });
                return affectedRows > 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la suppression de l'enregistreur avec l'ID {id}", ex);
            }
        }

        public async Task<IEnumerable<string>> GetEnregistreurVersionsByLiaison(string liaison)
        {
            try
            {
                var query = @"
                    SELECT DISTINCT version
                    FROM reseau.enregistreurs
                    WHERE liaison = @Liaison
                    ORDER BY version";

                using var connection = _context.CreateConnection();
                return await connection.QueryAsync<string>(query, new { Liaison = liaison });
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la récupération des versions d'enregistreurs pour la liaison {liaison}", ex);
            }
        }

        public async Task<bool> UpdateDernierTransfert(int id, DateTime dernierTransfert)
        {
            try
            {
                var query = @"
                    UPDATE reseau.enregistreurs
                    SET dernier_transfert = @DernierTransfert
                    WHERE id = @Id";

                using var connection = _context.CreateConnection();
                var affectedRows = await connection.ExecuteAsync(query, new { Id = id, DernierTransfert = dernierTransfert });

                return affectedRows > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la mise à jour de la date de dernier transfert pour l'enregistreur {Id}", id);
                throw;
            }
        }

    }
}