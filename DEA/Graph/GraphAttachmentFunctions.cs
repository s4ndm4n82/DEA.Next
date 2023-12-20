using Microsoft.Graph;
using System.Diagnostics.CodeAnalysis;
using FileNameCleanerClass;
using WriteLog;
using WriteNamesToLog;
using GraphHelper;
using GraphEmailFunctions;
using GetMailFolderIds;
using AppConfigReader;
using Directory = System.IO.Directory;
using FileFunctions;
using UserConfigRetriverClass;
using UserConfigSetterClass;
using GraphDownloadAttachmentFilesClass;

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
        public static async Task<int> GetMessagesWithAttachments([NotNull] GraphServiceClient graphClient,
                                                                  string inEmail,
                                                                  string mainFolderId,
                                                                  string subFolderId1,
                                                                  string subFolderId2,
                                                                  int maxMails,
                                                                  int customerId)
        {
            List<string> folderIds = new() { mainFolderId, subFolderId1, subFolderId2 };
            folderIds.RemoveAll(string.IsNullOrEmpty); // Remove the empty once.

            if (!folderIds.Any())
            {
                return 4;
            }

            IMailFolderRequestBuilder requestBuidler = graphClient.Users[$"{inEmail}"].MailFolders["Inbox"];

            foreach (string folderId in folderIds)
            {
                // Change the request builder to the next folder. If it is the last folder, then we will get the messages from there.
                requestBuidler = requestBuidler.ChildFolders[$"{folderId}"];
            }

            // Empty variable to store the messages.
            IMailFolderMessagesCollectionPage messages;
            try
            {
                // Get the messages with attachments.
                messages = await requestBuidler
                                 .Messages
                                 .Request()
                                 .Expand("attachments")
                                 .Top(maxMails)
                                 .GetAsync();

                // If there are no messages, then return 4.
                if (!messages.Any())
                {
                    return 4;
                }
            }
            catch (Exception ex)
            {
                // Log the failure and return the error code.
                WriteLogClass.WriteToLog(0, $"Exception at GetMessagesWithAttachments: {ex.Message}", 0);
                return 4; // 4 is an error code.
            }

            // Process the messages. And adds all the returned results to a list.
            List<Task<int>> taskReturns = new();
            foreach (Message message in messages)
            {
                taskReturns.Add(ProcessMessageAsync(graphClient,
                                                    message,
                                                    inEmail,
                                                    mainFolderId,
                                                    subFolderId1,
                                                    subFolderId2,
                                                    customerId));
            }

            // Wait for all the tasks to complete.
            int[] results = await Task.WhenAll(taskReturns);

            // Error code list.
            int[] errorCodes = { 4, 3, 1 };

            // Get the first error code from the list.
            int flag = errorCodes.FirstOrDefault(code => results.Contains(code));

            // If there is no error code, then return 0.
            flag = flag != 0 ? flag : 0;

            return flag;
        }

        private static async Task<int> ProcessMessageAsync(GraphServiceClient graphClient,
                                                           Message message,
                                                           string inEmail,
                                                           string mainFolderId,
                                                           string subFolderId1,
                                                           string subFolderId2,
                                                           int customerId)
        {
            if (!message.Attachments.Any())
            {
                try
                {
                    // If there are no attachments, forward the email and return 3.
                    (bool forwardSuccess, string forwardResult) = await GraphEmailFunctionsClass.EmailForwarder(graphClient,
                                                                                                      mainFolderId,
                                                                                                      subFolderId1,
                                                                                                      subFolderId2,
                                                                                                      message.Id,
                                                                                                      inEmail,
                                                                                                      customerId);
                    if (!forwardSuccess)
                    {
                        // Log the failure and return an error code
                        WriteLogClass.WriteToLog(1, $"Failed to forward email {message.Id}.", 2);
                        return 4; // Error code for failure
                    }

                    // Get the error folder id.
                    string destinationId = await GetMailFolderIdsClass.GetErrorFolderId(graphClient,
                                                                        inEmail,
                                                                        mainFolderId,
                                                                        subFolderId1,
                                                                        subFolderId2);
                    if (destinationId == null)
                    {
                        // Move the email to the error folder. And returns a bool value.
                        bool moveSuccess = await GraphHelperClass.MoveEmails(mainFolderId,
                                                                             subFolderId1,
                                                                             subFolderId2,
                                                                             message.Id,
                                                                             destinationId,
                                                                             inEmail);
                        if (!moveSuccess)
                        {
                            // Log the failure and return an error code
                            WriteLogClass.WriteToLog(1, $"Failed to move email {message.Id} to error folder.", 2);
                            return 4; // Error code for failure
                        }
                    }


                    // Log the problem and retuen 3.
                    WriteLogClass.WriteToLog(1, $"No attachments. Email forwarded to {forwardResult} and moved to error folder.", 2);
                    return 3; // Success code for no attachments
                }
                catch (Exception ex)
                {
                    WriteLogClass.WriteToLog(0, $"Exception at ProcessMessageAsync: {ex.Message}", 0);
                    return 4;
                }
            }
            else
            {
                try
                {
                    // If there are attachments, download them.
                    return await DownloadAttachments(graphClient,
                                                     message,
                                                     inEmail,
                                                     mainFolderId,
                                                     subFolderId1,
                                                     subFolderId2,
                                                     customerId);
                }
                catch (Exception ex)
                {
                    WriteLogClass.WriteToLog(0, $"Exception at ProcessMessageAsync running attachment downloader: {ex.Message}", 0);
                    return 4;
                }
            }
        }

        /// <summary>
        /// This function would start downloading the attachmetns. But only download attachments which are above
        /// 10kB (10240 Bytes). But any PDF file will be downloaded regardless of the file size.
        /// </summary>
        /// <param name="graphClient"></param>
        /// <param name="inMessage"></param>
        /// <param name="inEmail"></param>
        /// <param name="mainFolderId"></param>
        /// <param name="subFolderId1"></param>
        /// <param name="subFolderId2"></param>
        /// <returns>A bool value (true or false)</returns>
        private static async Task<int> DownloadAttachments([NotNull]GraphServiceClient graphClient,
                                                            Message inMessage,
                                                            string inEmail,
                                                            string mainFolderId,
                                                            string subFolderId1,
                                                            string subFolderId2,
                                                            int customerId)
        {
            int downloadCount = 0;
            UserConfigSetter.Customerdetail clientDeails = await UserConfigRetriver.RetriveUserConfigById(customerId);
            
            string recipientEmail = GraphDownloadAttachmentFiles.DetermineRecipientEmail(graphClient,
                                                                                         clientDeails,
                                                                                         mainFolderId,
                                                                                         subFolderId1,
                                                                                         subFolderId2,
                                                                                         inMessage.Id,
                                                                                         inEmail);

            string downloadFolderPath = GraphDownloadAttachmentFiles.CreateDownloadPath(recipientEmail);
            IEnumerable<Attachment> attachmentList = GraphDownloadAttachmentFiles.FilterAttachments(inMessage.Attachments, clientDeails.DocumentDetails.DocumentExtensions);

            if (!attachmentList.Any())
            {
                WriteLogClass.WriteToLog(1, $"No attachments to download in attachmentList ....", 2);
                return -1;
            }

            List<string> attachmentFileNameList = new();

            foreach (Attachment attachment in attachmentList)
            {
                try
                {
                    Attachment attachmentData = await GraphDownloadAttachmentFiles.FetchAttachmentData(graphClient,
                                                                                                       inEmail,
                                                                                                       mainFolderId,
                                                                                                       subFolderId1,
                                                                                                       subFolderId2,
                                                                                                       inMessage.Id,
                                                                                                       attachment.Id);

                    if (await GraphDownloadAttachmentFiles.SaveAttachmentToFile(attachmentData,
                                                                                downloadFolderPath,
                                                                                FileNameCleaner.FileNameCleanerFunction(attachment.Name)))
                    {
                        attachmentFileNameList.Add(attachment.Name);
                        downloadCount++;
                    }
                }
                catch (Exception ex)
                {
                    WriteLogClass.WriteToLog(0, $"Exception when processing attachment {inMessage.Subject}: {ex.Message}", 0);
                }
            }

            if (attachmentList.Count() == downloadCount)
            {
                WriteLogClass.WriteToLog(1, $"Downloaded file names: {WriteNamesToLogClass.GetFileNames(attachmentFileNameList.ToArray())}", 2);
                if (!await MoveMailsToExport(graphClient,
                                             mainFolderId,
                                             subFolderId1,
                                             subFolderId2,
                                             inMessage.Id,
                                             inMessage.Subject,
                                             inEmail))
                {
                    return 0;
                }

                return await StartAttachmentFilesUplaod(downloadFolderPath, customerId, recipientEmail);
            }

            return 0;
        }

        /// <summary>
        /// Batch file upload function. The while loop will upload file untill it ends.
        /// </summary>
        /// <param name="downloadFolderPath">Local download folder path.</param>
        /// <param name="customerId">Customer ID</param>
        /// <param name="toEmail">Email the client has sent email to.</param>
        /// <returns></returns>
        private static async Task<int> StartAttachmentFilesUplaod(string downloadFolderPath,
                                                                  int customerId,
                                                                  string toEmail)
        {
            try
            {
                if (!Directory.Exists(downloadFolderPath))
                {
                    WriteLogClass.WriteToLog(0, $"Directory not found: {downloadFolderPath}", 0);
                    return -1;
                }

                int successfullUpload = 0; // Successfull upload count.
                UserConfigSetter.Customerdetail batchSize = await UserConfigRetriver.RetriveUserConfigById(customerId);
                int batchCurrentIndex = 0;

                DirectoryInfo downloadDirectoryInfo = new(downloadFolderPath);
                FileInfo[] downloadedFileNameList = downloadDirectoryInfo.GetFiles();

                while (batchCurrentIndex < downloadedFileNameList.Length)
                {
                    string[] currentBatchFileNames = downloadedFileNameList
                                             .Skip(batchCurrentIndex)
                                             .Take(batchSize.MaxBatchSize)
                                             .Select(file => file.Name)
                                             .ToArray();

                    // Create a single task that uploads all fiels in the batch.
                    int uploadResult = await FileFunctionsClass.SendToWebService(null,
                                                                                 downloadFolderPath,
                                                                                 customerId,
                                                                                 currentBatchFileNames,
                                                                                 null,
                                                                                 toEmail);
                    if (uploadResult != 1)
                    {
                        WriteLogClass.WriteToLog(0, $"Upload failed with result: {uploadResult}", 0);
                        successfullUpload = 4;
                    }
                    else
                    {
                        successfullUpload = uploadResult;
                    }

                    // Increment the batch index
                    batchCurrentIndex += batchSize.MaxBatchSize;
                }
                return successfullUpload;
            }
            catch (Exception ex)
            {
                WriteLogClass.WriteToLog(0, $"Exception at StartAttachmentFilesUplaod: {ex.Message}", 0);
                return -1;
            }
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

            if (await GraphHelperClass.MoveEmails(mainFolderId, subFolderId1, subFolderId2, messageId, exportFolder.Id, inEmail))
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
