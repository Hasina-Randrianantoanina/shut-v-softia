using AutomateOperations.Entities;
using AutomateOperations.Logger;
using Dapper;
using SHUT.Core.Data;
using SHUT.Core.Domain.Reseau;

namespace AutomateOperations.Utils
{
    public static class DataBaseUtils
    {
        public static async Task InsertError(AppDbContext appContext, FileLogger fileLogger, Erreur erreur, DateTime appel, int stationId)
        {
            string errorText = $"[{erreur.Code}] {erreur.Description}";
            fileLogger.Log(errorText);

            erreur.Appel = appel;
            erreur.StationId = stationId;
            try
            {
                string query = $"""
                INSERT INTO defaut.defauts_actifs (appel, code_erreur, description_defaut, station_id, type)
                VALUES (@Appel, @Code, @Description, @StationId, @Type)
                ;
                """;
                using var connection = appContext.CreateConnection();
                await connection.ExecuteAsync(query, erreur);
            }
            catch (Exception ex)
            {
                fileLogger.Log($"Exception levée lors de l'insertion d'une erreur code {erreur.Code} en base : {ex.GetType().Name}");
            }
        }

        public static async Task<(Station? station, Erreur? erreur)> GetStationFromStationReseau(StationsReseau stationReseau, AppDbContext appContext)
        {
            try
            {
                string query = $"""
                    SELECT
                    s.id,
                    s.initiales,
                    s.enregistreur_id,
                    e.id,
                    e.adresse_ip AS AdresseIP,
                    e.dernier_appel AS DernierAppel,
                    e.dernier_enregistrement AS DernierEnregistrement,
                    e.dernier_transfert AS DernierTransfert,
                    e.liaison AS TypeLiaison,
                    e.type_heure AS TypeHeure,
                    e.version AS Version
                    FROM reseau.stations AS s
                    INNER JOIN reseau.enregistreurs AS e
                    ON s.enregistreur_id = e.id
                    WHERE s.id = '{stationReseau.Id}' AND s.enregistreur_id NOTNULL
                """;
                using var connection = appContext.CreateConnection();
                List<Station> result = (List<Station>)await connection.QueryAsync<Station, Enregistreur, Station>(
                    query,
                    (station, enregistreur) =>
                    {
                        station.Enregistreur = enregistreur;
                        return station;
                    },
                    splitOn: "enregistreur_id");

                if (result.Count == 0) { return (null, ErrorUtils.HandleDatabaseError(800, null, stationReseau.Id.ToString())); }

                return (result[0], null);
            }
            catch (Exception ex)
            {
                return (null, ErrorUtils.HandleDatabaseError(801, ex, stationReseau.Id.ToString()));
            }
        }

        public static async Task<(List<VoieTelemesuree>? voiesTM, Erreur? erreur)> GetVoiesTelemesurees(AppDbContext appContext, int stationId)
        {
            try
            {
                string query = $"""
                SELECT
                id,
                adresse_bes AS AdresseBES,
                delta AS ValeurDelta,
                groupe,
                info,
                libelle,
                numero,
                seuil_bas AS SeuilBas,
                seuil_haut AS SeuilHaut,
                station_id AS StationId
                FROM reseau.voies_telemesurees
                WHERE station_id = {stationId}
                ORDER BY numero
                """;
                using var connection = appContext.CreateConnection();
                List<VoieTelemesuree> voiesTM = (List<VoieTelemesuree>)await connection.QueryAsync<VoieTelemesuree>(query);
                return (voiesTM, null);
            }
            catch (Exception ex)
            {
                return (null, ErrorUtils.HandleDatabaseError(802, ex, stationId.ToString()));
            }
        }

        public static async Task<(List<VoieTOR>? voiesTOR, Erreur? erreur)> GetVoiesTOR(AppDbContext appContext, int stationId)
        {
            try
            {
                string query = $"""
                SELECT
                id,
                libelle,
                numero,
                station_id AS StationId
                FROM reseau.voies_tor
                WHERE station_id = {stationId} AND type = 'TS'
                ORDER BY numero
                """;
                using var connection = appContext.CreateConnection();
                List<VoieTOR> voiesTOR = (List<VoieTOR>)await connection.QueryAsync<VoieTOR>(query);
                return (voiesTOR, null);
            }
            catch (Exception ex)
            {
                return (null, ErrorUtils.HandleDatabaseError(803, ex, stationId.ToString()));
            }
        }

