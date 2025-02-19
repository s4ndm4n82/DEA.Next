using DEA.Next.FileOperations.TpsJsonStringCreatorFunctions;
using DEA.Next.Graph.GraphHelperClasses;
using DEA.Next.HelperClasses.ConfigFileFunctions;
using Microsoft.Graph;
using WriteLog;

namespace DEA.Next.Graph.GraphAttachmentRelatedActions;

public class GraphUploadMethods
{
    public static async Task<bool> UploadAttachmentEmailBodySendAsync(IMailFolderRequestBuilder requestBuilder,
        List<AttachmentFile> attachments,
        Message message,
        Guid customerId,
        string recipientEmail)
    {
        try
        {
            var batchIndex = 0;
            var result = false;
            var clientDetails = await UserConfigRetriever.RetrieveUserConfigById(customerId);

            while (batchIndex < attachments.Count)
            {
                var currentBatch = attachments
                    .Skip(batchIndex)
                    .Take(clientDetails.MaxBatchSize)
                    .Select(f => f.FullPath)
                    .ToArray();

                result = await MakeJsonRequestEmailBody.MakeJsonRequestEmailBodyWithAttachmentsAsync(requestBuilder,
                    attachments,
                    customerId,
                    message,
                    recipientEmail);

                batchIndex += clientDetails.MaxBatchSize;
            }

            return result;
        }
        catch (Exception e)
        {
            WriteLogClass.WriteToLog(1, $"Error uploading attachments body: {e.Message}", 2);
            return false;
        }
    }
}