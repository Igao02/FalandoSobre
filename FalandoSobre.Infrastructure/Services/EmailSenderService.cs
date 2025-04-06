using FalandoSobre.Web.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;
using System.Text.Encodings.Web;

public class EmailSenderService : IEmailSender<ApplicationUser>
{
    private readonly IConfiguration _configuration;

    public EmailSenderService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmailAsync(string email, string subject, string message)
    {
        var emailSettings = _configuration.GetSection("EmailSettings");

        var smtpClient = new SmtpClient(emailSettings["SMTPServer"]) 
        {
            Port = int.Parse(emailSettings["SMTPPort"]), 
            Credentials = new NetworkCredential(emailSettings["SMTPUsername"], emailSettings["SMTPPassword"]),
            EnableSsl = true,
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress(emailSettings["FromEmail"]), 
            Subject = subject,
            Body = message,
            IsBodyHtml = true,
        };
        mailMessage.To.Add(email);

        await smtpClient.SendMailAsync(mailMessage);
    }


    public async Task SendConfirmationLinkAsync(ApplicationUser user, string email, string confirmationLink)
    {
        string subject = "Confirm your email";
        string message = $"Por favor, confirme seu e-mail através desse link: <a href='{HtmlEncoder.Default.Encode(confirmationLink)}'>link</a>";
        await SendEmailAsync(email, subject, message);
    }

    public async Task SendPasswordResetCodeAsync(ApplicationUser user, string email, string resetCode)
    {
        string subject = "Reset your password code";
        string message = $"Your password reset code is: {HtmlEncoder.Default.Encode(resetCode)}";
        await SendEmailAsync(email, subject, message);
    }

    public async Task SendPasswordResetLinkAsync(ApplicationUser user, string email, string callbackUrl)
    {
        string subject = "Resetar sua senha";
        string message = $"Por favor, altere sua senha clicando no link: <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>link</a>";
        await SendEmailAsync(email, subject, message);
    }
}
