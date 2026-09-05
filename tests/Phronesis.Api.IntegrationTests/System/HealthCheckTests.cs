using FluentAssertions;
using Phronesis.Api.IntegrationTests.Base;

namespace Phronesis.Api.IntegrationTests.System;

public class HealthCheckTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public HealthCheckTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetHealthStatus_ReturnsOkAndHealthyStatus()
    {
        // Act
        var response = await _client.GetAsync("/health");

        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("healthy");
    }

    [Fact]
    public async Task GetApiMetadata_ReturnsOkWithVersion()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/meta");

        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Phronesis Platform API");
    }
}
