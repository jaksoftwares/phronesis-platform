using System;
using FluentAssertions;
using Phronesis.Domain.Identity;
using Xunit;

namespace Phronesis.Domain.UnitTests;

public class UserTests
{
    [Fact]
    public void RecordFailedLogin_After5Attempts_LocksAccount()
    {
        // Arrange
        var user = new User("test@example.com", "hash", "Test", "User");

        // Act
        for (int i = 0; i < 5; i++)
        {
            user.RecordFailedLogin();
        }

        // Assert
        user.FailedLoginAttempts.Should().Be(5);
        user.LockoutEnd.Should().NotBeNull();
        user.LockoutEnd.Value.Should().BeCloseTo(DateTime.UtcNow.AddMinutes(15), TimeSpan.FromSeconds(5));
    }
}
