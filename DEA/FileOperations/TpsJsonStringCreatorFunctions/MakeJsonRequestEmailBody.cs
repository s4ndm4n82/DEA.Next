using DEA.Next.FileOperations.TpsFileUploadFunctions;
using DEA.Next.FileOperations.TpsJsonStringClasses;
using DEA.Next.HelperClasses.ConfigFileFunctions;
using Microsoft.Graph;
using Newtonsoft.Json;
using WriteLog;

namespace DEA.Next.FileOperations.TpsJsonStringCreatorFunctions;

public static class MakeJsonRequestEmailBody
{
    public static async Task<bool> MakeJsonRequestEmailBodyAsync(IMailFolderRequestBuilder requestBuilder,
        Guid customerId,
        Message message,
        string recipientEmail)
    {
        try
        {
            var clientDetails = await UserConfigRetriever.RetrieveUserConfigById(customerId);
            var bodyText = message.Body.ContentType == BodyType.Text ? message.Body.Content : message.BodyPreview;

            var emailFieldList = await MakeJsonRequestHelperClass.ReturnEmailBodyFieldList(customerId,
                recipientEmail,
                bodyText);

            var emailFileList = MakeJsonRequestHelperClass.ReturnEmailBodyFileList(message.Subject);

            TpsJsonSendBodyTextClass.TpsJsonSendBodyText tpsJsonRequest = new()
            {
                Token = clientDetails.Token,
                Username = clientDetails.UserName,
                Queue = clientDetails.Queue,
                ProjectId = clientDetails.ProjectId,
                Fields = emailFieldList,
                Files = emailFileList
            };

            var jsonString = JsonConvert.SerializeObject(tpsJsonRequest, Formatting.Indented);

            return await SendBodyTextToRestApi.SendBodyTextToRestAsync(requestBuilder,
                customerId,
                message,
                jsonString);
        }
        catch (Exception ex)
        {
            WriteLogClass.WriteToLog(0, $"Exception at Json serialization: {ex.Message}", 0);
            return false;
        }
    }
}