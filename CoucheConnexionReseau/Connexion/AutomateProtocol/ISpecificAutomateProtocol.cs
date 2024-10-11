using Connexion.Entities;

namespace Connexion.AutomateProtocol
{
    public interface ISpecificAutomateProtocol
    {
        public Task<List<VoieInterne>> Read60AnalogicConfig(bool printConsole);

        public Task Read60AnalogicRealTimeDatas(List<VoieInterne> voiesInternes, bool printConsole);

        public Task GetDatas(Station station, int portFTP, bool printConsole, bool testMode, bool testVM);
    }
}
