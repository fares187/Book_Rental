using Bookify.web.settings;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.CodeAnalysis.Options;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace Bookify.web.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly MailSettings _mailSettings;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public EmailSender(IOptions<MailSettings> mailSettings, IWebHostEnvironment webHostEnvironment = null)
        {
            _mailSettings = mailSettings.Value;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            MailMessage message = new MailMessage()
            {
                From = new MailAddress(_mailSettings.Email!,_mailSettings.DisplayName),
                Body= htmlMessage,
                Subject= subject,   
                IsBodyHtml= true    

            };
            message.To.Add(_webHostEnvironment.IsDevelopment()? "faresahmed687@gmail.com": email);
            SmtpClient smtpClient = new SmtpClient(_mailSettings.Host)
            {
                Port = _mailSettings.Port,  
                Credentials=new NetworkCredential(_mailSettings.Email,_mailSettings.Password),
                EnableSsl = true    
            };
            await smtpClient.SendMailAsync(message);
            smtpClient.Dispose();
        }
    }
}
