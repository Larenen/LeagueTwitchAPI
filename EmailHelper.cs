using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using LeagueAPI.Models;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace LeagueAPI
{
    public class EmailHelper
    {
        public static async Task SendMail(ContactViewModel contactViewModel, string mailbody)
        {
            var apiKey = System.Environment.GetEnvironmentVariable("SENDGRID_APIKEY");
            var client = new SendGridClient(apiKey);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress(contactViewModel.Email),
                Subject = "New E-Mail from my website",
                PlainTextContent = mailbody
            };
            msg.AddTo(new EmailAddress("luki.lary@gmail.com", "Larenen"));
            var response = await client.SendEmailAsync(msg);
        }
    }
}