        public static async Task<(bool schemaTableExists, Erreur? erreur)> EnsureSchemaTablesExists(ArchiveDbContext archiveContext, Station station)
        {
            // Schema
            var resultSchemaExists = await EnsureSchemaExists(archiveContext, station.Initiales!);
            if (!resultSchemaExists.schemaExists) { return (false, resultSchemaExists.erreur); }

            // Voies telemesurees
            foreach (VoieTelemesuree voieTM in station.VoiesTelemesurees)
            {
                var resultTableExists = await EnsureTableExists(archiveContext, station.Initiales!, voieTM.Libelle);
                if (!resultTableExists.tableExists) { return (false, resultTableExists.erreur); }
            }

            // Voies TOR
            foreach (VoieTOR voieTOR in station.VoiesTOR)
            {
                var resultTableExists = await EnsureTableExists(archiveContext, station.Initiales!, voieTOR.Libelle);
                if (!resultTableExists.tableExists) { return (false, resultTableExists.erreur); }
            }

            return (true, null);
        }

        private static async Task<(bool schemaExists, Erreur? erreur)> EnsureSchemaExists(ArchiveDbContext archiveContext, string initiales)
        {
            string query = $"CREATE SCHEMA IF NOT EXISTS {initiales.ToLower()}";
            try
            {
                using var connection = archiveContext.CreateConnection();
                await connection.ExecuteAsync(query);
                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, ErrorUtils.HandleDatabaseError(804, ex, initiales.ToLower()));
            }
        }

