using MimeKit;
using MailKit.Net.Smtp;
using System.Threading.Tasks;

namespace NUREMarks.Services
{
    public class EmailService
    {
        public async Task SendEmailAsync(string email, string subject, string message)
        {
            // STEP 1: Navigate to this page https://www.google.com/settings/security/lesssecureapps & set to "Turn On"

            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress("NURE Marks", "nuremarks@gmail.com"));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = message
            };

            using (var client = new SmtpClient())
            {
                client.Connect("smtp.gmail.com", 587);


                // Note: since we don't have an OAuth2 token, disable
                // the XOAUTH2 authentication mechanism.
                client.AuthenticationMechanisms.Remove("XOAUTH2");

                // Note: only needed if the SMTP server requires authentication
                client.Authenticate("nuremarks@gmail.com", "[yeh'marks");

                client.Send(emailMessage);
                client.Disconnect(true);
            }
        }
    }
}