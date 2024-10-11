using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SHUT.Core.Application.Interfaces;
using SHUT.Core.Domain;
using SHUT.Core.Domain.Common;
using SHUT.Core.Data;
using Dapper;

namespace SHUT.Core.Application
{
    // Service background qui s'initialise par la programmation d'appels
    // lance l'appel selon la programmation definie
    // orchestre le deroulement e l'appel
    public class OrchestationReseauService : BackgroundService
    {
        private readonly AppDbContext _context;
        private readonly IOrchestration _orchestrateur;
        private readonly ILogger _logger;
        private static List<CycleAppel> _caz = new List<CycleAppel>();
        private static CancellationToken _cancellationToken;
        public OrchestationReseauService(IOrchestration orchestrateur, ILogger<OrchestationReseauService> logger, AppDbContext context)
        {
            _orchestrateur = orchestrateur;
            _context = context;
            _logger = logger;
        }
        //Fonction a des fins de tests uniquement
        public async Task RunAsync(CancellationToken stoppingToken)
        {
            await ExecuteAsync(stoppingToken);
        }

        //fonction sexcutant des la creation du backgroud service
        //Sert donc a la recuperation des cycles d'appels et initialiser l'objet responsable de la synchronisation
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _cancellationToken = stoppingToken;
            _ = InitializeCyclesAppels(_cancellationToken);
            //await Task.Delay(100, stoppingToken);

        }


        public async Task InitializeCyclesAppels(CancellationToken stoppingToken)
        {
            string query = "Select * from CyclesAppel";
            using var connection = _context.CreateConnection();
            var cycles = await connection.QueryAsync<CycleAppel>(query);
            _caz.AddRange(cycles);
            await Task.CompletedTask;
        }



        private async Task<CycleAppel> GetCyclesAppel()
        {
            //var query =
            //    "INSERT INTO users (Nom, Passe, Groupe, Droits, Page1) VALUES (@Nom, @Passe, @Groupe, @Droits, @Page1)";
            //using var connection = _context.CreateConnection();
            _logger.LogInformation("getting cycles");
            await Task.Delay(100);

            HeureAppel hA = new HeureAppel { Heure = 17 };
            CycleAppel cA1 = new();
            cA1.StationsAAppeler[hA] = new List<Enregistreur>
            {
                new Enregistreur
                {
                    stations = new List<Station>
                    {
                        new Station {Nom = "AB"},
                        new Station {Nom = "AC"}
                    },
                    IPAdress = "192.168.10.106",
                    Port = 22,
                    Liasion = Liaison.Ftp,
                },
                new Enregistreur
                {
                    stations = new List<Station>
                    {
                        new Station {Nom = "AD"},
                        new Station {Nom = "AF"}
                    },
                    IPAdress = "192.168.10.28",
                    Port = 22,
                    Liasion = Liaison.Tcp,
                },
            };

            return cA1;
        }

        public override void Dispose()
        {
        }
    }

}
