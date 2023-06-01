using Nexus.Core;

namespace PeopleAPI.Entities;

public class Person : AuditableEntityBase
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