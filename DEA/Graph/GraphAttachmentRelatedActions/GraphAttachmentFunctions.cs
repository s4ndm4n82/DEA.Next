using DEA.Next.FileOperations.TpsFileFunctions;
using DEA.Next.Graph.GraphEmailActons;
using DEA.Next.HelperClasses.ConfigFileFunctions;
using FileNameCleanerClass;
using GetMailFolderIds;
using GraphEmailFunctions;
using GraphMoveEmailsrClass;
using GraphMoveEmailsToExportClass;
using Microsoft.Graph;
using WriteLog;
using WriteNamesToLog;
using Directory = System.IO.Directory;
using Message = Microsoft.Graph.Message;

namespace DEA.Next.Graph.GraphAttachmentRelatedActions;

internal class GraphAttachmentFunctionsClass
{
    /// <summary>
    ///     As the function name suggest this function is designed to get messages with attachments.
    ///     And then pass it on to the next step which is downloading the attachments.
    /// </summary>
    /// <param name="requestBuilder"></param>
    /// <param name="inEmail"></param>
    /// <param name="deletedItemsId"></param>
    /// <param name="maxMails"></param>
    /// <param name="customerId"></param>
    /// <returns>A bool value (true or false)</returns>
    public static async Task<int> GetMessagesWithAttachments(IMailFolderRequestBuilder requestBuilder,
        string inEmail,
        string deletedItemsId,
        int maxMails,
        Guid customerId)
    {
        try
        {
            // Get the messages with attachments.
            var messages = await requestBuilder
                .Messages
                .Request()
                .Expand("attachments")
                .Top(maxMails)
                .GetAsync();

            // If there are no messages, then return 4.
            if (!messages.Any()) return 4;

            // Process the messages. And adds all the returned results to a list.
            // var results = await Task.WhenAll(messages
            //     .Select(message =>
            //         ProcessMessageAsync(requestBuilder,
            //             message,
            //             inEmail,
            //             deletedItemsId,
            //             customerId)));
            var results = new List<int>();
            foreach (var message in messages)
            {
                var result = await ProcessMessageAsync(requestBuilder, message, inEmail, deletedItemsId, customerId);
                results.Add(result);
            }

            // Error code list.
            int[] errorCodes = [5, 4, 3, 1];

            // Get the first error code from the list.
            var flag = errorCodes.LastOrDefault(code => results.Contains(code));

            // If there is no error code, then return 0.
            flag = flag != 0 ? flag : 0;

            return flag;
        }
        catch (Exception ex)
        {
            // Log the failure and return the error code.
            WriteLogClass.WriteToLog(0,
                $"Exception at GetMessagesWithAttachments: {ex.Message}",
                0);
            return 4; // 4 is an error code.
        }
    }

