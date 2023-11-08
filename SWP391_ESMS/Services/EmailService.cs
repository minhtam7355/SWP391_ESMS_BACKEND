using AutoMapper.Internal;
using Microsoft.Extensions.Options;
using MimeKit;
using MailKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using SWP391_ESMS.Models.ViewModels;

namespace SWP391_ESMS.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;

        public EmailService(IOptions<EmailSettings> options)
        {
            _emailSettings = options.Value;
        }

        public async Task SendEmailAsync(EmailRequest emailRequest)
        {
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(_emailSettings.Email);
            email.To.Add(MailboxAddress.Parse(emailRequest.ToEmail));
            email.Subject = emailRequest.Subject;
            var builder = new BodyBuilder();


            //byte[] fileBytes;
            //if (System.IO.File.Exists("Attachment/dummy.pdf"))
            //{
            //    FileStream file = new FileStream("Attachment/dummy.pdf", FileMode.Open, FileAccess.Read);
            //    using (var ms = new MemoryStream())
            //    {
            //        file.CopyTo(ms);
            //        fileBytes = ms.ToArray();
            //    }
            //    builder.Attachments.Add("attachment.pdf", fileBytes, ContentType.Parse("application/octet-stream"));
            //    builder.Attachments.Add("attachment2.pdf", fileBytes, ContentType.Parse("application/octet-stream"));
            //}

            builder.HtmlBody = emailRequest.Body;
            email.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();
            smtp.Connect(_emailSettings.Host, _emailSettings.Port, SecureSocketOptions.StartTls);
            smtp.Authenticate(_emailSettings.Email, _emailSettings.Password);
            await smtp.SendAsync(email);
            smtp.Disconnect(true);
        }
    }
}
