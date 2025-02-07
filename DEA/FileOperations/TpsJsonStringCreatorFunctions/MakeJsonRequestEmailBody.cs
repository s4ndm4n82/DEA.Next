using DEA.Next.FileOperations.TpsFileUploadFunctions;
using DEA.Next.FileOperations.TpsJsonStringClasses;
using DEA.Next.HelperClasses.ConfigFileFunctions;
using Microsoft.Graph;
using Newtonsoft.Json;
using WriteLog;

namespace DEA.Next.FileOperations.TpsJsonStringCreatorFunctions;

public class MakeJsonRequestEmailBody
{
    public static async Task<bool> MakeJsonRequestEmailBodyAsync(IMailFolderRequestBuilder requestBuilder,
        Guid customerId,
        Message message,
        string recipientEmail)
    {
        try
        {
            var clientDetails = await UserConfigRetriever.RetrieveUserConfigById(customerId);

            var emailFieldList = await MakeJsonRequestHelperClass.ReturnEmailBodyFieldList(customerId,
                recipientEmail,
                message.Body.Content);

            var emailFileList = MakeJsonRequestHelperClass.ReturnEmailBodyFileList();

            TpsJsonSendBodyTextClass.TpsJsonSendBodyText tpsJsonRequest = new()
            {
                Token = clientDetails.Token,
                Username = clientDetails.UserName,
                Queue = clientDetails.Queue,
                ProjectId = clientDetails.ProjectId,
                Fields = emailFieldList,
                Files = emailFileList
            };

            var jsonResult = JsonConvert.SerializeObject(tpsJsonRequest, Formatting.Indented);

            return await SendBodyTextToRestApi.SendBodyTextToRestAsync()
        }
        catch (Exception ex)
        {
            WriteLogClass.WriteToLog(0, $"Exception at Json serialization: {ex.Message}", 0);
            return false;
        }
    }
}