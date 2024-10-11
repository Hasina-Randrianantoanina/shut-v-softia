using TestConnexion.Entities;

namespace TestConnexion.AutomateProtocol
{
    public interface ISpecificAutomateProtocol
    {
        public Task<bool> TryRead60AnalogicConfig(bool printConsole);

        public Task<bool> TryRead60AnalogicRealTimeDatas(bool printConsole);

        public Task<bool> TryGetDatas(bool printConsole);

        public List<Mesure> GetMesuresFromBinaryFile(string binaryFileFullName, bool printConsole);
        
        public Task<bool> TryWriteCsvFile(string binaryFileFullName, List<Mesure> mesures, bool printConsole);
    }
}
