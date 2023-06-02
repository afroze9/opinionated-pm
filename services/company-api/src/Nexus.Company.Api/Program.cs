namespace Nexus.CompanyAPI;

[ExcludeFromCodeCoverage]
public class Program
{
    public static void Main(string[] args)
    {
        CompanyApiBootstrapper bootstrapper = new (args);
        bootstrapper.BootstrapAndRun();
    }
}