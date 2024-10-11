using ConnexionTR.Entities;

namespace ConnexionTR.AutomateProtocol
{
    public interface ISpecificAutomateProtocol
    {
        public Task<bool> TryRead60AnalogicConfig(bool printConsole);

        //public Task<bool> TryRead60AnalogicRealTimeDatas(bool printConsole);

    }
}
