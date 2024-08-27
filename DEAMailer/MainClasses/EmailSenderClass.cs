using System.Net.Mail;
using AppConfigReader;
using WriteLog;

namespace DEAMailer.MainClasses
{
    internal static class EmailClass
    {
        public static bool EmailSenderHandler(string emailBody)
        {
            var jsonData = AppConfigReaderClass.ReadAppDotConfig();
            var emailSettings = jsonData.EmailServerSettings;

            var senderEmail = emailSettings.EmailSettings.FromEmail;
            var recipientEmails = emailSettings.EmailSettings.ToEmail;

            MailMessage emailMessage = new();
            emailMessage.From = new MailAddress(senderEmail);

            foreach (var recipientEmail in recipientEmails)
            {
                emailMessage.To.Add(recipientEmail);
            }

            emailMessage.Subject = emailSettings.EmailSettings.Subject;
            emailMessage.Body = emailBody;

            SmtpClient smtpClient = new(emailSettings.ServerSettings.SmtpServer, emailSettings.ServerSettings.Port)
            {
                EnableSsl = false
            };

            try
            {
                smtpClient.Send(emailMessage);
                WriteLogClass.WriteToLog(1,"Message sent successfully.",1);
                return true;
            }
            catch (Exception ex)
            {
                WriteLogClass.WriteToLog(0,$"Error sending email: {ex.Message}",0);
                return false;
            }
        }
    }
}