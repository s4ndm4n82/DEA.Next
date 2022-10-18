using Microsoft.Graph;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using ReadSettings;
using WriteLog;

namespace GraphAttachmentFunctions
{
    internal class GraphAttachmentFunctionsClass
    {
        public static async Task<bool> GetMessagesWithAttachments([NotNull] GraphServiceClient graphClient, string inEmail, string mainFolderId, string subFolderId1, string subFolderId2)
        {
            IMailFolderMessagesCollectionPage messages = null!;

            if (!string.IsNullOrEmpty(mainFolderId) && string.IsNullOrEmpty(subFolderId1) && string.IsNullOrEmpty(subFolderId2))
            {
                messages = await graphClient.Users[$"{inEmail}"].MailFolders["Inbox"]
                           .ChildFolders[$"{mainFolderId}"]
                           .Messages
                           .Request()
                           .Expand("attachments")
                           .Top(40)
                           .GetAsync();
            }

            if (!string.IsNullOrEmpty(mainFolderId) && !string.IsNullOrEmpty(subFolderId1) && string.IsNullOrEmpty(subFolderId2))
            {
                messages = await graphClient.Users[$"{inEmail}"].MailFolders["Inbox"]
                           .ChildFolders[$"{mainFolderId}"]
                           .ChildFolders[$"{subFolderId1}"]
                           .Messages
                           .Request()
                           .Expand("attachments")
                           .GetAsync();
            }

            if (!string.IsNullOrEmpty(mainFolderId) && !string.IsNullOrEmpty(subFolderId1) && !string.IsNullOrEmpty(subFolderId2))
            {
                messages = await graphClient.Users[$"{inEmail}"].MailFolders["Inbox"]
                           .ChildFolders[$"{mainFolderId}"]
                           .ChildFolders[$"{subFolderId1}"]
                           .ChildFolders[$"{subFolderId2}"]
                           .Messages
                           .Request()
                           .Expand("attachments")
                           .GetAsync();
            }

            foreach (var message in messages)
            {
                await DownloadAttachments(graphClient, message, inEmail, mainFolderId, subFolderId1, subFolderId2);
            }
            return false;
        }

        private static async Task<bool> DownloadAttachments([NotNull]GraphServiceClient graphClient , Message inMessage, string inEmail, string mainFolderId, string subFolderId1, string subFolderId2)
        {
            var configParam = new ReadSettingsClass();
            Attachment attachmentData = null!;

            if (inMessage.Attachments.Count > 0)
            {
                foreach(Attachment attachment in inMessage.Attachments.Where(att => configParam.AllowedExtentions.Contains(att.Name.ToLower()) && att.Size > 10240 || (att.Name.ToLower().EndsWith(".pdf") && att.Size < 10240)))
                {
                    if (!string.IsNullOrEmpty(mainFolderId) && string.IsNullOrEmpty(subFolderId1) && string.IsNullOrEmpty(subFolderId2))
                    {
                        attachmentData = await graphClient.Users[$"{inEmail}"].MailFolders["Inbox"]
                                                .ChildFolders[$"{mainFolderId}"]
                                                .Messages[$"{inMessage.Id}"]
                                                .Attachments[$"{attachment.Id}"]
                                                .Request()
                                                .GetAsync();
                    }

                    if (!string.IsNullOrEmpty(mainFolderId) && !string.IsNullOrEmpty(subFolderId1) && string.IsNullOrEmpty(subFolderId2))
                    {
                        attachmentData = await graphClient.Users[$"{inEmail}"].MailFolders["Inbox"]
                                                .ChildFolders[$"{mainFolderId}"]
                                                .ChildFolders[$"{subFolderId1}"]
                                                .Messages[$"{inMessage.Id}"]
                                                .Attachments[$"{attachment.Id}"]
                                                .Request()
                                                .GetAsync();
                    }

                    if (!string.IsNullOrEmpty(mainFolderId) && string.IsNullOrEmpty(subFolderId1) && !string.IsNullOrEmpty(subFolderId2))
                    {
                        attachmentData = await graphClient.Users[$"{inEmail}"].MailFolders["Inbox"]
                                                .ChildFolders[$"{mainFolderId}"]
                                                .ChildFolders[$"{subFolderId1}"]
                                                .ChildFolders[$"{subFolderId2}"]
                                                .Messages[$"{inMessage.Id}"]
                                                .Attachments[$"{attachment.Id}"]
                                                .Request()
                                                .GetAsync();
                    }

                    FileAttachment attachmentProperties = (FileAttachment)attachmentData;
                    string attachmentName = attachmentProperties.Name;
                    byte[] attachmentBytes = attachmentProperties.ContentBytes;
                    string attachmentExtension = Path.GetExtension(attachmentName);

                    Regex matchChar = new Regex(@"[\\\/c:]");

                }
            }
            return false;
        }
    }
}
