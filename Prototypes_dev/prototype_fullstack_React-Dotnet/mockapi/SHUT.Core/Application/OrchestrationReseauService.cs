using Microsoft.Extensions.Hosting;
using SHUT.Core.Application.Interfaces;

namespace SHUT.Core.Application
{
    public class OrchestationReseauService : BackgroundService
    {
        //plan d'exec : cycle appel

        //liste enregisteur a appeler a chaque cycle

        //séquencement : 
        //Appel de tous les enregistreurs du cycle
        //Recuperation Data
        //Enregistrement
        private readonly IMyDependency _myDependency;

        public OrchestationReseauService(IMyDependency myDependency)
        {
            _myDependency = myDependency;
        }
        public async Task RunAsync(CancellationToken stoppingToken)
        {
            await ExecuteAsync(stoppingToken);
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            //stoppingToken.Register(() => Console.WriteLine("MyHostedService is stopping."));

            while (!stoppingToken.IsCancellationRequested)
            {
                //Console.WriteLine("MyHostedService is doing background work.");
                //_myDependency.DoWork();

                await Task.Delay(TimeSpan.FromSeconds(60), stoppingToken); // Adjust the delay as needed.
            }

            //Console.WriteLine("MyHostedService has stopped.");
        }
    }

}
