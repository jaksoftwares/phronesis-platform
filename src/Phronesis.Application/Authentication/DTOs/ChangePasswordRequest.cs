namespace Phronesis.Application.Authentication.DTOs;

public record ChangePasswordRequest(string Email, string TemporaryPassword, string NewPassword);
