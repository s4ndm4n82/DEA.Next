using DEA.Next.FileOperations.TpsFileFunctions;
using DEA.Next.Graph.GraphAttachmentRelatedActions;
using DEA.Next.HelperClasses.ConfigFileFunctions;
using Microsoft.Graph;
using WriteLog;

namespace DEA.Next.Graph.GraphEmailActions;

public static class SendEmailBody
{
    public static async Task<int> SendEmailBodyStartAsync(IMailFolderRequestBuilder requestBuilder,
        Guid customerId,
        Message message)
    {
        var result = false;
        var extensions = await UserConfigRetriever.RetrieveDocumentConfigById(customerId);
        WriteLogClass.WriteToLog(1, $"Reading email body from {message.Subject} ....", 2);

        // Check for attachments
        var attachments = GraphDownloadAttachmentFiles.FilterAttachments(message.Attachments, extensions).ToList();

        if (attachments.Count == 0)
            result = await SendToWebServiceEmailBody.SendEmailBodyStartAsync(requestBuilder,
                customerId,
                message);
        else
            result = await SendToWebServiceEmailBody.SendEmailBodyWithAttachmentsStartAsync(requestBuilder,
                customerId,
                message,
                attachments);


        return result ? 1 : 2;
    }
}