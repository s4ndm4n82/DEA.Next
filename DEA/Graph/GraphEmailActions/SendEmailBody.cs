using DEA.Next.FileOperations.TpsFileFunctions;
using Microsoft.Graph;
using WriteLog;

namespace DEA.Next.Graph.GraphEmailActions;

public static class SendEmailBody
{
    public static async Task<int> SendEmailBodyStartAsync(IMailFolderRequestBuilder requestBuilder,
        Guid customerId,
        Message message)
    {
        WriteLogClass.WriteToLog(1, $"Reading email body from {message.Subject} ....", 2);

        var result = await SendToWebServiceEmailBody.SendEmailBodyStartAsync(requestBuilder,
            customerId,
            message);

        return result ? 1 : 2;
    }
}