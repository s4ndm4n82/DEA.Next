using Microsoft.Graph;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using ReadSettings;
using WriteLog;
using DEA;

namespace GraphAttachmentFunctions
{
    internal class GraphAttachmentFunctionsClass
    {
        /// <summary>
        /// As the function name suggest this funcion is designed to get messages with attachments. And then pass it on to
        /// the next step which is downloading the.
        /// </summary>
        /// <param name="graphClient"></param>
        /// <param name="inEmail"></param>
        /// <param name="mainFolderId"></param>
        /// <param name="subFolderId1"></param>
        /// <param name="subFolderId2"></param>
        /// <returns>A bool value (true or false)</returns>
        public static async Task<bool> GetMessagesWithAttachments([NotNull] GraphServiceClient graphClient, string inEmail, string mainFolderId, string subFolderId1, string subFolderId2, int maxMails, int customerId)
        {
            IMailFolderMessagesCollectionPage messages = null!;

            if (!string.IsNullOrEmpty(mainFolderId) && string.IsNullOrEmpty(subFolderId1) && string.IsNullOrEmpty(subFolderId2))
            {
                messages = await graphClient.Users[$"{inEmail}"].MailFolders["Inbox"]
                           .ChildFolders[$"{mainFolderId}"]
                           .Messages
                           .Request()
                           .Expand("attachments")
                           .Top(maxMails)
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
                           .Top(maxMails)
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
                           .Top(maxMails)
                           .GetAsync();
            }

            foreach (var message in messages)
            {
                await DownloadAttachments(graphClient, message, inEmail, mainFolderId, subFolderId1, subFolderId2, customerId);
            }
            return false;
        }

        /// <summary>
        /// This function would start downloading the attachmetns. But his will only download attachments which are above
        /// 10kB (10240 Bytes). But any PDF file will be downloaded regardless of the file size.
        /// </summary>
        /// <param name="graphClient"></param>
        /// <param name="inMessage"></param>
        /// <param name="inEmail"></param>
        /// <param name="mainFolderId"></param>
        /// <param name="subFolderId1"></param>
        /// <param name="subFolderId2"></param>
        /// <returns>A bool value (true or false)</returns>
        private static async Task<bool> DownloadAttachments([NotNull]GraphServiceClient graphClient , Message inMessage, string inEmail, string mainFolderId, string subFolderId1, string subFolderId2, int customerId)
        {
            int loopCount = 0; // In order to check if the loop ran at least once.
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

                    Regex matchChar = new (@"[\\\/c:]");

                    if (matchChar.IsMatch(attachmentName.ToLower()))
                    {
                        attachmentName = Path.GetFileName(attachmentName);
                    }

                    Regex matchChar2 = new (@"[\w\d\s\.\-]+");

                    if (!matchChar2.IsMatch(attachmentName))
                    {
                        attachmentName = Regex.Replace(attachmentName, @"[\w\d\s\.\-]+", " ");
                    }

                    if (GraphHelper.DownloadAttachedFiles(Path.Combine(GraphHelper.CheckFolders("email"), GraphHelper.FolderNameRnd(10)), attachmentName, attachmentBytes))
                    {
                        loopCount++;

                        WriteLogClass.WriteToLog(3, $"File {attachmentName} downloaded ...", string.Empty);

                        if (await MoveEmail(graphClient, mainFolderId, subFolderId1, subFolderId2, inMessage.Id, inMessage.Subject, inEmail))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }

            if (loopCount > 0)
            {
                // Calla the base 64 converter and the file submitter to the web service.
            }

            if (inMessage.Attachments.Count == 0 || loopCount == 0)
            {
                // Calla the rejection function to send the email to error and then forward to customer.
            }
            return false;
        }

        /// <summary>
        /// After download ends with succession this function will be called. Which will move the completed email to another folder.
        /// Normally it's called "Exported".
        /// </summary>
        /// <param name="graphClient"></param>
        /// <param name="mainFolderId"></param>
        /// <param name="subFolderId1"></param>
        /// <param name="subFolderId2"></param>
        /// <param name="messageId"></param>
        /// <param name="messageSubject"></param>
        /// <param name="inEmail"></param>
        /// <returns>A bool value (true or false)</returns>
        private static async Task<bool> MoveEmail([NotNull] GraphServiceClient graphClient, string mainFolderId, string subFolderId1, string subFolderId2, string messageId, string messageSubject, string inEmail)
        {
            IMailFolderChildFoldersCollectionPage moveLocation;
            MailFolder exportFolder = null!;

            if (!string.IsNullOrEmpty(mainFolderId) && string.IsNullOrEmpty(subFolderId1) && string.IsNullOrEmpty(subFolderId2))
            {
                try
                {
                    moveLocation = await graphClient.Users[$"{inEmail}"].MailFolders["Inbox"]
                               .ChildFolders[$"{mainFolderId}"]
                               .ChildFolders
                               .Request()
                               .GetAsync();

                    exportFolder = moveLocation.FirstOrDefault(x => x.DisplayName == "Exported")!;
                }
                catch (Exception ex)
                {
                    WriteLogClass.WriteToLog(3, $"Exception at detination folder name 1st if: {ex.Message}", string.Empty);
                }
            }

            if (!string.IsNullOrEmpty(mainFolderId) && !string.IsNullOrEmpty(subFolderId1) && string.IsNullOrEmpty(subFolderId2))
            {
                try
                {
                    moveLocation = await graphClient.Users[$"{inEmail}"].MailFolders["Inbox"]
                               .ChildFolders[$"{mainFolderId}"]
                               .ChildFolders[$"{subFolderId1}"]
                               .ChildFolders
                               .Request()
                               .GetAsync();

                    exportFolder = moveLocation.FirstOrDefault(x => x.DisplayName == "Exported")!;
                }
                catch (Exception ex)
                {
                    WriteLogClass.WriteToLog(3, $"Exception at detination folder name 2nd if: {ex.Message}", string.Empty);
                }
            }

            if (!string.IsNullOrEmpty(mainFolderId) && !string.IsNullOrEmpty(subFolderId1) && !string.IsNullOrEmpty(subFolderId2))
            {
                try
                {
                    moveLocation = await graphClient.Users[$"{inEmail}"].MailFolders["Inbox"]
                               .ChildFolders[$"{mainFolderId}"]
                               .ChildFolders[$"{subFolderId1}"]
                               .ChildFolders[$"{subFolderId2}"]
                               .ChildFolders
                               .Request()
                               .GetAsync();

                    exportFolder = moveLocation.FirstOrDefault(x => x.DisplayName == "Exported")!;
                }
                catch (Exception ex)
                {
                    WriteLogClass.WriteToLog(3, $"Exception at detination folder name 3rd if: {ex.Message}", string.Empty);
                }
            }

            if (await GraphHelper.MoveEmails(mainFolderId, subFolderId1, subFolderId2, messageId, exportFolder.Id, inEmail))
            {   
                WriteLogClass.WriteToLog(3, $"Email {messageSubject} moved to export folder ...",string.Empty);
                return true;
            }
            else
            {
                WriteLogClass.WriteToLog(3, $"Email {messageSubject} not moved to export folder ...", string.Empty);
                return false;
            }
        }
    }
}
