using SHUT.Core.Data;
using SHUT.Core.Domain.Reseau;

namespace ReseauDEAOperations.Models
{
    public interface ReseauHandler
    {
        Task<(bool success, string errorMessage)> HandleCommunication(
            StationsReseau station,
            AppDbContext appContext,
            ArchiveDbContext archiveContext,
            string PathEchangeFiles
        );
        // public Task<bool> HandleCommunication(StationsReseau station, AppDbContext appContext, ArchiveDbContext archiveContext, string PathEchangeFiles);

    }
}
