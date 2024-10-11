using System.Diagnostics;
using System.Net.Sockets;
using AutomateOperations.AutomateProtocol;
using AutomateOperations.Entities;
using AutomateOperations.Entities.Infos;
using AutomateOperations.Logger;
using AutomateOperations.Utils;
using SHUT.Core.Data;
using SHUT.Core.Domain.Reseau;

namespace AutomateOperations
{
    public class Automate
    {
        public AppDbContext AppDbContext { get; set; }
        public ArchiveDbContext ArchiveDbContext { get; set; }
        private Station _station { get; set; }
        private StationsReseau _stationReseau { get; set; }
        private FileLogger _fileLogger { get; set; }
        private string _pathToDataFilesDirectories { get; set; }

        public Automate(StationsReseau stationReseau, AppDbContext appContext, ArchiveDbContext archiveContext, string pathToDataFilesDirectories)
        {
            ArchiveDbContext = archiveContext;
            AppDbContext = appContext;
            _station = new();
            _stationReseau = stationReseau;
            _fileLogger = new(string.Empty);
            _pathToDataFilesDirectories = pathToDataFilesDirectories;
        }

        public async Task<(bool success, string errorMessage)> Execute()
        {
            const bool PremiumGetDatas = true; // Laisser false si Premium de prod tant que l'ancien Shutweb tourne
            const bool WriteMesureInLog = true;
            const bool WriteDefautsInLog = true;

            // --------------------------------------------------------------

            // TODO : log/mdp FTP dans Appsettings
            string downloadDirectory = @$"{_pathToDataFilesDirectories}/Downloads/"; // Download from M580
            string binaryFileDirectory = @$"{_pathToDataFilesDirectories}/Binaries/"; // Binary file created by copy

            try
            {
                Directory.CreateDirectory(downloadDirectory);
                Directory.CreateDirectory(binaryFileDirectory);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Impossible de créér les dossiers {downloadDirectory} et {binaryFileDirectory}");
                Console.WriteLine(ex.ToString());
                return (false, "Impossible de créér les dossiers");
            }

            const int PortTCP = 502;
            DateTime appelLocale = DateTime.Now; // Name of the files
            DateTime appelUTC = appelLocale.ToUniversalTime();
            Stopwatch automateOperationClock = Stopwatch.StartNew();
            Stopwatch mainClock = Stopwatch.StartNew();
            Erreur? erreurUpdateEnregistreur = null;

            if (downloadDirectory.Last() != '/') { downloadDirectory += '/'; }
            if (binaryFileDirectory.Last() != '/') { binaryFileDirectory += '/'; }
            string fileLoggerName = $@"{downloadDirectory}log_{_stationReseau.Initiales}_{appelLocale.ToString("ddMMyyyy'_'HHmmss")}.txt";
            _fileLogger = new FileLogger(fileLoggerName, () => { return DateTime.Now.ToString("HH:mm:ss.fff"); });

            // -------------------------------------------------------------
            // ----------- First : Retrieve Station from database ----------
            // -------------------------------------------------------------

            // Get station
            var resultGetStation = await DataBaseUtils.GetStationFromStationReseau(_stationReseau, AppDbContext);
            if (resultGetStation.station == null)
            {
                await DataBaseUtils.InsertError(AppDbContext, _fileLogger, resultGetStation.erreur!, appelUTC, _stationReseau.Id);
                return (false, resultGetStation.erreur!.Description);
            }
            else if (!resultGetStation.station.Enregistreur.TypeHeure.Equals("LOCALE") && !resultGetStation.station.Enregistreur.TypeHeure.Equals("UNIVERSELLE")) 
            {
                Erreur erreur = ErrorUtils.HandleStatusError(412, null, _station.Enregistreur.TypeHeure);
                await DataBaseUtils.InsertError(AppDbContext, _fileLogger, erreur, appelUTC, _station.Id);
                return (false, erreur.Description);
            }
            _station = resultGetStation.station;

            // Get Voies Telemesurees
            var resultGetVoiesTM = await DataBaseUtils.GetVoiesTelemesurees(AppDbContext, _station.Id);
            if (resultGetVoiesTM.voiesTM == null)
            {
                await DataBaseUtils.InsertError(AppDbContext, _fileLogger, resultGetVoiesTM.erreur!, appelUTC, _station.Id);
                return (false, resultGetVoiesTM.erreur!.Description);
            }
            _station.VoiesTelemesurees = resultGetVoiesTM.voiesTM;

            // Get Voies TOR
            var resultGetVoiesTOR = await DataBaseUtils.GetVoiesTOR(AppDbContext, _station.Id);
            if (resultGetVoiesTOR.voiesTOR == null)
            {
                await DataBaseUtils.InsertError(AppDbContext, _fileLogger, resultGetVoiesTOR.erreur!, appelUTC, _station.Id);
                return (false, resultGetVoiesTOR.erreur!.Description);
            }
            _station.VoiesTOR = resultGetVoiesTOR.voiesTOR;

            try
            {
                _fileLogger.Log($"Début appel station {_station}");
                _station.Enregistreur.DernierAppel = appelUTC;

                // Ping the station
                var resultPing = await PingUtils.TryPingStation(_station.Enregistreur.AdresseIP, _fileLogger);
                if (!resultPing.statusPing)
                {
                    await DataBaseUtils.InsertError(AppDbContext, _fileLogger, resultPing.erreur!, appelUTC, _station.Id);
                    return (false, resultPing.erreur!.Description);
                }

                // -------------------------------------------------------------
                // ----------- Second : Get binary file from station -----------
                // -------------------------------------------------------------

                // Connexion to the TCP server
                using TcpClient client = new TcpClient();
                await client.ConnectAsync(_station.Enregistreur.AdresseIP!, PortTCP);
                NetworkStream stream = client.GetStream();

                _fileLogger.Log($"Connexion TCP réussie à {_station.Enregistreur.AdresseIP}:{PortTCP}");

                // -------------- Automate Shared Protocol -------------

                SharedAutomateProtocol sharedAutomateProtocol = new SharedAutomateProtocol(_station, stream, _fileLogger);

                // Question n°1 : Are there available datas ?
                var resultDatasAvailability = await sharedAutomateProtocol.AreDatasAvailable();
                if (!resultDatasAvailability.statusDatasAvailability)
                {
                    if (resultDatasAvailability.erreur != null)
                    {
                        await DataBaseUtils.InsertError(AppDbContext, _fileLogger, resultDatasAvailability.erreur, appelUTC, _station.Id);
                        return (false, resultDatasAvailability.erreur.Description);
                    }
                    else
                    {
                        mainClock.Stop();
                        _fileLogger.Log($"Pas de données, fin appel station {_station.Initiales} sans erreur ({mainClock.ElapsedMilliseconds / 1000.0}s)");
                        return (true, null);
                    }
                }
                _fileLogger.Log($"Données disponibles");

                // Question n°2 : LECT CONFIG_VER
                var resultGetVersion = await sharedAutomateProtocol.TryReadVersion();
                if (!resultGetVersion.statusVersion)
                {
                    await DataBaseUtils.InsertError(AppDbContext, _fileLogger, resultGetVersion.erreur!, appelUTC, _station.Id);
                    return (false, resultGetVersion.erreur!.Description);
                }
                _fileLogger.Log($"Version = {_station.Enregistreur.Version}");

                // Instanciate Specific Automate Protocol
                ISpecificAutomateProtocol specificAutomateProtocol = sharedAutomateProtocol.GetSpecificAutomateProtocol();

                if (specificAutomateProtocol == null)
                {
                    Erreur erreur = ErrorUtils.HandleStatusError(411, null, _station.Enregistreur.Version);
                    await DataBaseUtils.InsertError(AppDbContext, _fileLogger, erreur, appelUTC, _station.Id);
                    return (false, erreur.Description);
                }

                bool premiumProtocol = specificAutomateProtocol is AutomatePremiumProtocol;
                specificAutomateProtocol = premiumProtocol ? (AutomatePremiumProtocol)specificAutomateProtocol : (AutomateM580Protocol)specificAutomateProtocol;

                // -------------- Specific Protocol ---------------

                // Question n°3 : LECT CONFIG_ANA
                var resultAnalogicConfig = await specificAutomateProtocol.TryRead60AnalogicConfig();
                if (!resultAnalogicConfig.statusConfig)
                {
                    await DataBaseUtils.InsertError(AppDbContext, _fileLogger, resultAnalogicConfig.erreur!, appelUTC, _station.Id);
                    return (false, resultAnalogicConfig.erreur!.Description);
                }

                // Question n°4 : LECT DATA
                if (premiumProtocol && (!PremiumGetDatas || !_station.Initiales!.Equals("EN_test")))
                {
                    mainClock.Stop();
                    _fileLogger.Log($"Fin appel (LECT_DATA désactivé) station {_station.Initiales} sans erreur ({mainClock.ElapsedMilliseconds / 1000.0}s)");
                    return (true, null);
                }

                var resultGetDatas = await specificAutomateProtocol.TryGetDatas(downloadDirectory);
                if (!resultGetDatas.statusGetDatas)
                {
                    await DataBaseUtils.InsertError(AppDbContext, _fileLogger, resultGetDatas.erreur!, appelUTC, _station.Id);
                    return (false, resultGetDatas.erreur!.Description);
                }
                _station.Enregistreur.DernierTransfert= appelUTC;

                // Closing the connexion as soon as possible
                client.Dispose();

                // Exit without error
                mainClock.Stop();
                _fileLogger.Log($"Fin appel station {_station.Initiales} sans erreur ({mainClock.ElapsedMilliseconds / 1000.0}s)");

                // -------------------------------------------------------------
                // -------------------- Third : bin to POCO  -------------------
                // -------------------------------------------------------------

                _fileLogger.Log($"LECT MESURES");
                mainClock = Stopwatch.StartNew();

                // Input files
                var resultSourceFiles = FileUtils.GetSourceFilesShortNames(downloadDirectory, _station.Initiales!);
                if (resultSourceFiles.names == null)
                {
                    await DataBaseUtils.InsertError(AppDbContext, _fileLogger, resultSourceFiles.erreur!, appelUTC, _station.Id);
                    return (false, resultSourceFiles.erreur!.Description);
                }
                string[] sourceFilesShortNames = resultSourceFiles.names;

                int successfullReadingFiles = 0;
                List<Mesure> mesuresOfThisStation = new List<Mesure>();

                for (int i = 0; i < sourceFilesShortNames.Length; i++)
                {
                    string sourceFillFullName = @$"{downloadDirectory}{sourceFilesShortNames[i]}";
                    string binaryFileShortName = $"{_station.Initiales}_{appelLocale.ToString("ddMMyyyy'_'HHmmss")}.bin";
                    string binaryFileFullName = @$"{binaryFileDirectory}{binaryFileShortName}";

                    // Step n°1 : copying the file
                    var resultCopy = FileUtils.TryCopyFile(sourceFillFullName, binaryFileFullName);
                    if (!resultCopy.statusCopy)
                    {
                        await DataBaseUtils.InsertError(AppDbContext, _fileLogger, resultCopy.erreur!, appelUTC, _station.Id);
                        return (false, resultCopy.erreur!.Description); // Stop because we need all the files to create the defauts
                    }

                    _fileLogger.Log($"Début lecture des mesures depuis le fichier {binaryFileShortName} créé");

                    // Step n°2 : reading the datas in the binary file
                    Stopwatch secondClock = Stopwatch.StartNew();

                    // Creating the list of mesures of this file
                    List<Mesure> mesuresOfThisFile;
                    var resultReadMesures = specificAutomateProtocol.ReadMesuresInBinaryFile(binaryFileFullName);
                    if (resultReadMesures.mesures == null)
                    {
                        await DataBaseUtils.InsertError(AppDbContext, _fileLogger, resultReadMesures.erreur!, appelUTC, _station.Id);
                        return (false, resultReadMesures.erreur!.Description); // Stop because we need all the files to create the defauts
                    }
                    mesuresOfThisFile = resultReadMesures.mesures;

                    secondClock.Stop();
                    _fileLogger.Log($"Lecture de {mesuresOfThisFile.Count} données depuis {binaryFileShortName} en {secondClock.ElapsedMilliseconds / 1000.0}s");
                    if (WriteMesureInLog) { _fileLogger.LogAll(PrintUtils.GetMesures(mesuresOfThisFile)); }

                    // Adding the mesures of this file to the big list
                    mesuresOfThisStation.AddRange(mesuresOfThisFile);
                    successfullReadingFiles++;
                } // End For loop

                mainClock.Stop();
                _fileLogger.Log($"Fin lecture des mesures de {_station.Initiales} sans erreur ({mainClock.ElapsedMilliseconds / 1000.0}s) : {successfullReadingFiles}/{sourceFilesShortNames.Length} fichiers lus");

                if (mesuresOfThisStation.Count > 0)
                {
                    _station.Enregistreur.DernierEnregistrement = mesuresOfThisStation.Last().Time.ToUniversalTime();

                    // Set libelles
                    foreach (Mesure mesure in mesuresOfThisStation)
                    {
                        mesure.NomVoieBase = mesure.GetNomVoieBase(_station);
                        if (mesure.NomVoieBase == null)
                        {
                            string mesureStr = $"[Time={mesure.Time.ToString("dd/MM/yyyy HH:mm:ss")}, NumeroVoie={mesure.NumeroVoie.ToString("00")}, Info={ConversionUtils.IntToStringHexa4(mesure.Info.Value)}]";
                            _fileLogger.Log($"mesure avec voie inconnue {mesureStr}");
                        }
                    }

                    // -------------------------------------------------------------
                    // ----------------- Fourth : Persist Mesures ------------------
                    // -------------------------------------------------------------

                    _fileLogger.Log($"INSERTION MESURES");
                    mainClock = Stopwatch.StartNew();

                    // Ensure schema and tables exists in db mesure
                    var resultSchemaTableExists = await DataBaseUtils.EnsureSchemaTablesExists(ArchiveDbContext, _station);
                    if (!resultSchemaTableExists.schemaTableExists)
                    {
                        await DataBaseUtils.InsertError(AppDbContext, _fileLogger, resultSchemaTableExists.erreur!, appelUTC, _station.Id);
                        return (false, resultSchemaTableExists.erreur!.Description);
                    }

                    // Convert Mesures Locale => UTC if premium
                    if (_station.Enregistreur.TypeHeure.Equals("LOCALE"))
                    {
                        int print100 = 0;
                        foreach (Mesure m in mesuresOfThisStation)
                        {
                            DateTime time = m.Time;
                            m.Time = m.Time.ToUniversalTime();
                            if (print100++ % 100 == 0) { Console.WriteLine($"--- Convert {time.ToString()} => {m.Time.ToString()}"); }
                        }
                    }

                    // Before insertion : get last timestamp with defaut for each voie
                    // Last timestamps voies telemesurees
                    var resultGetLastTimestampTM = await DataBaseUtils.GetLastTimestampTM(ArchiveDbContext, _station);
                    if (resultGetLastTimestampTM.lastTimestamps == null)
                    {
                        await DataBaseUtils.InsertError(AppDbContext, _fileLogger, resultGetLastTimestampTM.erreur!, appelUTC, _station.Id);
                        return (false, resultGetLastTimestampTM.erreur!.Description);
                    }
                    DateTime[] lastTimestampTM = resultGetLastTimestampTM.lastTimestamps;

                    // Last timestamps voies TOR
                    var resultGetLastTimestampTOR = await DataBaseUtils.GetLastTimestampTOR(ArchiveDbContext, _station);
                    if (resultGetLastTimestampTOR.lastTimestamps == null)
                    {
                        await DataBaseUtils.InsertError(AppDbContext, _fileLogger, resultGetLastTimestampTOR.erreur!, appelUTC, _station.Id);
                        return (false, resultGetLastTimestampTOR.erreur!.Description);
                    }
                    DateTime[] lastTimestampTOR = resultGetLastTimestampTOR.lastTimestamps;
                    
                    // Last defauts timestamps voies telemesurees
                    var resultGetLastDefautsTimestampTM = await DataBaseUtils.GetLastTimestampTM(ArchiveDbContext, _station, "WHERE evenement = '$'");
                    if (resultGetLastDefautsTimestampTM.lastTimestamps == null)
                    {
                        await DataBaseUtils.InsertError(AppDbContext, _fileLogger, resultGetLastDefautsTimestampTM.erreur!, appelUTC, _station.Id);
                        return (false, resultGetLastDefautsTimestampTM.erreur!.Description);
                    }
                    DateTime[] lastDefautsTimestampTM = resultGetLastDefautsTimestampTM.lastTimestamps;

                    // Persist Mesures
                    var resultPersist = await DataBaseUtils.PersistMesures(ArchiveDbContext, _station, mesuresOfThisStation, lastTimestampTM, lastTimestampTOR, _fileLogger);
                    if (!resultPersist.statusPersistMesures)
                    {
                        if (resultPersist.erreur != null)
                        {
                            await DataBaseUtils.InsertError(AppDbContext, _fileLogger, resultPersist.erreur, appelUTC, _station.Id);
                        }
                        // TODO message si resultPersist.statusPersistMesures == false
                        return (false, resultPersist.erreur!.Description);
                    }

                    mainClock.Stop();
                    _fileLogger.Log($"Fin insertion de {mesuresOfThisStation.Count} mesures dans la base de données en {mainClock.ElapsedMilliseconds / 1000.0}s");

                    // -------------------------------------------------------------
                    // ----------------- Fifth : Handling defauts -----------------
                    // -------------------------------------------------------------
                    _fileLogger.Log("GESTION DEFAUTS");
                    mainClock = Stopwatch.StartNew();

                    // Not mesures TOR nor EnregistrementSurDefautGroupe
                    List<Mesure> mesuresWithDefauts = mesuresOfThisStation.Where(m =>
                            {
                                return (
                                        m.Info is InfoAnalogicCalcul && (((InfoAnalogicCalcul)m.Info).DebutDefaut || ((InfoAnalogicCalcul)m.Info).FinDefaut))
                                    || (m.Info is InfoAnalogicCapteur && (((InfoAnalogicCapteur)m.Info).DebutDefaut || ((InfoAnalogicCapteur)m.Info).FinDefaut));
                            }).ToList();

                    // Step 1 : get active defauts in defaut.defauts
                    var resultGetDefautsActifs = await DataBaseUtils.GetActivesDefautsOfThisStation(AppDbContext, _station.VoiesTelemesurees);
                    if (resultGetDefautsActifs.defauts == null)
                    {
                        await DataBaseUtils.InsertError(AppDbContext, _fileLogger, resultGetDefautsActifs.erreur!, appelUTC, _station.Id);
                        return (false, resultGetDefautsActifs.erreur!.Description);
                    }
                    DefautUtils.SetDefautsActifs(_station.VoiesTelemesurees, resultGetDefautsActifs.defauts);

                    // Step 2 : calculate the defauts of this station
                    var resultCalculateDefautsOfthisStation = DefautUtils.CalculateDefautsOfThisStation(mesuresWithDefauts, _station.VoiesTelemesurees, _station.Enregistreur.Version!, appelUTC);
                    if (resultCalculateDefautsOfthisStation.defautsOfThisStation == null)
                    {
                        await DataBaseUtils.InsertError(AppDbContext, _fileLogger, resultCalculateDefautsOfthisStation.erreur!, appelUTC, _station.Id);
                        return (false, resultCalculateDefautsOfthisStation.erreur!.Description);
                    }
                    List<Defaut> defautsOfThisStation = resultCalculateDefautsOfthisStation.defautsOfThisStation;
                    List<Defaut> activeDefautsAfterCall = resultCalculateDefautsOfthisStation.defautsActifs!;

                    // Step 3 : update defaut.defauts_actifs
                    var resultInsertDefautsActifs =
                        await DataBaseUtils.InsertDefautsActifsOfThisStation(AppDbContext, activeDefautsAfterCall, _station.VoiesTelemesurees, _fileLogger);
                    if (!resultInsertDefautsActifs.statusInsertActivesDefauts)
                    {
                        await DataBaseUtils.InsertError(AppDbContext, _fileLogger, resultInsertDefautsActifs.erreur!, appelUTC, _station.Id);
                        return (false, resultInsertDefautsActifs.erreur!.Description);
                    }
                    _fileLogger.Log(
                        $"Fin insertion/suppression des défauts actifs dans la table defaut.defauts_actifs"
                    );

                    // Step 4 : insert defaut.defauts in database
                    if (defautsOfThisStation.Count > 0)
                    {
                        var resultInsertAllDefauts =
                            await DataBaseUtils.InsertAllDefautsOfThisStation(AppDbContext, defautsOfThisStation, _station.VoiesTelemesurees, lastDefautsTimestampTM, _fileLogger);
                        if (!resultInsertAllDefauts.statusInsertDefauts)
                        {
                            await DataBaseUtils.InsertError(AppDbContext, _fileLogger, resultInsertAllDefauts.erreur!, appelUTC, _station.Id);
                            return (false, resultInsertAllDefauts.erreur!.Description);
                        }
                        _fileLogger.Log($"Fin insertion de {defautsOfThisStation.Count} défauts dans la table defaut.defauts");
                        if (WriteDefautsInLog) { _fileLogger.LogAll(PrintUtils.GetDefauts(defautsOfThisStation)); }
                    }
                    else { _fileLogger.Log($"Aucun défaut à insérer dans la table defaut.defauts"); }

                    mainClock.Stop();
                    _fileLogger.Log($"Fin gestion défauts des voies telemesurees : {activeDefautsAfterCall.Count}/{_station.VoiesTelemesurees.Count} voies telemesurees avec défaut actif ({mainClock.ElapsedMilliseconds / 1000.0}s)");
                }
                else
                {
                    _fileLogger.Log($"Annulation insertion mesures et gestion defauts de {_station.Initiales} sans erreur : aucune mesure dans les fichiers");
                }
            }
            catch (Exception ex)
            {
                Erreur erreur;

                if (ex is SocketException)
                {
                    erreur = ErrorUtils.HandleTCPError(200, ex, $"{_station.Enregistreur.AdresseIP}:{PortTCP}");
                }
                else if (ex is IOException)
                {
                    erreur = ErrorUtils.HandleFileSystemError(1, ex, ex.Message);
                }
                else
                {
                    erreur = ErrorUtils.HandleUnknowError(1000, ex);
                }
                await DataBaseUtils.InsertError(AppDbContext, _fileLogger, erreur, appelUTC, _stationReseau.Id);
                return (false, erreur.Description);
            }
            finally
            {
                // Last step : update enregistreur
                var resultUpdateEnregistreur = await DataBaseUtils.UpdateEnregistreur(AppDbContext, _station.Enregistreur);
                if (!resultUpdateEnregistreur.statusUpdateEnregistreur)
                {
                    await DataBaseUtils.InsertError(AppDbContext, _fileLogger, resultUpdateEnregistreur.erreur!, appelUTC, _station.Id);
                    erreurUpdateEnregistreur = resultUpdateEnregistreur.erreur!;
                }
                else
                {
                    _fileLogger.Log($"Mise à jour de l'enregistreur en base effectuée)");

                    automateOperationClock.Stop();
                    _fileLogger.Log($"Fin du traitement de la station {_station.Initiales} en {automateOperationClock.ElapsedMilliseconds / 1000.0}s");
                }
            }

            if (erreurUpdateEnregistreur == null) { return (true, null); }
            else { return (false, erreurUpdateEnregistreur.Description); }

        }
    }
}
