using Phronesis.Domain.Common;

namespace Phronesis.Domain.Identity;

public class Role : BaseEntity
{
    public string Name { get; private set; }
    public string Description { get; private set; }

    private readonly List<UserRole> _userRoles = new();
    public IReadOnlyCollection<UserRole> UserRoles => _userRoles.AsReadOnly();

    private readonly List<RolePermission> _rolePermissions = new();
    public IReadOnlyCollection<RolePermission> RolePermissions => _rolePermissions.AsReadOnly();

    private Role() { }

    public Role(string name, string description)
    {
        Name = name;
        Description = description;
    }
}
