
using SHUT.Core.Domain.Reseau;
using StenOperations.Data;
using StenOperations.Helpers;
using StenOperations.Logger;
using StenOperations.Models.Entities;
using System.Diagnostics;

namespace StenOperations.Models.Commands
{
    public class RESULTCommand: DefaultCommand
    {
        public PersistenceService Repository { get; set; }

        public RESULTCommand(ILogger logger, Station station, PersistenceService repository)
        :base(logger, station) 
        {
            Repository = repository;
        }

        public override void Execute()
        {
            //ProcessBinToMan();
            ProcessBinToObj();
        }


        public override async Task ExecuteAsync()
        {
            //await ProcessBinToManAsync();            
            await ProcessBinToObjAsync();
        }

        #region ProcessBinToObj/ProcessBinToObjAsync
        private void ProcessBinToObj()
        {
            Context.Operation = "FERMER";
            if (Context.FicBinRecu && ErrorCode == 0)
            {
                //var nomFicTrf = $"{Context.RootDirPath}/Result_{Station.Initiales.Trim()}.bin";
                // DecodageBinEtGenMan
                var decodageBinCmd = new DecodageBinCommand(Logger, Station) { Context = Context };
                decodageBinCmd.Execute();
                Context = decodageBinCmd.Context;

                // DecodageFichierBinaire
                var stenBinHelper = new StenBinHelper();
                var mesures = stenBinHelper.GetMesures(Context.NomFicBin, Station, Context.DateDebut);

                if (mesures != null)
                {
                    // Sauvegarde des mesures
                    Repository.SaveMesures(mesures);

                    //
                    //
                    //
                    //
                    //
                    //
                    //
                    //
                    //
                    //
                    //Traite Defauts Fichier Source (fichier man)
                    var stopWatch = new Stopwatch();
                    stopWatch.Start();

                    var defFicHelper = new DefautFichierHelper(Logger, Station, Repository);
                    var nbdef = defFicHelper.TraiterDefautFichierSource(Context.NomFicMan);
                    // Gestion AlerteDefauts > 300
                    if (nbdef.Result > 300)
                    {
                        Repository.GestionAlerteDefauts1121(Station);
                    }

                    stopWatch.Stop();
                    Logger.Log($"Trait défauts fic: {(double)(stopWatch.ElapsedMilliseconds / 1000)}s");

                    // Inserer Fichier
                    stopWatch.Restart();
                    var insererFichier = new InsererFichierHelper(Logger);
                    var res = insererFichier.ErrConcatenation(Context.NomFicMan);
                    if (res > 0)
                    {
                        ErrorCode = insererFichier.ErrorCode;
                        ErrorText = insererFichier.ErrorText;
                    }
                    stopWatch.Stop();
                    Logger.Log($"Insertion {(double)(stopWatch.ElapsedMilliseconds / 1000)}s");
                    
                    // Normalement cela doit continuer ici
                    Context.DateDebut = Context.DateDebut.AddDays(1);
                    if (Context.DateDebut > Context.DateFin)
                    {
                        Station.DateDerTrfBase = Context.DateFin;
                        Logger.Log("Fin Transfert IP");
                        Logger.Log($"Operation: {Context.Operation}");
                        if (Context.DefautDateGo)
                        {
                            ErrorCode = 18;
                            ErrorText = $"Détection perte avec rattrapage - {ErrorText}";
                            Logger.Log($"Erreur: {ErrorCode} - {ErrorText}");
                            Context.Operation = "FIN";
                        }
                    }
                    else
                    {
                        Station.DateDerTrfBase = Context.DateDebut;
                        Logger.Log($"Trf {Context.DateDebut.ToShortDateString()}");
                        Context.Operation = "PUTREQ";
                    }
                }
                else
                {
                    Logger.Log($"Erreur fic bin : {Station.Initiales}");
                    Context.Repetitions++;
                    Context.Operation = "PUTREQ";
                }
            }
            else
            {
                if (ErrorCode == 0)
                {
                    ErrorCode = 2004;
                    ErrorText = "fichier result.bin non reçu";
                }
            }
        }

