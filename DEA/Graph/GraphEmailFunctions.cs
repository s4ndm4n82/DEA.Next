using Microsoft.Graph;
using System.Diagnostics.CodeAnalysis;
using WriteLog;

namespace GraphEmailFunctions
{
    internal class GraphEmailFunctionsClass
    {
        public static async Task<bool> EmailForwarder([NotNull] GraphServiceClient graphClient, string mainFolderId, string subFolderId1, string subFolderId2, string messageId, string clientEmail, int atnStatus)
        {
            Message? messageDetails = null;
            string fromName = string.Empty;
            string fromEmail = string.Empty;
            string toEmail = string.Empty;

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
                await SendForwardEmail(graphClient, mainFolderId, subFolderId1, subFolderId2, fromName, fromEmail, toEmail, clientEmail, messageId, atnStatus);
            }
        }

        private static async Task<bool> SendForwardEmail([NotNull] GraphServiceClient graphClient
                                                         , string mainFolderId, string subFolderId1
                                                         , string subFolderId2, string fromName
                                                         , string fromEmail, string toEmail
                                                         , string inEmail, string messageId, int attnStatus)
        {

        }
    }
}
