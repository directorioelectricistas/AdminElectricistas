using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;

namespace DirectorioElectricistas.Services
{
    public class SmtpSettings
    {
        public string Server { get; set; }
        public int Port { get; set; }
        public string SenderName { get; set; }
        public string SenderEmail { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class EmailService
    {
        private readonly SmtpSettings _smtpSettings;

        public EmailService(IOptions<SmtpSettings> smtpSettings)
        {
            _smtpSettings = smtpSettings.Value;
        }

        public async Task SendEmailAsync(string recipientEmail, string subject, string message)
        {
            try
            {
                using (var client = new SmtpClient(_smtpSettings.Server, _smtpSettings.Port))
                {
                    client.Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password);
                    client.EnableSsl = true;

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(_smtpSettings.SenderEmail, _smtpSettings.SenderName),
                        Subject = subject,
                        Body = message,
                        IsBodyHtml = true
                    };

                    mailMessage.To.Add(recipientEmail);

                    await client.SendMailAsync(mailMessage);
                }
            }
            catch (SmtpFailedRecipientException ex)
            {
                // Si el email proporcionado no es válido o no existe
                throw new InvalidOperationException("La dirección de correo proporcionada no es válida.", ex);
            }
            catch (SmtpException ex) when (ex.Message.Contains("WASCL UserAction verdict is not None"))
            {
                // Si la cuenta está bloqueada
                throw new InvalidOperationException("El envío de correos está bloqueado. Contacte con el administrador.", ex);
            }
            catch (Exception ex)
            {
                // Cualquier otro tipo de error
                throw new InvalidOperationException("Ocurrió un error al intentar enviar el correo. Verificar cuenta de correo electrónico.", ex);
            }
        }
    }
}
