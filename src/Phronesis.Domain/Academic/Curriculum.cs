using Phronesis.Domain.Common;

namespace Phronesis.Domain.Academic;

public class Curriculum : BaseEntity
{
    public string Name { get; private set; }
    public string Version { get; private set; }
    public string Description { get; private set; }
    public bool IsActive { get; private set; }

    private Curriculum() { }

    public Curriculum(string name, string version, string description)
    {
        Name = name;
        Version = version;
        Description = description;
        IsActive = true;
    }

    public void Update(string name, string version, string description)
    {
        Name = name;
        Version = version;
        Description = description;
    }

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
}
