
using StenOperations.Data;
using StenOperations.Logger;
using StenOperations.Models.Entities;
using System.Diagnostics;

namespace StenOperations.Models.Commands
{
    public class TESTPertesCommand: DefaultCommand
    {
        public PersistenceService Repository { get; set; }

        public TESTPertesCommand(ILogger logger, Station station, PersistenceService repository)
            :base(logger, station) 
        {
            Repository = repository;
        }

        public override async Task ExecuteAsync()
        {
            Context.Operation = "PUTREQ";

            var dhDebutOperation = DateTime.Now;
            var stopWatch = new Stopwatch();

            stopWatch.Start();
            // Traitement défauts capteurs en ligne pour une station donnée
            // -> Selection Defauts de la station avec NumDefaut=26 => TmpDatatable
            // -> Suppression Defauts de la station avec NumDefaut=26
            // -> Selection from PertesEnrg pour la station => Datatable LesPertes
            // -> for i =1 to 32 : vérifie voie interne et si TmpDatatable contient la voie
            // -> alors, Insert to Defauts from TmpDataTable
            await Repository.DefautsCapteursEnLigne(Station);
            stopWatch.Stop();
            Logger.Log($"Trait déf capteurs {(double)(stopWatch.ElapsedMilliseconds / 1000)}");

            stopWatch.Restart();
            // Traitement défauts etats en ligne pour une station donnée
            // -> Selection Defauts de la station avec NumDefaut=26 => TmpDatatabe
            // -> Suppression Defauts de la station avec NumDefaut=27
            // -> Selection PertesEnrg pour la station => Datatable LesPertes
            // -> for i = 1 to 16: traite voie etat
            // -> verifie VEtat[i].ETmodule et traite DefVoieEC (typeDef=10)
            //    si Perte dans LesPertes, inserer ou met à jour PertesEnrg
            // -> vérifie si VEtat[i].ETetat != 0
            //    si VEtat[i].Etlibel dans TmpDatatable,
            //    insérer dans Defauts la station
            await Repository.DefautsEtatsEnLigne(Station);
            stopWatch.Stop();
            Logger.Log($"Trait déf états {(double)(stopWatch.ElapsedMilliseconds / 1000)}");

            // Pertes d'enregistrement
            // -> Selection depuis EvtsPluvieux
            // -> Insertion dans PertesEnrg
            // -> Vérifie resultat Station.Perte_type
            // Station.Perte_type = 0, 1, 2, 3, 4, 5, 7, else
            stopWatch.Restart();
            int ae = await Repository.PertesEnrg(Station, Context.RepetitionsDates);
            Station.ArretEnrg = ae > 0;

            switch(ae)
            {
                case 0:
                    break;
                case 1:
                    ErrorText = $"{ae} pb dateGo";
                    break;
                case 2:
                    ErrorText = $"{ae} pb dateInit";
                    break;
                case 3:
                    ErrorText = $"{ae} pb dateAcq";
                    break;
                case 4:
                    ErrorText = $"{ae} pb dateEnrg";
                    break;
                case 5:
                    ErrorText = $"{ae} pb écart Horloge";
                    break;
                case 7:
                    ErrorText = $"{ae} pb dateGo et dateInit";
                    break;
                default:
                    break;
            };

            if (ae > 0)
            {
                if (Context.RepetitionsDates)
                {
                    if (ae == 1) // Pb DateGo
                    {
                        Context.DefautDateGo = true;
                        DateTime.TryParse(DateTime.Now.ToString("dd/MM/yyyy HH:mm:00"), out var now);
                        Context.DateFin = now;
                    }
                    else
                    {
                        ErrorCode = 18;
                        ErrorText = $"Détection perte sans rattrapage - {ErrorText}";
                        Logger.Log($"Erreur {ae}: {ErrorCode} - {ErrorText}");
                        Context.Operation = "FIN";
                    }
                }
                else
                {
                    // Insert defaut (CodeErr: 58), Détection perte ErrorText
                    // Then PUTLTR
                    await Repository.InsDefaut(Station, 58, $"Détection perte {ErrorText}");
                    Context.RepetitionsDates = true;
                    Context.Operation = "PUTLTR";
                }
            }
            else 
            {
                Context.RepetitionsDates = false;
                DateTime.TryParse(DateTime.Now.ToString("dd/MM/yyyy HH:mm:00"), out var dt);
                Context.DateFin = dt;
            }
            stopWatch.Stop();    
            Logger.Log($"Trait Détection de pertes {(double)(stopWatch.ElapsedMilliseconds / 1000)}");
        }
    }
}
