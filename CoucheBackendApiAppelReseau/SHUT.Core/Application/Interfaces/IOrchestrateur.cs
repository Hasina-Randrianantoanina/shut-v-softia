using SHUT.Core.Domain.Common;
using SHUT.Core.Domain.Reseau;

namespace SHUT.Core.Application.Interfaces;

public interface IOrchestrateur
{
    Task<(bool success, string message)> AppelerStations(List<StationsReseau> stations);
}