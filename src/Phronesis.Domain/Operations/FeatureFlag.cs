using Phronesis.Domain.Common;

namespace Phronesis.Domain.Operations;

public class FeatureFlag : BaseEntity
{
    public string Name { get; private set; }
    public bool IsEnabled { get; private set; }
    public string Description { get; private set; }

    private FeatureFlag() { }

    public FeatureFlag(string name, bool isEnabled, string description)
    {
        Name = name;
        IsEnabled = isEnabled;
        Description = description;
    }

    public void Toggle(bool isEnabled)
    {
        IsEnabled = isEnabled;
    }
}
