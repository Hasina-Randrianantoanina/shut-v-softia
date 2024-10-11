using SHUT.Core.Data;
using SHUT.Core.Domain.Reseau;
using StenOperations.Data;
using StenOperations.Logger;
using StenOperations.Models.Commands;
using StenOperations.Models.Entities;
using StenOperations.Utils;

namespace StenOperations
{
    public class Sten
    {
        private Station station { get; set; }
        private StationsReseau StationsReseau { get; set; }
        public ILogger Logger { get; set; }

        public string PathFile { get; set; }
        private PersistenceService persistenceService { get; set; }

        public AppDbContext AppDbContext { get; set; }
        public ArchiveDbContext ArchiveDbContext { get; set; }

        public Sten(
            StationsReseau stationReseau,
            AppDbContext appContext,
            ArchiveDbContext archiveContext,
            string pathFile
        )
        {
            StationsReseau = stationReseau;
            ArchiveDbContext = archiveContext;
            AppDbContext = appContext;
            PathFile = pathFile;
            //station = AutomateMapper.MapStationReseauToStation(stationReseau, appContext);
        }

        public async Task<(bool success, string errorMessage)> Execute()
        {
            try
            {
                //var fullPath = $"{PathFile}/{StationsReseau.Initiales}";
                FileDirectoryUtils.TryCreateDirectoryIfNotExist(new ConsoleLogger(), PathFile);
                string logFile =
                    $"{PathFile}/{StationsReseau.Initiales}_{DateTime.Now.ToString("ddMMyyyyHHmmss")}.txt";
                Logger = new FileAndConsoleLogger(
                    logFile,
                    () =>
                    {
                        return $"{StationsReseau.Initiales} {DateTime.Now.ToString("HH:mm:ss.fff")}";
                    }
                );
                persistenceService = new PersistenceService(Logger, AppDbContext, ArchiveDbContext);

                station = await persistenceService.GetStation(StationsReseau.Initiales);
                //station.FtpAdresse = "127.0.0.1";
                station.FtpUserName = "sten";
                station.FtpPassword = "dea";
                station.DateDerTrfBase = DateTime.Now.AddDays(-1);
                var context = new CommandContext()
                {
                    Operation = "COM",
                    DateDebut = station.DateDerTrfBase, // Date de debut plage de transfert = date du dernier transfert
                    RootDirPath = PathFile,
                };
                var seq = new SequenceAppelIP5Command(Logger, station, persistenceService)
                {
                    Context = context,
                };
                await seq.ExecuteAsync();
            }
            catch (Exception ex)
            {
                Logger.Log(ex.ExceptionStackTraces());
                return (false, $"Une erreur est survenue : {ex.Message}");
            }
            return (true, "Exécution réussie");
        }
    }
}
