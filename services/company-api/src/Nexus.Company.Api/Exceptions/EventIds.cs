namespace Nexus.CompanyAPI.Exceptions;

internal static class EventIds
{
    internal static EventId CreateCompanyTransactionError => new (101, "Create Company Transaction Error");
    internal static EventId FetchCompanyError => new (102, "Fetch Company Transaction Error");
}