        private async Task ProcessBinToObjAsync()
        {
            Context.Operation = "FERMER";
            if (Context.FicBinRecu && ErrorCode == 0)
            {
                //var nomFicTrf = $"{Context.RootDirPath}/Result_{Station.Initiales.Trim()}.bin";
                // DecodageBinEtGenMan
                var decodageBinCmd = new DecodageBinCommand(Logger, Station) { Context = Context };
                decodageBinCmd.Execute();
                Context = decodageBinCmd.Context;
                // DecodageFichierBinGenObj
                var stenBinHelper = new StenBinHelper();
                var mesures = stenBinHelper.GetMesures(Context.NomFicBin, Station, Context.DateDebut);

                if (mesures != null)
                {
                    var stopWatch = new Stopwatch();
                    // Sauvegarde des mesures
                    stopWatch.Start();    
                    var result = await Repository.SaveMesuresAsync(mesures);
                    Logger.Log($"Sauvegarde mesures: {(double)(stopWatch.ElapsedMilliseconds / 1000)}s");
                    stopWatch.Stop();

                    // Traite Defauts Fichier Source (fichier man)
                    stopWatch.Restart();

                    var defFicHelper = new DefautFichierHelper(Logger, Station, Repository);
                    var nbdef = defFicHelper.TraiterDefautFichierSource(Context.NomFicMan);
                    // Gestion AlerteDefauts > 300
                    if (nbdef.Result > 300)
                    {
                        Repository.GestionAlerteDefauts1121(Station);
                    }

                    stopWatch.Stop();
                    Logger.Log($"Trait défauts fic: {(double)(stopWatch.ElapsedMilliseconds / 1000)}s");

                    // Inserer Fichier
                    stopWatch.Restart();
                    var insererFichier = new InsererFichierHelper(Logger);
                    var res = insererFichier.ErrConcatenation(Context.NomFicMan);
                    if (res > 0)
                    {
                        ErrorCode = insererFichier.ErrorCode;
                        ErrorText = insererFichier.ErrorText;
                    }
                    stopWatch.Stop();
                    Logger.Log($"Insertion {(double)(stopWatch.ElapsedMilliseconds / 1000)}s");
                    
                    // Normalement cela doit continuer ici
                    Context.DateDebut = Context.DateDebut.AddDays(1);
                    if (Context.DateDebut > Context.DateFin)
                    {
                        Station.DateDerTrfBase = Context.DateFin;
                        Logger.Log("Fin Transfert IP");
                        Logger.Log($"Operation: {Context.Operation}");
                        if (Context.DefautDateGo)
                        {
                            ErrorCode = 18;
                            ErrorText = $"Détection perte avec rattrapage - {ErrorText}";
                            Logger.Log($"Erreur: {ErrorCode} - {ErrorText}");
                            Context.Operation = "FIN";
                        }
                    }
                    else
                    {
                        Station.DateDerTrfBase = Context.DateDebut;
                        Logger.Log($"Trf {Context.DateDebut.ToShortDateString()}");
                        Context.Operation = "PUTREQ";
                    }
                }
                else
                {
                    Logger.Log($"Erreur fic bin : {Station.Initiales}");
                    Context.Repetitions++;
                    Context.Operation = "PUTREQ";
                }
            }
            else
            {
                if (ErrorCode == 0)
                {
                    ErrorCode = 2004;
                    ErrorText = "fichier result.bin non reçu";
                }
            }
        }
        #endregion

