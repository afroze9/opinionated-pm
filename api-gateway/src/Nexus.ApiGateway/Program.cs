namespace Nexus.ApiGateway;

[ExcludeFromCodeCoverage]
public class Program
{
    public static void Main(string[] args)
    {
        ApiGatewayBootstrapper bootstrapper = new (args);
        bootstrapper.BootstrapAndRun();
    }
}