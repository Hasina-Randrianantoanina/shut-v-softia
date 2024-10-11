using SHUT.Core.Application.Interfaces;
using SHUT.Core.Application.Reseau;
using SHUT.Core.Domain.Historique;
using SHUT.Core.Domain.Reseau;

namespace SHUT.Core.Application
{
    // Service background qui s'initialise par la programmation d'appels
    // lance l'appel selon la programmation definie
    // orchestre le deroulement e l'appel
    public class OrchestationReseauService
    {
        private IOrchestrateur _orchestrateur { get; set; }
        private StationService _stationService { get; set; }

        public OrchestationReseauService(
            IOrchestrateur orchestrateur,
            StationService stationService
        )
        {
            _orchestrateur = orchestrateur;
            _stationService = stationService;
        }

        //Appel chaque heure
        public async Task<(bool success, string errorMessage)> AppelStationsHoraire()
        {
            IEnumerable<StationsReseau> stations = await _stationService.GetStations();
            List<StationsReseau> stationsL = stations.ToList();
            return await _orchestrateur.AppelerStations(stationsL);
        }

        //Appel initie par user en selectionnant liste a appeler
        // public async Task<(bool success, string errorMessage)> AppelStationsUser(
        //     List<StationsReseau> stations
        // )
        // {
        //     if (stations == null || !stations.Any())
        //     {
        //         Console.WriteLine("Aucune station à appeler.");
        //         return (false, "Aucune station à appeler.");
        //     }

        //     try
        //     {
        //         bool result = await _orchestrateur.AppelerStations(stations);
        //         return (
        //             result,
        //             result ? null : "L'appel des stations a échoué sans erreur spécifique."
        //         );
        //     }
        //     catch (Exception ex)
        //     {
        //         Console.WriteLine($"Erreur lors de l'appel des stations: {ex}");
        //         return (false, ex.ToString());
        //     }
        // }

        public async Task<(bool success, string message)> AppelStationsUser(
            List<StationsReseau> stations
        )
        {
            if (stations == null || !stations.Any())
            {
                return (false, "Aucune station à appeler.");
            }
            return await _orchestrateur.AppelerStations(stations);
        }
    }
}
