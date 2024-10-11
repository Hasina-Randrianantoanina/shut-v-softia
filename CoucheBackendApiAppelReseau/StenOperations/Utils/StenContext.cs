

using SHUT.Core.Data;
using StenOperations.Logger;

namespace StenOperations.Utils
{
    public class StenContext
    {
        public ILogger Logger { get; set; }
        public ArchiveDbContext ArchiveDbContext { get; set; }
        public AppDbContext AppDbContext { get; set; }
        public StenContext(ILogger logger, ArchiveDbContext archiveDbContext, AppDbContext appDbContext)
        {
            Logger = logger;
            ArchiveDbContext = archiveDbContext;
            AppDbContext = appDbContext;
        }
    }
}
