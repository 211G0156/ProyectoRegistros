using Microsoft.Extensions.Configuration;
using ProyectoRegistros.Services;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

public class SmtpEmailService : EmailService
{
    private readonly string _host;
    private readonly int _port;
    private readonly string _user;
    private readonly string _pass;
    private readonly bool _enableSsl;
    private readonly string _senderEmail;
    private readonly string _senderName;

    public SmtpEmailService(IConfiguration config)
    {
        _host = config["SmtpSettings:Host"];
        _port = int.Parse(config["SmtpSettings:Port"]);
        _user = config["SmtpSettings:User"];
        _pass = config["SmtpSettings:Pass"];
        _enableSsl = bool.Parse(config["SmtpSettings:EnableSsl"]);
        _senderEmail = config["SmtpSettings:SenderEmail"];
        _senderName = config["SmtpSettings:SenderName"];
    }

    public async Task EnviarEmailAsync(string destinatario, string asunto, string cuerpo)
    {
        var client = new SmtpClient(_host, _port)
        {
            Credentials = new NetworkCredential(_user, _pass),
            EnableSsl = _enableSsl
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress(_senderEmail, _senderName),

            Subject = asunto,
            Body = cuerpo,
            IsBodyHtml = true,
        };
        mailMessage.To.Add(destinatario);

        await client.SendMailAsync(mailMessage);
    }
}