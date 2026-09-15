using Phronesis.Domain.Common;

namespace Phronesis.Domain.Identity;

public class Permission : BaseEntity
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public string Resource { get; private set; }

    private Permission() { }

    public Permission(string name, string description, string resource)
    {
        Name = name;
        Description = description;
        Resource = resource;
    }
}
