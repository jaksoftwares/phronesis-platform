using System.Threading;
using System.Threading.Tasks;

namespace Phronesis.Application.Common.Interfaces;

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken = default);
    Task SendPasswordResetEmailAsync(string to, string resetLink, CancellationToken cancellationToken = default);
    Task SendEmailVerificationEmailAsync(string to, string verificationLink, CancellationToken cancellationToken = default);
}
