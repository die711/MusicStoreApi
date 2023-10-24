using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MusicStore.Entities;
using MusicStore.Services.Interfaces;

namespace MusicStore.Services.Implementations;

public class EmailService : IEmailService
{
    private readonly IOptions<AppSettings> _options;
    private readonly ILogger<EmailService> _logger;


    public EmailService(IOptions<AppSettings> options, ILogger<EmailService> logger)
    {
        _options = options;
        _logger = logger;
    }


    public async Task SendEmailAsync(string email, string subject, string message)
    {
        try
        {
            var mailMessage = new MailMessage(
                new MailAddress(_options.Value.SmtpConfiguration.UserName, _options.Value.SmtpConfiguration.FromName),
                new MailAddress(email))

            {
                Subject = subject,
                Body = message,
                IsBodyHtml = true
            };


            using var smpClient = new SmtpClient(_options.Value.SmtpConfiguration.Server,
                _options.Value.SmtpConfiguration.PortNumber)
            {
                Credentials = new NetworkCredential(_options.Value.SmtpConfiguration.UserName,
                    _options.Value.SmtpConfiguration.Password),
                EnableSsl = _options.Value.SmtpConfiguration.EnableSsl
            };


            await smpClient.SendMailAsync(mailMessage);
        }
        catch (SmtpException ex)
        {
            _logger.LogWarning(ex,"No se puede enviar el correo {Message}", ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Error al enviar el correo a {Email}, {Message}", email, ex.Message);
        }

    }
}