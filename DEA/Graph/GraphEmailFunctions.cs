using Microsoft.Graph;
using System.Diagnostics.CodeAnalysis;
using WriteLog;

namespace GraphEmailFunctions
{
    internal class GraphEmailFunctionsClass
    {
        public static async Task<(bool, string)> EmailForwarder([NotNull] GraphServiceClient graphClient, string mainFolderId, string subFolderId1, string subFolderId2, string messageId, string clientEmail, int atnStatus)
        {
            Message? messageDetails = null;
            string fromName = string.Empty;
            string fromEmail = string.Empty;
            string toEmail = string.Empty;
            bool returnResult = false;

            if (!string.IsNullOrWhiteSpace(mainFolderId) && string.IsNullOrWhiteSpace(subFolderId1) && string.IsNullOrWhiteSpace(subFolderId2))
            {
                try
                {
                    messageDetails = await graphClient.Users[$"{clientEmail}"].MailFolders["Inbox"]
                                .ChildFolders[$"{mainFolderId}"]
                                .Messages[$"{messageId}"]
                                .Request()
                                .GetAsync();

                    fromName = messageDetails.From.EmailAddress.Name;
                    fromEmail = messageDetails.From.EmailAddress.Address;
                    toEmail = messageDetails.InternetMessageHeaders.Where(toAddress => toAddress.Value.Contains("@efakturamottak.no")).ToString()!;
                }
                catch (Exception ex)
                {
                    WriteLogClass.WriteToLog(3, $"Exception at email forwarder if1: {ex.Message}", string.Empty);
                }
            }

            if (!string.IsNullOrWhiteSpace(mainFolderId) && !string.IsNullOrWhiteSpace(subFolderId1) && string.IsNullOrWhiteSpace(subFolderId2))
            {
                try
                {
                    messageDetails = await graphClient.Users[$"{clientEmail}"].MailFolders["Inbox"]
                                .ChildFolders[$"{mainFolderId}"]
                                .ChildFolders[$"{subFolderId1}"]
                                .Messages[$"{messageId}"]
                                .Request()
                                .GetAsync();

                    fromName = messageDetails.From.EmailAddress.Name;
                    fromEmail = messageDetails.From.EmailAddress.Address;
                    toEmail = messageDetails.InternetMessageHeaders.Where(toAddress => toAddress.Value.Contains("@efakturamottak.no")).ToString()!;
                }
                catch (Exception ex)
                {
                    WriteLogClass.WriteToLog(3, $"Exception at email forwarder if2: {ex.Message}", string.Empty);
                }
            }

            if (!string.IsNullOrWhiteSpace(mainFolderId) && !string.IsNullOrWhiteSpace(subFolderId1) && !string.IsNullOrWhiteSpace(subFolderId2))
            {
                try
                {
                    messageDetails = await graphClient.Users[$"{clientEmail}"].MailFolders["Inbox"]
                                .ChildFolders[$"{mainFolderId}"]
                                .ChildFolders[$"{subFolderId1}"]
                                .ChildFolders[$"{subFolderId2}"]
                                .Messages[$"{messageId}"]
                                .Request()
                                .GetAsync();

                    fromName = messageDetails.From.EmailAddress.Name;
                    fromEmail = messageDetails.From.EmailAddress.Address;
                    toEmail = messageDetails.InternetMessageHeaders.Where(toAddress => toAddress.Value.Contains("@efakturamottak.no")).ToString()!;
                }
                catch (Exception ex)
                {
                    WriteLogClass.WriteToLog(3, $"Exception at email forwarder if3: {ex.Message}", string.Empty);
                }
            }

            if (!string.IsNullOrEmpty(fromName) && !string.IsNullOrEmpty(fromEmail) && !string.IsNullOrEmpty(toEmail))
            {
                returnResult = await SendForwardEmail(graphClient, mainFolderId, subFolderId1, subFolderId2, fromName, fromEmail, toEmail, clientEmail, messageId, atnStatus);
            }

            return (returnResult, toEmail);
        }

        private static async Task<bool> SendForwardEmail([NotNull] GraphServiceClient graphClient
                                                         , string mainFolderId, string subFolderId1
                                                         , string subFolderId2, string fromName
                                                         , string fromEmail, string toEmail
                                                         , string inEmail, string messageId, int attnStatus)
        {
            bool forwardSwitch = false;
            string mailBody = string.Empty;

            if (attnStatus != 1)
            {
                // Can be change with html.
                mailBody = "Hi,<br /><b>Please do not reply to this email</b><br />. Below email doesn't contain any attachment.";
            }
            else
            {
                mailBody= "Hi,<br /><b>Please do not reply to this email</b><br />Below email's attachment file type is not accepted. Please send attachments as .pdf or .jpg.";
            }

            var mailRecipients = new List<Recipient>()
            {
                new Recipient
                {
                    EmailAddress = new EmailAddress
                    {
                        Name= fromName,
                        Address= fromEmail,
                    }
                }
            };

            if (!string.IsNullOrEmpty(mainFolderId) && string.IsNullOrEmpty(subFolderId1) && string.IsNullOrEmpty(subFolderId2))
            {
                try
                {
                    await graphClient.Users[$"{toEmail}"].MailFolders["Inbox"]
                    .ChildFolders[$"{mainFolderId}"]
                    .Messages[$"{messageId}"]
                    .Forward(mailRecipients, null, mailBody)
                    .Request()
                    .PostAsync();

                    forwardSwitch = true;
                }
                catch (Exception ex)
                {
                    WriteLogClass.WriteToLog(3, $"Exception at emai forward 1: {ex.Message}", string.Empty);
                    forwardSwitch = false;
                }
            }

            if (!string.IsNullOrEmpty(mainFolderId) && !string.IsNullOrEmpty(subFolderId1) && string.IsNullOrEmpty(subFolderId2))
            {
                try
                {
                    await graphClient.Users[$"{toEmail}"].MailFolders["Inbox"]
                    .ChildFolders[$"{mainFolderId}"]
                    .ChildFolders[$"{subFolderId1}"]
                    .Messages[$"{messageId}"]
                    .Forward(mailRecipients, null, mailBody)
                    .Request()
                    .PostAsync();

                    forwardSwitch = true;
                }
                catch (Exception ex)
                {
                    WriteLogClass.WriteToLog(3, $"Exception at emai forward 2: {ex.Message}", string.Empty);
                    forwardSwitch = false;
                }
            }

            if (!string.IsNullOrEmpty(mainFolderId) && !string.IsNullOrEmpty(subFolderId1) && !string.IsNullOrEmpty(subFolderId2))
            {
                try
                {
                    await graphClient.Users[$"{toEmail}"].MailFolders["Inbox"]
                    .ChildFolders[$"{mainFolderId}"]
                    .ChildFolders[$"{subFolderId1}"]
                    .ChildFolders[$"{subFolderId2}"]
                    .Messages[$"{messageId}"]
                    .Forward(mailRecipients, null, mailBody)
                    .Request()
                    .PostAsync();

                    forwardSwitch = true;
                }
                catch (Exception ex)
                {
                    WriteLogClass.WriteToLog(3, $"Exception at emai forward 3: {ex.Message}", string.Empty);
                    forwardSwitch = false;
                }
            }

            return forwardSwitch;
        }
    }
}
