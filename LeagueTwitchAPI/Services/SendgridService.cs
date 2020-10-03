using System;
using System.Threading.Tasks;
using LeagueAPI.Models;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace LeagueAPI.Services
{
    public class SendgridService : ISendgridService
    {
        private readonly string sendgridApiKey;
        public SendgridService(IConfiguration configuration)
        {
            sendgridApiKey = configuration[Constants.SENDGRID_API_KEY] ?? throw new ArgumentNullException("Sendgrid api key not found.");
        }

        public async Task SendMail(ContactDto contactViewModel)
        {
            var mailbody = $@"
                            This is a new contact request from your website:

                            Name: {contactViewModel.Name}
                            Email: {contactViewModel.Email}
                            Message: ""{contactViewModel.Message}""

                            The website contact form";

            var client = new SendGridClient(sendgridApiKey);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress(contactViewModel.Email),
                Subject = "New E-Mail from my LeagueTwitchApi",
                PlainTextContent = mailbody
            };
            msg.AddTo(new EmailAddress("luki.lary@gmail.com", "Larenen"));
            await client.SendEmailAsync(msg);
        }
    }
}
