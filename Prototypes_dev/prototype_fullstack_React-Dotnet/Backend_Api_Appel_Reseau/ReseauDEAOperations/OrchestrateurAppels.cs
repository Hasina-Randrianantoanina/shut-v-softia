using SHUT.Core.Application.Interfaces;
using SHUT.Core.Domain.Common;
namespace ReseauDEAOperations;

//1. Recupere la data des differents stations du cycle
//2. Rapatrie la data et lance une analyse detetant les defauts et pertes
//3. consigne des logs et des erreurs
//4. enregistre les resultats
public class OrchestrateurAppels : IOrchestration
{
    private static List<CycleAppel> EnregistreursCycleAppel = new List<CycleAppel>();
    private static Timer? _timer = null;
    
    public async Task getSequenceAppels(CancellationToken cancellationToken)
    {
        //Recuperer la bonne data de la database
        //verifier que le timer se declenche au bon moment
        await Task.Delay(1000, cancellationToken);
        System.Console.WriteLine("Test");
        //await EnregistreursCycleAppel.Select(x => async => {
        //    x.StationsAAppeler
        //});
        



    }

    public Task InitializeSequence(List<CycleAppel> cycles, CancellationToken cancellationToken)
    {
        EnregistreursCycleAppel = cycles;
        DateTime start = DateTime.Now;
        
        return Task.CompletedTask;
    }
}

