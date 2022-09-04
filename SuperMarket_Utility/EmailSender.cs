using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity.UI.Services;
using MimeKit;


namespace SuperMarket_Utility
{
    public class EmailSender : IEmailSender
    {

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var emailToSend = new MimeMessage();
            emailToSend.From.Add(MailboxAddress.Parse("ktkstore2022@gmail.com"));
            emailToSend.To.Add(MailboxAddress.Parse(email));
            emailToSend.Subject = subject;
            emailToSend.Body = new TextPart(MimeKit.Text.TextFormat.Html){
               Text = htmlMessage            
            };

            using (var emailClient = new SmtpClient())
            {
               await emailClient.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
               await emailClient.AuthenticateAsync("ktkstore2022@gmail.com", "nsbjwadmjdnfbnvn");
               await emailClient.SendAsync(emailToSend);
               await emailClient.DisconnectAsync(true);
            }

        }

        public async Task UserSendEmailAsync(string subject, string htmlMessage)
        {

            try
            {
                var emailToSend = new MimeMessage();
                emailToSend.From.Add(MailboxAddress.Parse("ktkstore2022@gmail.com"));
                emailToSend.To.Add(MailboxAddress.Parse("ktkstore2022@gmail.com"));
                emailToSend.Subject = subject;
                emailToSend.Body = new TextPart(MimeKit.Text.TextFormat.Html)
                {
                    Text = htmlMessage
                };


                using (var emailClient = new SmtpClient())
                {
                    await emailClient.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                    await emailClient.AuthenticateAsync("ktkstore2022@gmail.com", "nsbjwadmjdnfbnvn");
                    await emailClient.SendAsync(emailToSend);
                    await emailClient.DisconnectAsync(true);
                }
            }
            catch (Exception)
            {
              
            }
            
        }
    }
}
