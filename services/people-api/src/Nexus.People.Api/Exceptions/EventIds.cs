namespace Nexus.PeopleAPI.Exceptions;

public static class EventIds
{
    internal static EventId CreatePersonTransactionError => new (201, "Create Person Transaction Error");
    internal static EventId UpdatePersonTransactionError => new (202, "Update Person Transaction Error");
    internal static EventId DeletePersonTransactionError => new (203, "Delete Person Transaction Error");
}