using Moq;
using SHUT.Core.Application;
using SHUT.Core.Application.Interfaces;

namespace SHUT.Tests;

public class SynchroniseurTest
{
    [Fact]
    public async Task CreationTest()
    {
        var myDependencyMock = new Mock<IOrchestration>();
        //var mockEnregistreur = new OrchestationReseauService(myDependencyMock.Object);

        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(20000);
        try
        {
            //await mockEnregistreur.RunAsync(cancellationTokenSource.Token);
        }
        catch
        {

        }

        //myDependencyMock.Verify(m => m.getSequenceAppels(), Times.AtLeastOnce());
    }

    [Fact]
    public async Task CheckTimerBehaviour()
    {
        //var myDependencyMock = new Mock<IOrchestration>();
        //var mockEnregistreur = new OrchestationReseauService(myDependencyMock.Object);

        //var cancellationTokenSource = new CancellationTokenSource();
        //cancellationTokenSource.CancelAfter(20000);
        //try
        //{
        //    await mockEnregistreur.RunAsync(cancellationTokenSource.Token);
        //}
        //catch
        //{

        //}

        //myDependencyMock.Verify(m => m.getSequenceAppels(), Times.AtLeastOnce());
    }
}