        #region ProcesssBinToMan/ProcessBinToManAsync
        private void ProcessBinToMan()
        {
            Context.Operation = "FERMER";
            if (Context.FicBinRecu && ErrorCode == 0)
            {
                //var nomFicTrf = $"Result_{Station.Initiales.Trim()}.bin";
                // DecodageFichierBinaire
                var decodageBinCmd = new DecodageBinCommand(Logger, Station) { Context = Context };
                decodageBinCmd.Execute();
                Context = decodageBinCmd.Context;

                if (decodageBinCmd.ErrorCode == 0)
                {
                    // Traite Defauts Fichier Source (fichier man)
                    var stopWatch = new Stopwatch();
                    stopWatch.Start();

                    var defFicHelper = new DefautFichierHelper(Logger, Station, Repository);
                    var nbdef = defFicHelper.TraiterDefautFichierSource(Context.NomFicMan);
                    // Gestion AlerteDefauts > 300
                    if (nbdef.Result > 300)
                    {
                        Repository.GestionAlerteDefauts1121(Station);
                    }

                    stopWatch.Stop();
                    Logger.Log($"Trait défauts fic: {(double)(stopWatch.ElapsedMilliseconds / 1000)}s");

                    // Inserer Fichier
                    stopWatch.Restart();
                    var insererFichier = new InsererFichierHelper(Logger);
                    var res = insererFichier.ErrConcatenation(Context.NomFicMan);
                    if (res > 0)
                    {
                        ErrorCode = insererFichier.ErrorCode;
                        ErrorText = insererFichier.ErrorText;
                    }
                    stopWatch.Stop();
                    Logger.Log($"Insertion {(double)(stopWatch.ElapsedMilliseconds / 1000)}s");

                    // Normalement cela doit continuer ici
                    Context.DateDebut = Context.DateDebut.AddDays(1);
                    if (Context.DateDebut > Context.DateFin)
                    {
                        Station.DateDerTrfBase = Context.DateFin;
                        Logger.Log("Fin Transfert IP");
                        Logger.Log($"Operation: {Context.Operation}");
                        if (Context.DefautDateGo)
                        {
                            ErrorCode = 18;
                            ErrorText = $"Détection perte avec rattrapage - {ErrorText}";
                            Logger.Log($"Erreur: {ErrorCode} - {ErrorText}");
                            Context.Operation = "FIN";
                        }
                    }
                    else
                    {
                        Station.DateDerTrfBase = Context.DateDebut;
                        Logger.Log($"Trf {Context.DateDebut.ToShortDateString()}");
                        Context.Operation = "PUTREQ";
                    }
                }
                else
                {
                    Logger.Log($"Erreur fic bin : {Station.Initiales}");
                    Context.Repetitions++;
                    Context.Operation = "PUTREQ";
                }
            }
            else
            {
                if (ErrorCode == 0)
                {
                    ErrorCode = 2004;
                    ErrorText = "fichier result.bin non reçu";
                }
            }
        }

        private async Task ProcessBinToManAsync()
        {
            Context.Operation = "FERMER";
            if (Context.FicBinRecu && ErrorCode == 0) 
            {
                //var nomFicTrf = $"Result_{Station.Initiales.Trim()}.bin";
                // DecodageFichierBinaire
                var decodageBinCmd = new DecodageBinCommand(Logger, Station) { Context = Context };
                decodageBinCmd.Execute();
                Context = decodageBinCmd.Context;

                if (decodageBinCmd.ErrorCode == 0) 
                {
                    // Traite Defauts Fichier Source (fichier man)
                    var stopWatch = new Stopwatch();
                    stopWatch.Start();

                    var defFicHelper = new DefautFichierHelper(Logger, Station, Repository);
                    var nbdef = await defFicHelper.TraiterDefautFichierSource(Context.NomFicMan);
                    // Gestion AlerteDefauts > 300
                    if (nbdef > 300)
                    {
                        Repository.GestionAlerteDefauts1121(Station);
                    }

                    stopWatch.Stop();
                    Logger.Log($"Trait défauts fic: {(double)(stopWatch.ElapsedMilliseconds / 1000)}s");

                    // Inserer Fichier
                    stopWatch.Restart();
                    var insererFichier = new InsererFichierHelper(Logger);
                    var res = insererFichier.ErrConcatenation(Context.NomFicMan);
                    if (res > 0) 
                    {
                        ErrorCode = insererFichier.ErrorCode;
                        ErrorText = insererFichier.ErrorText;
                    }
                    stopWatch.Stop();
                    Logger.Log($"Insertion {(double)(stopWatch.ElapsedMilliseconds / 1000)}s");

                    // Normalement cela doit continuer ici
                    Context.DateDebut = Context.DateDebut.AddDays(1);
                    if (Context.DateDebut > Context.DateFin)
                    {
                        Station.DateDerTrfBase = Context.DateFin;
                        Logger.Log("Fin Transfert IP");
                        Logger.Log($"Operation: {Context.Operation}");
                        if (Context.DefautDateGo)
                        {
                            ErrorCode = 18;
                            ErrorText = $"Détection perte avec rattrapage - {ErrorText}";
                            Logger.Log($"Erreur: {ErrorCode} - {ErrorText}");
                            Context.Operation = "FIN";
                        }
                    }
                    else
                    {
                        Station.DateDerTrfBase = Context.DateDebut;
                        Logger.Log($"Trf {Context.DateDebut.ToShortDateString()}");
                        Context.Operation = "PUTREQ";
                    }
                }
                else
                {
                    Logger.Log($"Erreur fic bin : {Station.Initiales}");
                    Context.Repetitions++;
                    Context.Operation = "PUTREQ";
                }
            }
            else
            {
                if (ErrorCode == 0) 
                {
                    ErrorCode = 2004;
                    ErrorText = "fichier result.bin non reçu";
                }
            }
        }
        #endregion
    }
}
