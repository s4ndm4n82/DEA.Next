using Microsoft.Graph;
using System.Diagnostics.CodeAnalysis;
using GetRecipientEmail;
using EmailFileHelper;
using FileFunctions;
using FolderFunctions;
using WriteLog;
using WriteNamesToLog;
using DEA;
using GraphEmailFunctions;
using GetMailFolderIds;
using UserConfigReader;

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
        public static async Task<bool> GetMessagesWithAttachments([NotNull] GraphServiceClient graphClient,
                                                                  string inEmail,
                                                                  string mainFolderId,
                                                                  string subFolderId1,
                                                                  string subFolderId2,
                                                                  int maxMails,
                                                                  int customerId)
        {
            bool flag = false;
            IMailFolderMessagesCollectionPage messages = null!;            

            if (!string.IsNullOrEmpty(mainFolderId) && string.IsNullOrEmpty(subFolderId1) && string.IsNullOrEmpty(subFolderId2))
            {
                try
                {
                    messages = await graphClient.Users[$"{inEmail}"].MailFolders["Inbox"]
                           .ChildFolders[$"{mainFolderId}"]
                           .Messages
                           .Request()
                           .Expand("attachments")
                           .Top(maxMails)
                           .GetAsync();
                }
                catch (Exception ex)
                {
                    WriteLogClass.WriteToLog(0, $"Excpetion at geting messages 1: {ex.Message}", 0);
                }
            }

            if (!string.IsNullOrEmpty(mainFolderId) && !string.IsNullOrEmpty(subFolderId1) && string.IsNullOrEmpty(subFolderId2))
            {
                try
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
                catch (Exception ex)
                {
                    WriteLogClass.WriteToLog(0, $"Excpetion at geting messages 2: {ex.Message}", 0);
                }
            }

            if (!string.IsNullOrEmpty(mainFolderId) && !string.IsNullOrEmpty(subFolderId1) && !string.IsNullOrEmpty(subFolderId2))
            {
                try
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
                catch (Exception ex)
                {
                    WriteLogClass.WriteToLog(0, $"Excpetion at geting messages 2: {ex.Message}", 0);
                }
            }

            foreach (var message in messages)
            {
                if (message.Attachments.Count > 0)
                {
                    WriteLogClass.WriteToLog(1, $"Reading email {message.Subject}", 2);
                    flag = await DownloadAttachments(graphClient, message, inEmail, mainFolderId, subFolderId1, subFolderId2, customerId);
                }
                else
                {
                    var forwardFalg = await GraphEmailFunctionsClass.EmailForwarder(graphClient, mainFolderId, subFolderId1, subFolderId2, message.Id, inEmail, 0);

                    if (forwardFalg.Item1)
                    {
                        WriteLogClass.WriteToLog(1, $"Email forwarded to {forwardFalg.Item2} ....", 2);

                        var destinationId = await GetMailFolderIdsClass.GetErrorFolderId(graphClient, inEmail, mainFolderId, subFolderId1, subFolderId2);
                        flag = await GraphHelper.MoveEmails(mainFolderId, subFolderId1, subFolderId2, message.Id, destinationId, inEmail);

                        if (flag)
                        {
                            WriteLogClass.WriteToLog(1, $"No attachments. Mail moved to error folder ....", 2);
                        }
                    }
                }
            }
            return flag;
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
        private static async Task<bool> DownloadAttachments([NotNull]GraphServiceClient graphClient,
                                                            Message inMessage,
                                                            string inEmail,
                                                            string mainFolderId,
                                                            string subFolderId1,
                                                            string subFolderId2,
                                                            int customerId)
        {
            int loopCount = 0; // In order to check if the loop ran at least once.

            bool flagReturn = false; // Flag to check if the transfer to TPS is successful.
            bool loopFlag = false; // To execute the TPS file transfer function at the end of the file download loop.

            UserConfigReaderClass.CustomerDetailsObject jsonData = UserConfigReaderClass.ReadAppDotConfig<UserConfigReaderClass.CustomerDetailsObject>();
            UserConfigReaderClass.Customerdetail clientDetails = jsonData.CustomerDetails!.FirstOrDefault(cid => cid.id == customerId)!;

            string mainClient = clientDetails.MainCustomer!;
            string clientName = clientDetails.ClientName!;
            string recipientEmail = string.Empty;

            if (mainClient.ToLower() == "digiacc")
            {
                recipientEmail = new(GetRecipientEmailClass.GetRecipientEmail(graphClient, mainFolderId, subFolderId1, subFolderId2, inMessage.Id, inEmail)); // Get the Recipient email from the email.
            }            

            string downloadPath = Path.Combine(FolderFunctionsClass.CheckFolders("attachments"), recipientEmail, GraphHelper.FolderNameRnd(10)); // Creates the file download path.
            Attachment attachmentData = null!; // Variable to store attachment ID.
            List<string> acceptedExtentions = clientDetails.DocumentDetails!.DocumentExtensions!;
            IEnumerable<Attachment> acceptedAtachments = inMessage.Attachments
                                                         .Where(x => acceptedExtentions
                                                         .Contains(Path.GetExtension(x.Name.ToLower())) && x.Size > 10240 || (x.Name.ToLower().EndsWith(".pdf") && x.Size < 10240));
            int lastItem = acceptedAtachments.Count();
            

            foreach (Attachment attachment in acceptedAtachments)
            {
                if (!string.IsNullOrEmpty(mainFolderId) && string.IsNullOrEmpty(subFolderId1) && string.IsNullOrEmpty(subFolderId2))
                {
                    try
                    {
                        attachmentData = await graphClient.Users[$"{inEmail}"].MailFolders["Inbox"]
                                            .ChildFolders[$"{mainFolderId}"]
                                            .Messages[$"{inMessage.Id}"]
                                            .Attachments[$"{attachment.Id}"]
                                            .Request()
                                            .GetAsync();
                    }
                    catch (Exception ex)
                    {
                        WriteLogClass.WriteToLog(0, $"Excpetion at attachmentData 1: {ex.Message}", 0);
                    }
                }

                if (!string.IsNullOrEmpty(mainFolderId) && !string.IsNullOrEmpty(subFolderId1) && string.IsNullOrEmpty(subFolderId2))
                {
                    try
                    {
                        attachmentData = await graphClient.Users[$"{inEmail}"].MailFolders["Inbox"]
                                            .ChildFolders[$"{mainFolderId}"]
                                            .ChildFolders[$"{subFolderId1}"]
                                            .Messages[$"{inMessage.Id}"]
                                            .Attachments[$"{attachment.Id}"]
                                            .Request()
                                            .GetAsync();
                    }
                    catch (Exception ex)
                    {
                        WriteLogClass.WriteToLog(0, $"Excpetion at attachmentData 2: {ex.Message}", 0);
                    }
                }

                if (!string.IsNullOrEmpty(mainFolderId) && string.IsNullOrEmpty(subFolderId1) && !string.IsNullOrEmpty(subFolderId2))
                {
                    try
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
                    catch (Exception ex)
                    {
                        WriteLogClass.WriteToLog(0, $"Excpetion at attachmentData 3: {ex.Message}", 0);
                    }
                }

                // Attachment properties.
                FileAttachment attachmentProperties = (FileAttachment)attachmentData;                
                byte[] attachmentBytes = attachmentProperties.ContentBytes;

                if (EmailFileHelperClass.FileDownloader(downloadPath, EmailFileHelperClass.FileNameCleaner(attachmentProperties.Name), attachmentBytes))
                {
                    loopCount++;
                }

                if (lastItem == loopCount)
                {                    
                    loopFlag = true;                    
                    WriteLogClass.WriteToLog(1, $"Downloaded {lastItem} attachments from {inMessage.Subject} ....", 2);
                    WriteLogClass.WriteToLog(1, $"Downloaded file names: {WriteNamesToLogClass.GetFileNames(downloadPath)}", 2);
                }
            }

            if (loopCount > 0 && loopFlag)
            {
                // Call the base 64 converter and the file submitter to the web service.
                // And then moves to email to export folder. If both functions succed then the varible will be set to true.
                await MoveMailsToExport(graphClient, mainFolderId, subFolderId1, subFolderId2, inMessage.Id, inMessage.Subject, inEmail); 

                flagReturn = await FileFunctionsClass.SendToWebService(null!, downloadPath, customerId, null!, null!, recipientEmail);
            }

            // Forwards the email if there's no attachments and attachment download loop doesn't run.
            if (!loopFlag && loopCount == 0)
            {
                var forwardFalg = await GraphEmailFunctionsClass.EmailForwarder(graphClient, mainFolderId, subFolderId1, subFolderId2, inMessage.Id, inEmail, 0);

                if (forwardFalg.Item1)
                {
                    WriteLogClass.WriteToLog(1, $"Email forwarded to {forwardFalg.Item2}", 2);

                    var destinationId = await GetMailFolderIdsClass.GetErrorFolderId(graphClient, inEmail, mainFolderId, subFolderId1, subFolderId2);
                    flagReturn = await GraphHelper.MoveEmails(mainFolderId, subFolderId1, subFolderId2, inMessage.Id, destinationId, inEmail);

                    if (flagReturn)
                    {
                        WriteLogClass.WriteToLog(1, $"Mail moved to error folder ....", 2);
                        flagReturn = true;
                    }
                }
            }
            return flagReturn;
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
        private static async Task<bool> MoveMailsToExport([NotNull] GraphServiceClient graphClient, string mainFolderId, string subFolderId1, string subFolderId2, string messageId, string messageSubject, string inEmail)
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
                    WriteLogClass.WriteToLog(0, $"Exception at detination folder name 1st if: {ex.Message}", 0);
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
                    WriteLogClass.WriteToLog(0, $"Exception at detination folder name 2nd if: {ex.Message}", 0);
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
                    WriteLogClass.WriteToLog(0, $"Exception at detination folder name 3rd if: {ex.Message}", 0);
                }
            }

            if (await GraphHelper.MoveEmails(mainFolderId, subFolderId1, subFolderId2, messageId, exportFolder.Id, inEmail))
            {   
                WriteLogClass.WriteToLog(1, $"Email {messageSubject} moved to export folder ...", 2);
                return true;
            }
            else
            {
                WriteLogClass.WriteToLog(1, $"Email {messageSubject} not moved to export folder ...", 2);
                return false;
            }
        }
    }
}
