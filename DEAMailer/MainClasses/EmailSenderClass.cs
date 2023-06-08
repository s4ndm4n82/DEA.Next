using System;
using System.Net;
using System.Net.Mail;
using WriteLog;
using AppConfigReader;

namespace Emailer
{
    internal class EmailerClass
    {
        public static int EmailSenderHandler(string emailBody)
        {
            AppConfigReaderClass.AppSettingsRoot jsonData = AppConfigReaderClass.ReadAppDotConfig();
            AppConfigReaderClass.Emailserversettings emailSettings = jsonData.EmailServerSettings;

            string senderEmail = emailSettings.EmailSettings.FromEmail;
            string[] recipientEmails = emailSettings.EmailSettings.ToEmail;

            MailMessage emailMessage = new();
            emailMessage.From = new(senderEmail);

            foreach (string recipientEmail in recipientEmails)
            {
                emailMessage.To.Add(recipientEmail);
            }

            emailMessage.Subject = emailSettings.EmailSettings.Subject;
            emailMessage.Body = emailBody;

            SmtpClient smtpClient = new(emailSettings.ServerSettings.SmtpServer, emailSettings.ServerSettings.Port);
            smtpClient.EnableSsl = false;

            try
            {
                smtpClient.Send(emailMessage);
                Console.WriteLine("Message sent successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error sending email: {0}", ex.Message);
            }

            return 0;
        }
    }
}