    /// <summary>
    ///     Start the attachment processing and downloading.
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
        Guid customerId)
    {
        // Get the allowed file extensions.
        var extensions = await UserConfigRetriever.RetrieveDocumentConfigById(customerId);

        // Filter the attachments.
        var attachmentList = GraphDownloadAttachmentFiles.FilterAttachments(message.Attachments, extensions).ToList();

        // If there are attachments.
        if (attachmentList.Count != 0)
            try
            {
                // If there are attachments, download them.
                return await DownloadAttachments(requestBuilder,
                    attachmentList,
                    message,
                    message.Id,
                    customerId);
            }
            catch (Exception ex)
            {
                WriteLogClass.WriteToLog(0,
                    $"Exception at ProcessMessageAsync running attachment download: {ex.Message}", 0);
                return 4;
            }

        try
        {
            // Check if the email has too many replies. If so email will be moved to deleted items.
            if (await CheckEmailChain.CheckEmailChainAsync(requestBuilder, message.Id, deletedItemsId))
            {
                WriteLogClass.WriteToLog(0, $"Email {message.Subject} has too many replies moved to deleted items ....",
                    0);
                return 5;
            }

            // If there are no attachments, forward the email and return 3.
            var (forwardSuccess, forwardResult) = await GraphEmailFunctionsClass.EmailForwarder(requestBuilder,
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
            var markSuccess = await MarkEmailsAsNotRead(requestBuilder, message.Id);

            if (!markSuccess)
            {
                // Log the failure and return an error code
                WriteLogClass.WriteToLog(1, $"Failed to mark email {message.Id} as not read.", 2);
                return 4; // Error code for failure
            }

            // Get the error folder id.
            var destinationId = await GetMailFolderIdsClass.GetErrorFolderId(requestBuilder);

            // Move the email to the error folder. And returns a bool value.
            var moveSuccess = await GraphMoveEmailsFolder.MoveEmailsToAnotherFolder(requestBuilder,
                message.Id,
                destinationId);

            if (moveSuccess) return 3; // Error code for no attachments

            // Log the failure and return an error code
            WriteLogClass.WriteToLog(1, $"Failed to move email {message.Id} to error folder.", 2);
            return 2; // Error code for failure
        }
        catch (Exception ex)
        {
            WriteLogClass.WriteToLog(0, $"Exception at ProcessMessageAsync: {ex.Message}", 0);
            return 2;
        }
    }

    /// <summary>
    ///     This function would start downloading the attachments. But only download attachments which are above
    ///     10kB (10240 Bytes). But any PDF file will be downloaded regardless of the file size.
    /// </summary>
    /// <param name="requestBuilder"></param>
    /// <param name="inMessage"></param>
    /// <param name="attachmentList"></param>
    /// <param name="messageId"></param>
    /// <param name="customerId"></param>
    /// <returns>A bool value (true or false)</returns>
    private static async Task<int> DownloadAttachments(IMailFolderRequestBuilder requestBuilder,
        List<Attachment> attachmentList,
        Message inMessage,
        string messageId,
        Guid customerId)
    {
        try
        {
            var downloadCount = 0;

            var recipientEmail = await GraphDownloadAttachmentFiles.DetermineRecipientEmail(requestBuilder,
                messageId,
                customerId);

            var downloadFolderPath = GraphDownloadAttachmentFiles.CreateDownloadPath(recipientEmail);

            if (attachmentList.Count == 0)
            {
                WriteLogClass.WriteToLog(1, "No attachments to download in attachmentList ....", 2);
                return -1;
            }

            List<string> attachmentFileNameList = [];

            foreach (var attachment in attachmentList)
                try
                {
                    var attachmentData = await GraphDownloadAttachmentFiles.FetchAttachmentData(requestBuilder,
                        inMessage.Id,
                        attachment.Id);

                    if (!await GraphDownloadAttachmentFiles.SaveAttachmentToFile(attachmentData,
                            downloadFolderPath,
                            FileNameCleaner.FileNameCleanerFunction(attachment.Name))) continue;
                    attachmentFileNameList.Add(attachment.Name);
                    downloadCount++;
                }
                catch (Exception ex)
                {
                    WriteLogClass.WriteToLog(0,
                        $"Exception when processing attachment {inMessage.Subject}: {ex.Message}", 0);
                }

            if (attachmentList.Count == downloadCount)
            {
                WriteLogClass.WriteToLog(1,
                    $"Downloaded file names: {WriteNamesToLogClass.GetFileNames(attachmentFileNameList.ToArray())}",
                    2);

                if (await GraphMoveEmailsToExport.MoveEmailsToExport(requestBuilder,
                        inMessage.Id,
                        inMessage.Subject))
                    return await StartAttachmentFilesUpload(customerId,
                        downloadFolderPath,
                        recipientEmail,
                        inMessage.Subject);

                WriteLogClass.WriteToLog(0, $"Failed to move email {inMessage.Subject} to export folder ....", 2);
                return 2;
            }
        }
        catch (Exception ex)
        {
            WriteLogClass.WriteToLog(0, $"Exception at DownloadAttachments: {ex.Message}", 0);
            return 2;
        }

        return 0;
    }

    /// <summary>
    ///     Batch file upload function. The while loop will upload file until it ends.
    /// </summary>
    /// <param name="downloadFolderPath">Local download folder path.</param>
    /// <param name="customerId">Customer ID</param>
    /// <param name="toEmail">Email the client has sent email to.</param>
    /// <param name="emailSubject">Clients email subject</param>
    /// <returns></returns>
    private static async Task<int> StartAttachmentFilesUpload(Guid customerId,
        string downloadFolderPath,
        string toEmail,
        string emailSubject)
    {
        try
        {
            if (!Directory.Exists(downloadFolderPath))
            {
                WriteLogClass.WriteToLog(0, $"Directory not found: {downloadFolderPath}", 0);
                return -1;
            }

            var successfulUpload = 0; // Successful upload count.

            // Reads the ClientConfig.json file. 
            var clientDetails = await UserConfigRetriever.RetrieveUserConfigById(customerId);

            // Setting the batch count index 
            var batchCurrentIndex = 0;

            DirectoryInfo downloadDirectoryInfo = new(downloadFolderPath);
            var downloadedFileNameList = downloadDirectoryInfo.GetFiles();

            while (batchCurrentIndex < downloadedFileNameList.Length)
            {
                var currentBatchFileNames = downloadedFileNameList
                    .Skip(batchCurrentIndex)
                    .Take(clientDetails.MaxBatchSize)
                    .Select(file => file.Name)
                    .ToArray();

                // Create a single task that uploads all files in the batch.
                var uploadResult = await SendToWebServiceProject.SendToWebServiceProjectAsync(null,
                    null,
                    customerId,
                    currentBatchFileNames,
                    null,
                    downloadFolderPath,
                    string.Empty,
                    toEmail,
                    emailSubject);

                if (uploadResult != 1)
                {
                    WriteLogClass.WriteToLog(0, $"Upload failed with result: {uploadResult}", 0);
                    successfulUpload = 4;
                }
                else
                {
                    successfulUpload = uploadResult;
                }

                // Increment the batch index
                batchCurrentIndex += clientDetails.MaxBatchSize;
            }

            return successfulUpload;
        }
        catch (Exception ex)
        {
            WriteLogClass.WriteToLog(0, $"Exception at StartAttachmentFilesUpload: {ex.Message}", 0);
            return 2;
        }
    }

    /// <summary>
    ///     Set the email status as unread after forwarding the email the sender.
    /// </summary>
    /// <param name="requestBuilder"></param>
    /// <param name="messageId"></param>
    /// <returns></returns>
    private static async Task<bool> MarkEmailsAsNotRead(IMailFolderRequestBuilder requestBuilder,
        string messageId)
    {
        Message messageUpdateStatus = new()
        {
            IsRead = false // Set IsRead to false mark the email as not read
        };

        try
        {
            // Updating the IsRead property to false.
            await requestBuilder
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