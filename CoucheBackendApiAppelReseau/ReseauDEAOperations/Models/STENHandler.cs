using SHUT.Core.Data;
using SHUT.Core.Domain.Reseau;
using StenOperations;

namespace ReseauDEAOperations.Models
{
    public class STENHandler : ReseauHandler
    {
        private Sten sten { get; set; } = null!;

        public async Task<(bool success, string errorMessage)> HandleCommunication(
            StationsReseau station, 
            AppDbContext appContext, 
            ArchiveDbContext archiveContext, 
            string pathFile)
        {
            sten = new Sten(station, appContext, archiveContext, pathFile);
            return await sten.Execute();
        }
    }
}