using System.IO;
using System.Net;
using System.Net.Mail;
using UnstopAPI.Models.DTO;
using UnstopAPI.Repository.IRepository;

namespace UnstopAPI.Repository
{
    public class EmailSenderRepository : IEmailSenderRepository
    {
        private readonly IConfiguration _configuration;

        public EmailSenderRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<bool> SendEmailAsync(EmailRequestModel emailRequest)
        {
            try
            {
                SmtpClient client = new(_configuration["SMTP:Host"], int.Parse(_configuration["SMTP:Port"]))
                {
                    Credentials = new NetworkCredential(_configuration["SMTP:Username"], _configuration["SMTP:Password"]),
                    EnableSsl = true,
                };

                MailMessage mailMessage = new()
                {
                    From = new MailAddress(_configuration["SMTP:From"]),
                    Subject = emailRequest.Subject,
                    Body = emailRequest.Body,
                    IsBodyHtml = emailRequest.IsBodyHtml,
                };

                mailMessage.To.Add(emailRequest.To);
                mailMessage.CC.Add(new MailAddress(_configuration["SMTP:CC"]));

                if (emailRequest.Attachment != null)
                {
                    Attachment attachment = new(new MemoryStream(emailRequest.Attachment), emailRequest.AttachmentName);

                    mailMessage.Attachments.Add(attachment);
                }

                await client.SendMailAsync(mailMessage);

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
