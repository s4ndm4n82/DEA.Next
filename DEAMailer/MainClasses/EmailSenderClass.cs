using System.Net.Mail;
using WriteLog;
using AppConfigReader;

namespace Emailer;

internal class EmailerClass
{
    public static bool EmailSenderHandler(string emailBody)
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