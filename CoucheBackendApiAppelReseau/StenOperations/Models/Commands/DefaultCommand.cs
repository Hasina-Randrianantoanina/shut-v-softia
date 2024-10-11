
using StenOperations.Logger;
using StenOperations.Models.Entities;

namespace StenOperations.Models.Commands
{
    public class DefaultCommand: ICommand, ICommandAsync
    {
        public ILogger Logger { get; set; }
        public int ErrorCode { get; protected set; } = 0;
        public string ErrorText { get; protected set; } = "";
        public Station Station { get; set; }
        public CommandContext Context { get; set; } = new CommandContext();

        public DefaultCommand(ILogger logger, Station station)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            Station = station ?? throw new ArgumentNullException(nameof(station));
        }

        public virtual void Execute() { }

        public virtual async Task ExecuteAsync() 
        {
            Execute();
        }
    }
}
