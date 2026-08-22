using SendGrid;
using SendGrid.Helpers.Mail;
using System.Net.Mail;

namespace BestStoreAPI.Services
{
    public class EmailSender
    {
        private readonly string apiKey;
        private readonly string fromeEmail;
        private readonly string senderName;

        public EmailSender(IConfiguration configuration)
        {
            apiKey = configuration["EmailSender;ApiKey"]!;
            fromeEmail = configuration["EmailSender:FromEmail"]!;
            senderName = configuration["EmailSender:SendName"]!;
            
        }
        public async Task SendEmail(string toemail , string username , string message, string subject)
        {
            var apiKey = "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";
            /* var options = new SendGridClientOptions
            {
                ApiKey = apiKey
            };
            options.SetDataResidency("eu"); 
            var client = new SendGridClient(options); */
            // uncomment the above 6 lines if you are sending mail using a regional EU subuser
            // and remove the client declaration just below
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(fromeEmail, senderName);
            
            var to = new EmailAddress(toemail, username);
            var plainTextContent = message;
            var htmlContent = "";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);
        }
    }
}
