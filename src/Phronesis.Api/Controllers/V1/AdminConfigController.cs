using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Phronesis.Application.Common.Interfaces;
using Phronesis.Shared.Responses;

namespace Phronesis.Api.Controllers.V1;

[ApiController]
[Route("api/v1/admin/config")]
[Authorize(Policy = "RequireAdmin")] // Ensure only admins can access these
public class AdminConfigController : ControllerBase
{
    private readonly IConfigurationService _configService;

    public AdminConfigController(IConfigurationService configService)
    {
        _configService = configService;
    }

    [HttpGet("settings")]
    public async Task<IActionResult> GetAllSettings(CancellationToken cancellationToken)
    {
        try
        {
            var settings = await _configService.GetAllSettingsAsync(cancellationToken);
            return Ok(ApiResponse<object>.Ok(settings));
        }
        catch (Exception ex) { return BadRequest(new { Message = ex.Message }); }
    }

    [HttpGet("settings/{key}")]
    [AllowAnonymous] // Some settings like branding or login types might need to be public
    public async Task<IActionResult> GetSetting(string key, CancellationToken cancellationToken)
    {
        try
        {
            var value = await _configService.GetSettingValueAsync(key, cancellationToken);
            if (value == null) return NotFound(ApiResponse<object>.Failure("Setting not found."));

            return Ok(ApiResponse<object>.Ok(new { Key = key, Value = value }));
        }
        catch (Exception ex) { return BadRequest(new { Message = ex.Message }); }
    }

    [HttpPut("settings/{key}")]
    public async Task<IActionResult> UpdateSetting(string key, [FromBody] UpdateSettingDto dto, CancellationToken cancellationToken)
    {
        try
        {
            await _configService.UpdateSettingAsync(key, dto.Value, dto.Description, dto.DataType, cancellationToken);
            return Ok(ApiResponse<object>.Ok(null, "Setting updated successfully."));
        }
        catch (Exception ex) { return BadRequest(new { Message = ex.Message }); }
    }

    [HttpGet("features")]
    [AllowAnonymous] // Frontends often need to know what features to show
    public async Task<IActionResult> GetAllFeatureFlags(CancellationToken cancellationToken)
    {
        try
        {
            var features = await _configService.GetAllFeatureFlagsAsync(cancellationToken);
            return Ok(ApiResponse<object>.Ok(features));
        }
        catch (Exception ex) { return BadRequest(new { Message = ex.Message }); }
    }

    [HttpGet("features/{name}")]
    [AllowAnonymous]
    public async Task<IActionResult> IsFeatureEnabled(string name, CancellationToken cancellationToken)
    {
        try
        {
            var isEnabled = await _configService.IsFeatureEnabledAsync(name, cancellationToken);
            return Ok(ApiResponse<object>.Ok(new { Name = name, IsEnabled = isEnabled }));
        }
        catch (Exception ex) { return BadRequest(new { Message = ex.Message }); }
    }

    [HttpPut("features/{name}/toggle")]
    public async Task<IActionResult> ToggleFeature(string name, [FromBody] ToggleFeatureDto dto, CancellationToken cancellationToken)
    {
        try
        {
            await _configService.ToggleFeatureAsync(name, dto.IsEnabled, dto.Description, cancellationToken);
            return Ok(ApiResponse<object>.Ok(null, $"Feature {name} is now {(dto.IsEnabled ? "Enabled" : "Disabled")}."));
        }
        catch (Exception ex) { return BadRequest(new { Message = ex.Message }); }
    }
}

public class UpdateSettingDto
{
    public string Value { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string DataType { get; set; } = "String";
}

public class ToggleFeatureDto
{
    public bool IsEnabled { get; set; }
    public string Description { get; set; } = string.Empty;
}
