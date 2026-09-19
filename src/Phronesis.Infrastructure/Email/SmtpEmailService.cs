using System.Net;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Phronesis.Application.Common.Interfaces;

namespace Phronesis.Infrastructure.Email;

public class SmtpEmailService : IEmailService
{
    private readonly EmailSettings _settings;
    private readonly ILogger<SmtpEmailService> _logger;

    public SmtpEmailService(IOptions<EmailSettings> settings, ILogger<SmtpEmailService> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken = default)
    {
        try
        {
            if (_settings.SmtpServer == "localhost")
            {
                _logger.LogInformation("DEV MODE: Bypassing actual SMTP send to {Email}", to);
                _logger.LogInformation("Email Subject: {Subject}", subject);
                _logger.LogInformation("Email Body: {Body}", body);
                return;
            }

            using var client = new SmtpClient(_settings.SmtpServer, _settings.SmtpPort)
            {
                Credentials = new NetworkCredential(_settings.SmtpUsername, _settings.SmtpPassword),
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_settings.FromEmail, _settings.FromName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
            };
            
            mailMessage.To.Add(to);

            await client.SendMailAsync(mailMessage, cancellationToken);
            _logger.LogInformation("Email sent to {Email} with subject {Subject}", to, subject);
        }
        catch (System.Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {Email}", to);
            // In a real scenario, you might want to rethrow or handle specific exceptions
            throw;
        }
    }

    public Task SendPasswordResetEmailAsync(string to, string resetLink, CancellationToken cancellationToken = default)
    {
        var subject = "Reset Your Phronesis Password";
        var body = $@"
            <h2>Password Reset Request</h2>
            <p>Please click the link below to reset your password. This link will expire in 1 hour.</p>
            <p><a href='{resetLink}'>Reset Password</a></p>
            <p>If you didn't request this, please ignore this email.</p>
        ";
        return SendEmailAsync(to, subject, body, cancellationToken);
    }

    public Task SendEmailVerificationEmailAsync(string to, string verificationLink, CancellationToken cancellationToken = default)
    {
        var subject = "Verify Your Phronesis Account";
        var body = $@"
            <h2>Email Verification</h2>
            <p>Please click the link below to verify your email address. This link will expire in 1 hour.</p>
            <p><a href='{verificationLink}'>Verify Email</a></p>
            <p>If you didn't request this, please ignore this email.</p>
        ";
        return SendEmailAsync(to, subject, body, cancellationToken);
    }
}
