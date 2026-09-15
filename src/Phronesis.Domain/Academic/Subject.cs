using Phronesis.Domain.Common;

namespace Phronesis.Domain.Academic;

public class Subject : BaseEntity
{
    public string Name { get; private set; }
    public string Code { get; private set; }
    public string Description { get; private set; }
    public bool IsActive { get; private set; }

    private Subject() { }

    public Subject(string name, string code, string description)
    {
        Name = name;
        Code = code;
        Description = description;
        IsActive = true;
    }

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
}
