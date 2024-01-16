using Microsoft.Graph;
using FileNameCleanerClass;
using WriteLog;
using WriteNamesToLog;
using GraphEmailFunctions;
using GetMailFolderIds;
using Directory = System.IO.Directory;
using FileFunctions;
using UserConfigRetriverClass;
using UserConfigSetterClass;
using GraphDownloadAttachmentFilesClass;
using GraphMoveEmailsToExportClass;
using GraphMoveEmailsrClass;
using Message = Microsoft.Graph.Message;
using DEA.Next.Graph.GraphEmailActons;

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
        public static async Task<int> GetMessagesWithAttachments(IMailFolderRequestBuilder requestBuilder,
                                                                 string inEmail,
                                                                 string deletedItemsId,
                                                                 int maxMails,
                                                                 int customerId)
        {   
            IMailFolderMessagesCollectionPage messages;
            try
            {
                // Get the messages with attachments.
                messages = await requestBuilder
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
                taskReturns.Add(ProcessMessageAsync(requestBuilder,
                                                    message,
                                                    inEmail,
                                                    deletedItemsId,
                                                    customerId));
            }

            // Wait for all the tasks to complete.
            int[] results = await Task.WhenAll(taskReturns);

            // Error code list.
            int[] errorCodes = { 5, 4, 3, 1 };

            // Get the first error code from the list.
            int flag = errorCodes.LastOrDefault(code => results.Contains(code));

            // If there is no error code, then return 0.
            flag = flag != 0 ? flag : 0;

            return flag;
        }

        /// <summary>
        /// Start the attachment processing and downloading.
        /// </summary>
        /// <param name="requestBuilder">Request built</param>
        /// <param name="message">Email message</param>
        /// <param name="inEmail">Clients email address.</param>
        /// <param name="deletedItemsId">Deleted items folder ID</param>
        /// <param name="customerId">Customer ID</param>
        /// <returns>Returns the success or error code.</returns>
        private static async Task<int> ProcessMessageAsync(IMailFolderRequestBuilder requestBuilder,
                                                           Message message,
                                                           string inEmail,
                                                           string deletedItemsId,
                                                           int customerId)
        {
            // Get client details.
            UserConfigSetter.Customerdetail clientDeails = await UserConfigRetriver.RetriveUserConfigById(customerId);

            // Filter the attachments.
            IEnumerable<Attachment> attachmentList = GraphDownloadAttachmentFiles.FilterAttachments(message.Attachments,
                                                                                                    clientDeails.DocumentDetails.DocumentExtensions);

            // If there are attachments.
            if (attachmentList.Any())
            {
                try
                {
                    // If there are attachments, download them.
                    return await DownloadAttachments(requestBuilder,
                                                     message,
                                                     attachmentList,
                                                     clientDeails,
                                                     message.Id,
                                                     customerId);
                }
                catch (Exception ex)
                {
                    WriteLogClass.WriteToLog(0, $"Exception at ProcessMessageAsync running attachment downloader: {ex.Message}", 0);
                    return 4;
                }
            }
            else
            {
                try
                {
                    // Check if the email has too many replies. If som email will be moved to deleted items.
                    if (await CheckEmailChain.CheckEmailChainAsync(requestBuilder, message.Id, deletedItemsId))
                    {
                        WriteLogClass.WriteToLog(0, $"Email {message.Subject} has too many replies moved to deleted items ....", 0);
                        return 5;
                    }

                    // If there are no attachments, forward the email and return 3.
                    (bool forwardSuccess, string forwardResult) = await GraphEmailFunctionsClass.EmailForwarder(requestBuilder,
                                                                                                                message.Id,
                                                                                                                inEmail,
                                                                                                                message.Attachments.Count);
                    if (!forwardSuccess)
                    {
                        // Log the failure and return an error code
                        WriteLogClass.WriteToLog(1, $"Failed to forward email {message.Subject}. Error: {forwardResult}", 2);
                        return 4; // Error code for failure
                    }

                    // Mark the email as not read.
                    bool markSuccess = await MarkEmailsAsNotRead(requestBuilder, message.Id);
                    if (!markSuccess)
                    {
                        // Log the failure and return an error code
                        WriteLogClass.WriteToLog(1, $"Failed to mark email {message.Id} as not read.", 2);
                        return 4; // Error code for failure
                    }

                    // Get the error folder id.
                    string destinationId = await GetMailFolderIdsClass.GetErrorFolderId(requestBuilder);
                    if (destinationId != null)
                    {
                        // Move the email to the error folder. And returns a bool value.
                        bool moveSuccess = await GraphMoveEmailsFolder.MoveEmailsToAnotherFolder(requestBuilder,
                                                                                                      message.Id,
                                                                                                      destinationId);
                        if (!moveSuccess)
                        {
                            // Log the failure and return an error code
                            WriteLogClass.WriteToLog(1, $"Failed to move email {message.Id} to error folder.", 2);
                            return 4; // Error code for failure
                        }
                    }

                    return 3; // Error code for no attachments
                }
                catch (Exception ex)
                {
                    WriteLogClass.WriteToLog(0, $"Exception at ProcessMessageAsync: {ex.Message}", 0);
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
        private static async Task<int> DownloadAttachments(IMailFolderRequestBuilder requestBuilder,
                                                           Message inMessage,
                                                           IEnumerable<Attachment> attachmentList,
                                                           UserConfigSetter.Customerdetail clientDeails,
                                                           string messageId,
                                                           int customerId)
        {
            try
            {
                int downloadCount = 0;

                string recipientEmail = await GraphDownloadAttachmentFiles.DetermineRecipientEmail(requestBuilder,
                                                                                                   clientDeails,
                                                                                                   messageId);

                string downloadFolderPath = GraphDownloadAttachmentFiles.CreateDownloadPath(recipientEmail);

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
                        Attachment attachmentData = await GraphDownloadAttachmentFiles.FetchAttachmentData(requestBuilder,
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

                    if (!await GraphMoveEmailsToExport.MoveEmailsToExport(requestBuilder,
                                                                          inMessage.Id,
                                                                          inMessage.Subject))
                    {
                        return 0;
                    }

                    return await StartAttachmentFilesUplaod(downloadFolderPath, customerId, recipientEmail);
                }
            }
            catch (Exception ex)
            {
                WriteLogClass.WriteToLog(0, $"Exception at DownloadAttachments: {ex.Message}", 0);
                return 0;
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
                    int uploadResult = await SendToWebServiceProject.SendToWebServiceProjectAsync(null,
                                                                                 downloadFolderPath,
                                                                                 customerId,
                                                                                 currentBatchFileNames,
                                                                                 null,
                                                                                 string.Empty,
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
        /// Set the email status as unread after forwarding the email the sender.
        /// </summary>
        /// <param name="requestBuilder"></param>
        /// <param name="messageId"></param>
        /// <returns></returns>
        private static async Task<bool> MarkEmailsAsNotRead(IMailFolderRequestBuilder requestBuilder,
                                                            string messageId)
        {
            Message messageUpdateStatus = new()
            {
                IsRead = false // Set IsRead to false to mark the email as not read
            };

            try
            {
                // Updating the IsRead property to false.
                Message request = await requestBuilder
                                        .Messages[$"{messageId}"]
                                        .Request()
                                        .UpdateAsync(messageUpdateStatus);
                return true;
            }
            catch (Exception ex)
            {
                WriteLogClass.WriteToLog(0, $"Exception at marking emails as not read: {ex.Message}", 0);
                return false;
            }
        }
    }
}
