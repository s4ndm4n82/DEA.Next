using DEA.Next.FileOperations.TpsJsonStringCreatorFunctions;
using DEA.Next.Graph.GraphAttachmentRelatedActions;
using DEA.Next.HelperClasses.ConfigFileFunctions;
using Microsoft.Graph;

namespace DEA.Next.FileOperations.TpsFileFunctions;

public static class SendToWebServiceEmailBody
{
    public static async Task<bool> SendEmailBodyStartAsync(IMailFolderRequestBuilder requestBuilder,
        Guid customerId,
        Message message)
    {
        var clientDetails = await UserConfigRetriever.RetrieveUserConfigById(customerId);
        var recipientEmail = await GraphDownloadAttachmentFiles.DetermineRecipientEmail(requestBuilder,
            message.Id,
            customerId);

        return await MakeJsonRequestEmailBody.MakeJsonRequestEmailBodyAsync(requestBuilder,
            customerId,
            message,
            recipientEmail);
    }
}