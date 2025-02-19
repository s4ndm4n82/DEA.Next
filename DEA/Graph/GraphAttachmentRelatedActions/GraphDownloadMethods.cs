using DEA.Next.Graph.GraphHelperClasses;
using FileNameCleanerClass;
using Microsoft.Graph;
using WriteLog;
using WriteNamesToLog;

namespace DEA.Next.Graph.GraphAttachmentRelatedActions;

public static class GraphDownloadMethods
{
    public static async Task<List<AttachmentFile>> DownloadAttachmentsEmailBodySendAsync(
        IMailFolderRequestBuilder requestBuilder,
        List<Attachment> attachments,
        Guid customerId,
        string recipientEmail,
        string messageId)
    {
        try
        {
            var downloadFilePath = GraphDownloadAttachmentFiles.CreateDownloadPath(recipientEmail);

            var downloadedFileList = new List<AttachmentFile>();

            foreach (var attachment in attachments)
            {
                var attachmentData = await GraphDownloadAttachmentFiles.FetchAttachmentData(requestBuilder,
                    messageId,
                    attachment.Id);

                var fileName = FileNameCleaner.FileNameCleanerFunction(attachment.Name);
                var fullPath = Path.Combine(downloadFilePath, fileName);

                if (!await GraphDownloadAttachmentFiles.SaveAttachmentToFile(attachmentData,
                        downloadFilePath,
                        fileName)) continue;

                downloadedFileList.Add(new AttachmentFile
                {
                    FileName = fileName,
                    FullPath = fullPath
                });
            }

            WriteLogClass.WriteToLog(1,
                $"Downloaded file names: {
                    WriteNamesToLogClass.GetFileNames(downloadedFileList.Select(f => f.FileName).ToArray())
                } ....",
                2);

            return downloadedFileList;
        }
        catch (Exception e)
        {
            WriteLogClass.WriteToLog(1, $"Error downloading attachments body: {e.Message}", 2);
            throw;
        }
    }
}