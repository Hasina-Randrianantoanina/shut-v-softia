using IsodaqOperations;
using SHUT.Core.Data;
using SHUT.Core.Domain.Reseau;

namespace ReseauDEAOperations.Models
{
    public class ISODAQHandler : ReseauHandler
    {
        // public async Task<bool> HandleCommunication(StationsReseau station, AppDbContext appContext, ArchiveDbContext archiveContext)
        // {
        //     throw new NotImplementedException();
        // }
        private Isodaq isodaq { get; set; } = null!;

        public async Task<(bool success, string errorMessage)> HandleCommunication(
            StationsReseau station,
            AppDbContext appContext,
            ArchiveDbContext archiveContext,
            string PathEchangeFiles
        )
        {
            isodaq = new Isodaq(station, appContext, archiveContext, PathEchangeFiles);
            return await isodaq.Execute();

            // public async Task<bool> HandleCommunication(StationsReseau station, AppDbContext appContext, ArchiveDbContext archiveContext, string PathEchangeFiles)
            // {
            //     isodaq = new Isodaq(station, appContext, archiveContext, PathEchangeFiles);
            //     return await isodaq.Execute();
        }
    }
}
