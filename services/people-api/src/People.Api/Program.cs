namespace PeopleAPI;

[ExcludeFromCodeCoverage]
public class Program
{
    public static void Main(string[] args)
    {
        PeopleApiBootstrapper bootstrapper = new (args);
        bootstrapper.BootstrapAndRun();
    }
}