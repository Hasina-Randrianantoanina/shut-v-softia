using AutomateOperations;
using SHUT.Core.Data;
using SHUT.Core.Domain.Reseau;

namespace ReseauDEAOperations.Models
{
    public class AutomateHandler : ReseauHandler
    {
        private Automate automate { get; set; } = null!;

        public async Task<(bool success, string errorMessage)> HandleCommunication(
            StationsReseau station,
            AppDbContext appContext,
            ArchiveDbContext archiveContext,
            string PathEchangeFiles
        )
        {
            try
            {
                automate = new Automate(station, appContext, archiveContext, PathEchangeFiles);
                var (success, errorMessage) = await automate.Execute();
                return (success, errorMessage);
            }
            catch (Exception ex)
            {
                return (
                    false,
                    $"{ex.ToString()}"
                );
            }

        // public async Task<bool> HandleCommunication(StationsReseau station, AppDbContext appContext, ArchiveDbContext archiveContext, string PathEchangeFiles)
        // {
        //     automate = new Automate(station, appContext, archiveContext, PathEchangeFiles);
        //     return await automate.Execute();

        }
    }
}
