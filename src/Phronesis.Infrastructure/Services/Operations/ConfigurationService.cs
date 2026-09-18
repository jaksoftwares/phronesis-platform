using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Phronesis.Application.Common.Interfaces;
using Phronesis.Domain.Operations;

namespace Phronesis.Infrastructure.Services.Operations;

public class ConfigurationService : IConfigurationService
{
    private readonly IApplicationDbContext _context;
    private readonly IMemoryCache _cache;
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(15);

    public ConfigurationService(IApplicationDbContext context, IMemoryCache cache)
    {
        _context = context;
        _cache = cache;
    }

    public async Task<string?> GetSettingValueAsync(string key, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"Setting_{key}";
        if (_cache.TryGetValue(cacheKey, out string? cachedValue))
        {
            return cachedValue;
        }

        var setting = await _context.SystemSettings.AsNoTracking().FirstOrDefaultAsync(s => s.Key == key, cancellationToken);
        if (setting != null)
        {
            _cache.Set(cacheKey, setting.Value, CacheDuration);
            return setting.Value;
        }

        return null;
    }

    public async Task<IEnumerable<SystemSetting>> GetAllSettingsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SystemSettings.AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task UpdateSettingAsync(string key, string value, string description = "", string dataType = "String", CancellationToken cancellationToken = default)
    {
        var setting = await _context.SystemSettings.FirstOrDefaultAsync(s => s.Key == key, cancellationToken);
        if (setting == null)
        {
            setting = new SystemSetting(key, value, description, dataType);
            _context.SystemSettings.Add(setting);
        }
        else
        {
            setting.UpdateValue(value);
        }

        await _context.SaveChangesAsync(cancellationToken);
        
        // Invalidate cache
        _cache.Remove($"Setting_{key}");
    }

    public async Task<bool> IsFeatureEnabledAsync(string featureName, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"Feature_{featureName}";
        if (_cache.TryGetValue(cacheKey, out bool cachedValue))
        {
            return cachedValue;
        }

        var feature = await _context.FeatureFlags.AsNoTracking().FirstOrDefaultAsync(f => f.Name == featureName, cancellationToken);
        var isEnabled = feature?.IsEnabled ?? false; // Default to false if not found

        _cache.Set(cacheKey, isEnabled, CacheDuration);
        return isEnabled;
    }

    public async Task<IEnumerable<FeatureFlag>> GetAllFeatureFlagsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.FeatureFlags.AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task ToggleFeatureAsync(string featureName, bool isEnabled, string description = "", CancellationToken cancellationToken = default)
    {
        var feature = await _context.FeatureFlags.FirstOrDefaultAsync(f => f.Name == featureName, cancellationToken);
        if (feature == null)
        {
            feature = new FeatureFlag(featureName, isEnabled, description);
            _context.FeatureFlags.Add(feature);
        }
        else
        {
            feature.Toggle(isEnabled);
        }

        await _context.SaveChangesAsync(cancellationToken);

        // Invalidate cache
        _cache.Remove($"Feature_{featureName}");
    }
}
