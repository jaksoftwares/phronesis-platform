using Phronesis.Domain.Common;

namespace Phronesis.Domain.Operations;

public class SystemSetting : BaseEntity
{
    public string Key { get; private set; }
    public string Value { get; private set; }
    public string Description { get; private set; }
    public string DataType { get; private set; }

    private SystemSetting() { }

    public SystemSetting(string key, string value, string description, string dataType)
    {
        Key = key;
        Value = value;
        Description = description;
        DataType = dataType;
    }

    public void UpdateValue(string value)
    {
        Value = value;
    }
}
