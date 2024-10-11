using SHUT.Core.Application.Interfaces;
namespace ReseauDEAOperations;

public class myDependency : IMyDependency
{
    public void DoWork()
    {
        System.Console.WriteLine("Test");
    }
}
