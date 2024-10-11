using System;
using ReseauDEAOperations.Models;
using SHUT.Core.Application.Interfaces;
using SHUT.Core.Domain.Reseau;
using SHUT.Core.Data;
using Dapper;

namespace ReseauDEAOperations;

//1. Recupere la data des differents stations a appeler
//2. Rapatrie la data et  analyse les defauts et pertes
//3. consigne des logs et des erreurs
//4. enregistre les resultats dans les bases respectives
public class OrchestrateurAppels : IOrchestrateur
{
    //private static Timer? _timer = null;
    //private List<StationsReseau> _stationAAppler { get; set; } = new();
    private AppDbContext _appDbContext { get; set; }
    private ArchiveDbContext _archiveDbContext { get; set; }


    public const string PathForExchangeDirectory = "/var/www/shut/NetworkExchangeData";

    public OrchestrateurAppels(AppDbContext Appcontext, ArchiveDbContext ArchiveContext)
    {
        _appDbContext = Appcontext;
        _archiveDbContext = ArchiveContext;
    }

    public async Task<(bool success, string message)> AppelerStations(List<StationsReseau> stations)
    {
        foreach (var station in stations)
        {

            try
            {
                ReseauHandler reseauHandler = GetTypeAppelPourStation(station, _appDbContext);
            string test = PathForExchangeDirectory;
            string JourAppel = DateTime.Now.ToString("yyyy-MM-dd");
            string HeureAppel = $"{DateTime.Now.Hour.ToString("00")}H00";
            string stationInitiales = station.Initiales;
            string path = Path.Combine(test, JourAppel, HeureAppel, stationInitiales);
                var (success, message) = await reseauHandler.HandleCommunication(
                    station,
                    _appDbContext,
                    _archiveDbContext,
                    path
                );
                if (!success)
                {
                    return (false, $"Échec pour la station {station.Initiales} : {message}");
                }
            }
            catch (Exception ex)
            {
                return (false, $"Erreur pour la station {station.Initiales} : {ex.Message}");
            }
        }
        return (true, "Appel des stations réussi");
    }

    public static ReseauHandler GetTypeAppelPourStation(
        StationsReseau station,
        AppDbContext context
    )
    {
        string query = $"SELECT e.liaison FROM reseau.enregistreurs as e JOIN reseau.stations as s on s.enregistreur_id = e.id WHERE s.id = {station.Id}";
        var connexion = context.CreateConnection();
        string? result = connexion.QueryFirstOrDefault<string>(query);
        switch (result)
        {
            case "AP":
                return new AutomateHandler();
            case "IP":
                return new STENHandler();
            case "XX":
                return new ISODAQHandler();
            default:
                throw new InvalidOperationException("Protocole non reconnu");
        }
    }
}
