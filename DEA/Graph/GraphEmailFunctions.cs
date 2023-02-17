using Microsoft.Graph;
using System.Diagnostics.CodeAnalysis;
using WriteLog;

namespace GraphEmailFunctions
{
    /// <summary>
    /// Mainly contains all the function for email actions like sending emails and forwarding errored emails.
    /// </summary>
    internal class GraphEmailFunctionsClass
    {
        /// <summary>
        /// Get's all needed details from the recived email. So, the process can forward the email to the appropriate sender.
        /// </summary>
        /// <param name="graphClient"></param>
        /// <param name="mainFolderId"></param>
        /// <param name="subFolderId1"></param>
        /// <param name="subFolderId2"></param>
        /// <param name="messageId"></param>
        /// <param name="clientEmail"></param>
        /// <param name="atnStatus"></param>
        /// <returns></returns>
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
                    WriteLogClass.WriteToLog(3, $"Exception at email forwarder if1: {ex.Message}", 5);
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
                    WriteLogClass.WriteToLog(3, $"Exception at email forwarder if2: {ex.Message}", 5);
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
                    WriteLogClass.WriteToLog(3, $"Exception at email forwarder if3: {ex.Message}", 5);
                }
            }

            if (!string.IsNullOrEmpty(fromName) && !string.IsNullOrEmpty(fromEmail) && !string.IsNullOrEmpty(toEmail))
            {
                returnResult = await SendForwardEmail(graphClient, mainFolderId, subFolderId1, subFolderId2, fromName, fromEmail, toEmail, clientEmail, messageId, atnStatus);
            }

            return (returnResult, toEmail);
        }

        /// <summary>
        /// Forwards the email and flags the email to be moved to the error folder.
        /// </summary>
        /// <param name="graphClient"></param>
        /// <param name="mainFolderId"></param>
        /// <param name="subFolderId1"></param>
        /// <param name="subFolderId2"></param>
        /// <param name="fromName"></param>
        /// <param name="fromEmail"></param>
        /// <param name="clientEmail"></param>
        /// <param name="inEmail"></param>
        /// <param name="messageId"></param>
        /// <param name="attnStatus"></param>
        /// <returns></returns>
        private static async Task<bool> SendForwardEmail([NotNull] GraphServiceClient graphClient
                                                         , string mainFolderId, string subFolderId1
                                                         , string subFolderId2, string fromName
                                                         , string fromEmail, string clientEmail
                                                         , string inEmail, string messageId, int attnStatus)
        {
            bool forwardSwitch = false;
            string mailBody;
            if (attnStatus != 1)
            {
                // Can be change with html.
                mailBody = $"Hi,<br /><b>Please do not reply to this email</b><br />. Below email you sent to {inEmail}, doesn't contain any attachment." +
                           $"<br />Please send an email with your documents as attachements to {inEmail}.";
            }
            else
            {
                mailBody= $"Hi,<br /><b>Please do not reply to this email</b><br />Below email, you sent to {inEmail}. The attachment file type is not accepted by the system." +
                          $"Please send attachments as .pdf or .jpg to {inEmail}.";
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
                    await graphClient.Users[$"{clientEmail}"].MailFolders["Inbox"]
                    .ChildFolders[$"{mainFolderId}"]
                    .Messages[$"{messageId}"]
                    .Forward(mailRecipients, null, mailBody)
                    .Request()
                    .PostAsync();

                    forwardSwitch = true;
                }
                catch (Exception ex)
                {
                    WriteLogClass.WriteToLog(3, $"Exception at emaiL forward 1: {ex.Message}", 6);
                    forwardSwitch = false;
                }
            }

            if (!string.IsNullOrEmpty(mainFolderId) && !string.IsNullOrEmpty(subFolderId1) && string.IsNullOrEmpty(subFolderId2))
            {
                try
                {
                    await graphClient.Users[$"{clientEmail}"].MailFolders["Inbox"]
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
                    WriteLogClass.WriteToLog(3, $"Exception at emai forward 2: {ex.Message}", 6);
                    forwardSwitch = false;
                }
            }

            if (!string.IsNullOrEmpty(mainFolderId) && !string.IsNullOrEmpty(subFolderId1) && !string.IsNullOrEmpty(subFolderId2))
            {
                try
                {
                    await graphClient.Users[$"{clientEmail}"].MailFolders["Inbox"]
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
                    WriteLogClass.WriteToLog(3, $"Exception at emai forward 3: {ex.Message}", 6);
                    forwardSwitch = false;
                }
            }

            return forwardSwitch;
        }
    }
}
