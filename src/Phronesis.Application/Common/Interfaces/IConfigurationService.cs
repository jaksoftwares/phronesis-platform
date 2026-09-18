using Phronesis.Domain.Operations;

namespace Phronesis.Application.Common.Interfaces;

public interface IConfigurationService
{
    // Settings
    Task<string?> GetSettingValueAsync(string key, CancellationToken cancellationToken = default);
    Task<IEnumerable<SystemSetting>> GetAllSettingsAsync(CancellationToken cancellationToken = default);
    Task UpdateSettingAsync(string key, string value, string description = "", string dataType = "String", CancellationToken cancellationToken = default);

    // Feature Flags
    Task<bool> IsFeatureEnabledAsync(string featureName, CancellationToken cancellationToken = default);
    Task<IEnumerable<FeatureFlag>> GetAllFeatureFlagsAsync(CancellationToken cancellationToken = default);
    Task ToggleFeatureAsync(string featureName, bool isEnabled, string description = "", CancellationToken cancellationToken = default);
}
