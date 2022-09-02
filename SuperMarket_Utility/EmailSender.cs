using Microsoft.AspNetCore.Identity.UI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace SuperMarket_Utility
{
    public class EmailSender : IEmailSender
    {
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var message = new MailMessage();
            message.To.Add(email);
            message.From = new MailAddress("khoathdts2104013@fpt.edu.vn"); 
            message.Subject = subject;
            message.Body = htmlMessage;
            var smtpClient = new SmtpClient();
            smtpClient.Host = "smtp.gmail.com";
            smtpClient.Port = 587;
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new System.Net.NetworkCredential("khoathdts2104013@fpt.edu.vn", "dangkhoa123");
            smtpClient.EnableSsl = true;
            smtpClient.Send(message);

        }
    }
}
