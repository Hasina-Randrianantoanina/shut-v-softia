
using StenOperations.Data;
using StenOperations.Logger;
using StenOperations.Models.Entities;

namespace StenOperations.Models.Commands
{
    public class SequenceAppelIP5Command: DefaultCommand
    {
        private Dictionary<string, DefaultCommand> _cmds = new Dictionary<string, DefaultCommand>();

        public PersistenceService Repository { get; set; }

        public SequenceAppelIP5Command(ILogger logger, Station station, PersistenceService repository)
            :base(logger, station) 
        {
            Context = new CommandContext() { Operation = "COM" };
            Repository = repository;
            // InitCommands must be called after setting Repository
            InitCommands();
        }

        public SequenceAppelIP5Command(ILogger logger, Station station, PersistenceService repository, CommandContext context)
            :this(logger, station, repository)
        {
            Context = context;
        }

        private void InitCommands()
        {
            _cmds["COM"] = new COMCommand(Logger, Station.FtpAdresse);
            _cmds["GETSTATUS"] = new GETSTATUSCommand(Logger, Station, Repository);
            _cmds["GETCFG"] = new GETCFGCommand(Logger, Station);
            _cmds["GETLTR"] = new GETLTRCommand(Logger, Station);
            _cmds["GETRAZ"] = new GETRAZCommand(Logger, Station);
            _cmds["GETREQ"] = new GETREQCommand(Logger, Station);
            _cmds["PUTCFG"] = new PUTCFGCommand(Logger, Station);
            _cmds["PUTLTR"] = new PUTLTRCommand(Logger, Station);
            _cmds["PUTRAZ"] = new PUTRAZCommand(Logger, Station);
            _cmds["PUTREQ"] = new PUTREQCommand(Logger, Station);
            _cmds["RESULT"] = new RESULTCommand(Logger, Station, Repository);
            _cmds["TEST_Pertes"] = new TESTPertesCommand(Logger, Station, Repository);
        }

        //public override void Execute()
        //{
        //    var op = Context.Operation;
        //    var iteration = 0;
        //    // En cas de boucle infinie, la boucle s'arrète forcément
        //    // après 10 iterations
        //    while (op != "FIN" && iteration < 20)
        //    {
        //        if (op == "FERMER")
        //        {
        //            break;
        //        }
        //        if (_cmds.ContainsKey(op))
        //        {
        //            Logger.Log($"-> Command: {op}");
        //            //Thread.Sleep(100);
        //            var cmd = _cmds[op];
        //            cmd.Context = Context;
        //            cmd.Station = Station;
        //            cmd.Execute();

        //            // Affiche l'erreur en cours
        //            Context = cmd.Context;
        //            Station = cmd.Station;
        //            if (cmd.ErrorCode != 0) 
        //            {
        //                Logger.Log($"{cmd.ErrorCode}: {cmd.ErrorText}");
        //                // On reteste l'opération une 2ème fois pour voir
        //                // si cela passe ou pas. Si cela ne passe pas,
        //                // on arrête l'exécution
        //                //iteration = (iteration < 8) ? 8 : iteration + 1;
        //            }
  
        //            // Prochaine opération
        //            op = string.IsNullOrEmpty(Context.Operation) ? "FIN" : Context.Operation;
        //            if (Context.Repetitions >= 3)
        //            {
        //                Context.Operation = "RESULT";
        //            }
        //            if (Context.Repetitions > 0)
        //            {
        //                //Thread.Sleep(3000);
        //            }

        //            Logger.Log($"{new string('-', 40)}");
        //            Logger.Log($"Operation  : {Context.Operation}");
        //            Logger.Log($"Repetitions: {Context.Repetitions}");
        //            Logger.Log($"JourCourant: {Context.JourCourant}");
        //            Logger.Log($"FicBinRecu : {Context.FicBinRecu}");
        //            Logger.Log($"Date Début: {Context.DateDebut.ToShortDateString()}");
        //            Logger.Log($"Date Fin  : {Context.DateFin.ToShortDateString()}");
        //            Logger.Log($"Iteration : {iteration}");
        //            Logger.Log($"{new string('-', 80)}");
        //        }
        //        else
        //        {
        //            Logger.Log("Command not found: {0}", op);
        //            op = "FERMER";
        //        }
        //        iteration++;
        //    }
        //    Logger.Log($"Fin de l'operation! {iteration} iterations.");
        //}

        public override async Task ExecuteAsync()
        {
            var op = Context.Operation;
            var iteration = 0;
            // En cas de boucle infinie, la boucle s'arrète forcément
            // après 10 iterations
            while (op != "FIN" && iteration < 20)
            {
                if (op == "FERMER")
                {
                    break;
                }
                if (_cmds.ContainsKey(op))
                {
                    Logger.Log($"-> AsyncCommand: {op}");
                    var cmd = _cmds[op];
                    cmd.Context = Context;
                    cmd.Station = Station;
                    await cmd.ExecuteAsync();

                    // Affiche l'erreur en cours
                    Context = cmd.Context;
                    Station = cmd.Station;
                    if (cmd.ErrorCode != 0)
                    {
                        Logger.Log($"{cmd.ErrorCode}: {cmd.ErrorText}");
                        // On reteste l'opération une 2ème fois pour voir
                        // si cela passe ou pas. Si cela ne passe pas,
                        // on arrête l'exécution
                        //iteration = (iteration < 8) ? 8 : iteration + 1;
                    }

                    // Prochaine opération
                    op = string.IsNullOrEmpty(Context.Operation) ? "FIN" : Context.Operation;
                    if (Context.Repetitions >= 3)
                    {
                        Context.Operation = "RESULT";
                    }

                    Logger.Log($"{new string('-', 40)}");
                    Logger.Log($"Operation  : {Context.Operation}");
                    Logger.Log($"Repetitions: {Context.Repetitions}");
                    Logger.Log($"JourCourant: {Context.JourCourant}");
                    Logger.Log($"FicBinRecu : {Context.FicBinRecu}");
                    Logger.Log($"Date Début: {Context.DateDebut.ToShortDateString()}");
                    Logger.Log($"Date Fin  : {Context.DateFin.ToShortDateString()}");
                    Logger.Log($"Iteration : {iteration}");
                    Logger.Log($"{new string('-', 80)}");
                }
                else
                {
                    Logger.Log("Command not found: {0}", op);
                    op = "FERMER";
                }
                iteration++;
            }
            Logger.Log($"Fin de l'operation! {iteration} iterations.");
        }
    }
}
