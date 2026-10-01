using FoodApp.Repositories.Interfaces;
using FoodApp.Utilities;
using MailKit.Net.Smtp;
using MimeKit;

namespace FoodApp.Services.Interfaces
{
    public interface IEmailService
    {
        void SendEmailFromTemplate(Guid? temaplateId, string? templateName, string recipientEmail, Dictionary<string, string> placeholders);
    }

    public class EmailService : IEmailService
    {
        private readonly IEmailTemplateRepository _emailTemplateRepository;

        public EmailService(IEmailTemplateRepository emailTemplateRepository)
        {
            _emailTemplateRepository = emailTemplateRepository;
        }

        public void SendEmailFromTemplate(Guid? templateId, string? templateName, string recipientEmail, Dictionary<string, string> placeholders)
        {
            var template = _emailTemplateRepository.GetTemplate(templateId, templateName);

            if (template == null)
                throw new ArgumentException("Email template not found");

            string body = template.template;
            try
            {
                foreach (var placeholder in placeholders)
                {
                    body = body.Replace($"{placeholder.Key}", placeholder.Value);
                }

                
            }
            catch (Exception ex)
            {
                throw new Exception("error when replacing values");
            }
            SendGmailEmail(recipientEmail, template.subject, body);
        }

        private void SendGmailEmail(string recipientEmail, string subject, string body)
        {
            try
            {
                string sender = SecretController.GetSmtpEmail();
                string pass = SecretController.GetSmtpPass();

                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse(sender));
                email.To.Add(MailboxAddress.Parse(recipientEmail));
                email.Subject = subject;
                email.Body = new TextPart("html") { Text = body };

                using var smtp = new SmtpClient();
                smtp.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                smtp.Authenticate(sender, pass);
                smtp.Send(email);
                smtp.Disconnect(true);
            }
            catch
            {
                throw new Exception("error sending email");
            }
        }
    }
}

