using Nexus.Common;

namespace Nexus.PeopleAPI.Entities;

public class Person : AuditableNexusEntityBase
{
    public Person(string name, string email)
    {
        Name = name;
        Email = email;
    }
    
    public string Name { get; private set; }

    public string Email { get; private set; }

    public void UpdateName(string newName)
    {
        Name = newName;
    }
}
