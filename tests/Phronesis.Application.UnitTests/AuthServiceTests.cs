using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Phronesis.Application.Authentication;
using Phronesis.Application.Authentication.DTOs;
using Phronesis.Application.Common.Interfaces;
using Phronesis.Domain.Common.Exceptions;
using Phronesis.Domain.Identity;
using Xunit;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;
using MockQueryable.Moq;

namespace Phronesis.Application.UnitTests;

public class AuthServiceTests
{
    private readonly Mock<IApplicationDbContext> _dbContextMock;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private readonly Mock<IJwtProvider> _jwtProviderMock;
    private readonly Mock<IEmailService> _emailServiceMock;
    private readonly Mock<IConfiguration> _configMock;
    private readonly Mock<ILogger<AuthService>> _loggerMock;
    private readonly AuthService _sut;

    public AuthServiceTests()
    {
        _dbContextMock = new Mock<IApplicationDbContext>();
        _passwordHasherMock = new Mock<IPasswordHasher>();
        _jwtProviderMock = new Mock<IJwtProvider>();
        _emailServiceMock = new Mock<IEmailService>();
        _configMock = new Mock<IConfiguration>();
        _loggerMock = new Mock<ILogger<AuthService>>();

        _sut = new AuthService(
            _dbContextMock.Object,
            _passwordHasherMock.Object,
            _jwtProviderMock.Object,
            _emailServiceMock.Object,
            _configMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task LoginAsync_WithInvalidPassword_ThrowsDomainException_And_RecordsFailedLogin()
    {
        // Arrange
        var email = "test@example.com";
        var password = "WrongPassword";
        
        var user = new User(email, "hashed_password", "Test", "User");
        var users = new List<User> { user }.BuildMockDbSet();
        
        _dbContextMock.Setup(x => x.Users).Returns(users.Object);
        _passwordHasherMock.Setup(x => x.Verify(password, "hashed_password")).Returns(false);

        var request = new LoginRequest { Email = email, Password = password };

        // Act
        Func<Task> act = async () => await _sut.LoginAsync(request, "device", "127.0.0.1", CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<DomainException>().WithMessage("Invalid email or password.");
        user.FailedLoginAttempts.Should().Be(1);
    }
}
