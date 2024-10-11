using AutomateOperations.Entities;
using AutomateOperations.Utils;

namespace AutomateOperations.AutomateProtocol
{
    public interface ISpecificAutomateProtocol
    {
        public Task<(bool statusConfig, Erreur? erreur)> TryRead60AnalogicConfig();

        public Task<(bool statusGetDatas,  Erreur? erreur)> TryGetDatas(string downloadDirectory);

        public (List<Mesure>? mesures, Erreur? erreur) ReadMesuresInBinaryFile(string binaryFileFullName);
        
    }
}
