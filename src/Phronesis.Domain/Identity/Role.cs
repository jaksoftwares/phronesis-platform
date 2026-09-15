using Phronesis.Domain.Common;

namespace Phronesis.Domain.Identity;

public class Role : BaseEntity
{
    public string Name { get; private set; }
    public string Description { get; private set; }

    private Role() { }

    public Role(string name, string description)
    {
        Name = name;
        Description = description;
    }
}
