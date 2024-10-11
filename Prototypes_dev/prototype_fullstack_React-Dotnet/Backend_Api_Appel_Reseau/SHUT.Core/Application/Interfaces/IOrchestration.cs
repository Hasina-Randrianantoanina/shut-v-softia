using SHUT.Core.Domain.Common;

namespace SHUT.Core.Application.Interfaces;

public interface IOrchestration
{
    public Task getSequenceAppels(CancellationToken cancellationToken);
    public Task InitializeSequence(List<CycleAppel> EnregistreursCycleAppel, CancellationToken cancellationToken);

}