        private static async Task<(bool tableExists, Erreur? erreur)> EnsureTableExists(ArchiveDbContext archiveContext, string initiales, string nomVoie)
        {

            try
            {
                string query = $"CREATE TABLE IF NOT EXISTS {initiales.ToLower()}.{nomVoie.ToLower()} ";
                query += " (horodate TIMESTAMP WITHOUT TIME ZONE NOT NULL, evenement TEXT, mesure DOUBLE PRECISION, mesure_brute TEXT);";
                using var connection = archiveContext.CreateConnection();
                await connection.ExecuteAsync(query);
                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, ErrorUtils.HandleDatabaseError(805, ex, $"{initiales.ToLower()}.{nomVoie.ToLower()}"));
            }

        }

        public static async Task<(DateTime[]? lastTimestamps, Erreur? erreur)> GetLastTimestampTM(ArchiveDbContext archiveContext, Station station, string conditions = "")
        {
            DateTime[] lastTimestamps = new DateTime[32];
            int i = 0;
            try
            {
                using var connection = archiveContext.CreateConnection();
                for (i = 0; i < station.VoiesTelemesurees.Count; i++)
                {
                    VoieTelemesuree voie = station.VoiesTelemesurees[i];
                    if (voie.Numero > 32) { continue; }
                    string query = $"SELECT max(horodate) from {station.Initiales!.ToLower()}.{voie.Libelle.ToLower()} ";
                    query += conditions;
                    DateTime? lastTimestamp = await connection.QuerySingleOrDefaultAsync<DateTime?>(query);
                    lastTimestamps[voie.Numero -1] = lastTimestamp != null ? (DateTime)lastTimestamp : DateTime.MinValue;
                }
                return (lastTimestamps, null);
            }
            catch (Exception ex)
            {
                return (null, ErrorUtils.HandleDatabaseError(806, ex, $"{station.Initiales!.ToLower()}.{station.VoiesTelemesurees[i].Libelle.ToLower()}"));
            }
        }

        public static async Task<(DateTime[]? lastTimestamps, Erreur? erreur)> GetLastTimestampTOR(ArchiveDbContext archiveContext, Station station, string conditions = "")
        {
            DateTime[] lastTimestamps = new DateTime[272];
            int i = 0;
            try
            {
                using var connection = archiveContext.CreateConnection();
                for (i = 0; i < station.VoiesTelemesurees.Count; i++)
                {
                    VoieTOR voie = station.VoiesTOR[i];
                    string query = $"SELECT max(horodate) from {station.Initiales!.ToLower()}.{voie.Libelle.ToLower()} ";
                    query += conditions;
                    DateTime? lastTimestamp = await connection.QuerySingleOrDefaultAsync<DateTime?>(query);
                    lastTimestamps[voie.Numero -1] = lastTimestamp != null ? (DateTime)lastTimestamp : DateTime.MinValue;
                }
                return (lastTimestamps, null);
            }
            catch (Exception ex)
            {
                return (null, ErrorUtils.HandleDatabaseError(806, ex, $"{station.Initiales!.ToLower()}.{station.VoiesTOR[i].Libelle.ToLower()}"));
            }
        }

        public static async Task<(bool statusPersistMesures, Erreur? erreur)> PersistMesures(ArchiveDbContext archiveContext, Station station, List<Mesure> mesures, DateTime[] lastTimestampsTM, DateTime[] lastTimestampsTOR, FileLogger fileLogger)
        {
            try
            {
                // Voies Telemesurees
                foreach (VoieTelemesuree voie in station.VoiesTelemesurees)
                {
                    if (voie.Numero > 32) { continue; }
                    DateTime lastTimestamp = lastTimestampsTM[voie.Numero -1];
                    List<Mesure> mesuresVoies = mesures.Where(x => (x.NomVoieBase.Equals(voie) && lastTimestamp < x.Time)).OrderBy(x => x.Time).ToList();
                    // Risque perte données si même minute que la last?
                    if (mesuresVoies.Count == 0) { continue; }

                    string table = $"{station.Initiales!.ToLower()}.{voie.Libelle.ToLower()}";
                    string sql = $"INSERT INTO {table} (horodate, evenement, mesure) VALUES (@Time, @Evenement, @ValeurALEchelle)";

                    using var connection = archiveContext.CreateConnection();
                    int rowsAffected = mesuresVoies.Count() > 0 ? await connection.ExecuteAsync(sql, mesuresVoies) : 1;

                    if (rowsAffected > 0) 
                    {
                        fileLogger.Log($"Insertion de {mesuresVoies.Count} mesures dans la table {table} réussie");
                    }
                    else 
                    {
                        return (false, ErrorUtils.HandleDatabaseError(807, null, table));
                    }
                }

                // Voies TOR
                foreach (VoieTOR voie in station.VoiesTOR)
                {
                    DateTime lastTimestamp = lastTimestampsTOR[voie.Numero -1];
                    List<Mesure> mesuresVoies = mesures.Where(x => (x.NomVoieBase.Equals(voie) && lastTimestamp < x.Time)).OrderBy(x => x.Time).ToList();
                    // Risque perte données si même minute que la last?
                    if (mesuresVoies.Count == 0) { continue; }

                    string table = $"{station.Initiales!.ToLower()}.{voie.Libelle.ToLower()}";
                    string sql = $"INSERT INTO {table} (horodate, evenement, mesure) VALUES (@Time, @Evenement, @ValeurALEchelle)";

                    using var connection = archiveContext.CreateConnection();
                    int rowsAffected = mesuresVoies.Count() > 0 ? await connection.ExecuteAsync(sql, mesuresVoies) : 1;

                    if (rowsAffected > 0)
                    {
                        fileLogger.Log($"Insertion de {mesuresVoies.Count} mesures dans la table {table} réussie");
                    }
                    else
                    {
                        return (false, ErrorUtils.HandleDatabaseError(807, null, table));
                    }
                }

                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, ErrorUtils.HandleDatabaseError(808, ex));
            }
        }

        public static async Task<(List<Defaut>? defauts, Erreur? erreur)> GetActivesDefautsOfThisStation(AppDbContext appContext, List<VoieTelemesuree> voies)
        {
            try
            {
                int[] ids = voies.Select(v => v.Id).ToArray();

                string query = $"""
                    SELECT
                    id,
                    actif,
                    appel,
                    type,
                    debut AS DebutDefaut,
                    version_enregistreur AS VersionEnregistreur,
                    voie_telemesuree_id AS VoieTelemesureeId
                    FROM defaut.defauts
                    WHERE type = 'CAPTEUR' AND actif AND voie_telemesuree_id IN @ids
                    ORDER BY voie_telemesuree_id
                    """;
                using var connection = appContext.CreateConnection();
                List<Defaut> defauts = (List<Defaut>)await connection.QueryAsync<Defaut>(query, ids);

                return (defauts, null);
            }
            catch (Exception ex)
            {
                return (null, ErrorUtils.HandleDatabaseError(809, ex));
            }
        }

        public static async Task<(bool statusInsertActivesDefauts, Erreur? erreur)> InsertDefautsActifsOfThisStation(AppDbContext appContext, List<Defaut> activeDefautsAfterCall, List<VoieTelemesuree> voies, FileLogger fileLogger)
        {
            try
            {
                foreach (VoieTelemesuree voie in voies)
                {
                    bool delete = voie.DefautActif != null;
                    bool insert = false;
                    Defaut? defautToInsert = null;
                    string query = string.Empty;

                    foreach (Defaut defaut in activeDefautsAfterCall)
                    {
                        if (defaut.VoieTelemesureeId == voie.Id)
                        {
                            if (defaut.Id > 0) // Already actif before call => do nothing
                            {
                                delete = false;
                            }
                            else
                            {
                                defautToInsert = defaut;
                                insert = true;
                            }
                            break;
                        }
                        // If no defaut found => delete
                    }

                    if (!delete && !insert) { continue;  }
                    using var connection = appContext.CreateConnection();

                    if (delete)
                    {
                        query = $"""
                            DELETE FROM defaut.defauts_actifs
                            WHERE type = 'CAPTEUR' AND voie_telemesuree_id = {voie.Id}
                            """;

                        int rowsAffected = await connection.ExecuteAsync(query);
                        if (rowsAffected > 0)
                        {
                            fileLogger.Log($"Suppression du défaut actif de la voie id {voie.Id} réussi");
                        }
                        else
                        {
                            return (false, ErrorUtils.HandleDatabaseError(810, null, $"[Appel={voie.DefautActif!.Appel.ToString("dd/MM/yyyy HH:mm:ss")}, Type=CAPTEUR, VoieTelemesureeId = {voie.Id}]"));
                        }
                    }
                    if (insert)
                    {
                        query = $"""
                            INSERT INTO defaut.defauts_actifs (appel, type, voie_telemesuree_id)
                            VALUES (@Appel, @Type, @VoieTelemesureeId)
                            ;
                            """;
                        int rowsAffected = await connection.ExecuteAsync(query, defautToInsert);

                        if (rowsAffected > 0)
                        {
                            fileLogger.Log($"Insertion du défaut actif de la voie id {voie.Id} réussi");
                        }
                        else
                        {
                            return (false, ErrorUtils.HandleDatabaseError(811, null, $"[Appel={defautToInsert!.Appel.ToString("dd/MM/yyyy HH:mm:ss")}, Type=CAPTEUR, VoieTelemesureeId = {voie.Id}]"));
                        }
                    }
                }

                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, ErrorUtils.HandleDatabaseError(812, ex));
            }
        }

        public static async Task<(bool statusInsertDefauts, Erreur? erreur)> InsertAllDefautsOfThisStation(AppDbContext appContext, List<Defaut> defautsOfThisStation, List<VoieTelemesuree> voies, DateTime[] lastDefautsTimestampsTM, FileLogger fileLogger)
        {
            try
            {
                foreach (VoieTelemesuree voie in voies)
                {
                    if (voie.Numero > 32) { continue; }
                    DateTime lastTimestamp = lastDefautsTimestampsTM[voie.Numero -1];
                    List<Defaut> defautsOfThisVoie = defautsOfThisStation.Where(d => (d.VoieTelemesureeId == voie.Id  && lastTimestamp < d.TimeMesure)).ToList();
                    if (defautsOfThisVoie.Count == 0) { continue; }

                    using var connection = appContext.CreateConnection();
                    string query;
                    int rowsAffected;

                    // If there is active defaut before call, it's the first defaut in the list
                    if (voie.DefautActif != null)
                    {
                        // Update defaut actif
                        query = $"""
                        UPDATE defaut.defauts 
                        SET actif = @Actif, fin_defaut = @FinDefaut
                        WHERE id = @Id
                        ;
                        """;
                        rowsAffected = await connection.ExecuteAsync(query, defautsOfThisVoie[0]);
                        if (rowsAffected == 0)
                        {
                            return (false, ErrorUtils.HandleDatabaseError(813, null, $"[Id={defautsOfThisVoie[0]}Appel={defautsOfThisVoie[0]!.Appel.ToString("dd/MM/yyyy HH:mm:ss")}, Type=CAPTEUR, VoieTelemesureeId = {voie.Id}]"));
                        }
                        fileLogger.Log($"Mise à jour du défaut id {defautsOfThisVoie[0].Id} de la voie id {voie.Id} réussi");
                        defautsOfThisVoie.RemoveAt(0);
                        if (defautsOfThisVoie.Count == 0) { continue; }
                    }

                    // Insert other defauts
                    query = $"""
                        INSERT INTO defaut.defauts (actif, appel, type, debut_defaut, fin_defaut, version_enregistreur, voie_telemesuree_id)
                        VALUES (@Actif, @Appel, @Type, @DebutDefaut, @FinDefaut, @VersionEnregistreur, @VoieTelemesureeId)
                        ;
                        """;
                    rowsAffected = await connection.ExecuteAsync(query, defautsOfThisVoie);
                    if (rowsAffected == 0)
                    {
                        return (false, ErrorUtils.HandleDatabaseError(814, null, $"{voie.Id}"));
                    }
                    fileLogger.Log($"Insertion des défauts de la voie id {voie.Id} réussi");

                }

                return (true, null);

            }
            catch (Exception ex)
            {
                return (false, ErrorUtils.HandleDatabaseError(815, ex));
            }
        }

        public static async Task<(bool statusUpdateEnregistreur, Erreur? erreur)> UpdateEnregistreur(AppDbContext appContext, Enregistreur enregistreur)
        {
            try
            {
                string query = $"""
                    UPDATE reseau.enregistreurs
                    SET dernier_appel = @DernierAppel, dernier_transfert = @DernierTransfert, dernier_enregistrement = @DernierEnregistrement version = @Version
                    WHERE id = @Id;
                    """;

                using var connection = appContext.CreateConnection();
                int rowsAffected = await connection.ExecuteAsync(query, enregistreur);
                if (rowsAffected == 0)
                {
                    return (false, ErrorUtils.HandleDatabaseError(816));
                }

                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, ErrorUtils.HandleDatabaseError(817, ex));
            }
        }

    }
}
