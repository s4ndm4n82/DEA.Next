using DEA.Next.FileOperations.TpsJsonStringCreatorFunctions;
using DEA.Next.Graph.GraphAttachmentRelatedActions;
using Microsoft.Graph;
using WriteLog;

namespace DEA.Next.FileOperations.TpsFileFunctions;

public static class SendToWebServiceEmailBody
{
    public static async Task<bool> SendEmailBodyStartAsync(IMailFolderRequestBuilder requestBuilder,
        Guid customerId,
        Message message)
    {
        var recipientEmail = await GraphDownloadAttachmentFiles.DetermineRecipientEmail(requestBuilder,
            message.Id,
            customerId);

        return await MakeJsonRequestEmailBody.MakeJsonRequestEmailBodyAsync(requestBuilder,
            customerId,
            message,
            recipientEmail);
    }

    public static async Task<bool> SendEmailBodyWithAttachmentsStartAsync(IMailFolderRequestBuilder requestBuilder,
        Guid customerId,
        Message message,
        List<Attachment> attachments)
    {
        try
        {
            var recipientEmail = await GraphDownloadAttachmentFiles.DetermineRecipientEmail(requestBuilder,
                message.Id,
                customerId);

            var downloadedFileList = await GraphDownloadMethods.DownloadAttachmentsEmailBodySendAsync(requestBuilder,
                attachments,
                customerId,
                recipientEmail,
                message.Id);

            return await GraphUploadMethods.UploadAttachmentEmailBodySendAsync(requestBuilder,
                downloadedFileList,
                message,
                customerId,
                recipientEmail);
        }
        catch (Exception e)
        {
            WriteLogClass.WriteToLog(0,
                $"Error sending email body with attachments: {e.Message}",
                0);
            return false;
        }
    }
}