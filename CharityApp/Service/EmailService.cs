using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;

public class EmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config;
    }

    public void SendEmail(string toEmail, string subject, string body)
    {
        var email = new MimeMessage();
        email.From.Add(new MailboxAddress(_config["EmailSettings:SenderName"], _config["EmailSettings:SenderEmail"]));
        email.To.Add(new MailboxAddress("", toEmail));
        email.Subject = subject;
        email.Body = new TextPart("plain") { Text = body };

        using var client = new SmtpClient();
        client.Connect(_config["EmailSettings:SmtpServer"], int.Parse(_config["EmailSettings:SmtpPort"]), false);
        client.Authenticate(_config["EmailSettings:SenderEmail"], _config["EmailSettings:SenderPassword"]);
        client.Send(email);
        client.Disconnect(true);
    }
}
