using DEA.Next.FileOperations.TpsFileFunctions;
using DEA.Next.Graph.GraphAttachmentRelatedActions;
using DEA.Next.HelperClasses.ConfigFileFunctions;
using Microsoft.Graph;

namespace DEA.Next.Graph.GraphEmailActions;

public static class SendEmailBody
{
    public static async Task<bool> SendEmailBodyStartAsync(IMailFolderRequestBuilder requestBuilder,
        Guid customerId,
        Message message)
    {
        var clientDetails = await UserConfigRetriever.RetrieveUserConfigById(customerId);
        var recipientEmail = await GraphDownloadAttachmentFiles.DetermineRecipientEmail(requestBuilder,
            message.Id,
            customerId);

        return await SendToWebServiceEmailBody.SendEmailBodyStartAsync(requestBuilder,
            customerId,
            message);
    }
}