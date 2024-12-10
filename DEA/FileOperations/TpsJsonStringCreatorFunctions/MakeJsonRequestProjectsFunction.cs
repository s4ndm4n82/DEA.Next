using DEA.Next.FileOperations.TpsFileUploadFunctions;
using DEA.Next.HelperClasses.ConfigFileFunctions;
using FluentFTP;
using Newtonsoft.Json;
using Renci.SshNet;
using TpsJsonProjectUploadString;
using WriteLog;

namespace DEA.Next.FileOperations.TpsJsonStringCreatorFunctions;

internal class MakeJsonRequestProjectsFunction
{
    public static async Task<int> MakeJsonRequestProjects(AsyncFtpClient ftpConnect,
        SftpClient sftpConnect,
        Guid customerId,
        string clientOrgNo,
        string[] filesToSend,
        string[] ftpFileList,
        string[] localFileList)
    {
        try
        {
            var customerDetails = await UserConfigRetriever.RetrieveUserConfigById(customerId);

            // Create the file list to be added to the Json request.
            var jsonFileList = MakeJsonRequestHelperClass.ReturnFileList(filesToSend);

            // Create the field list to be added to the Json request.
            var idFields = await MakeJsonRequestHelperClass.ReturnIdFieldList(customerId, clientOrgNo);

            // Create the Json request.
            TpsJsonProjectUploadStringClass.TpsJsonProjectUploadObject tpsJsonRequest = new()
            {
                Token = customerDetails.Token,
                Username = customerDetails.UserName,
                TemplateKey = customerDetails.TemplateKey,
                Queue = customerDetails.Queue,
                ProjectId = customerDetails.ProjectId,
                Fields = idFields,
                Files = jsonFileList
            };

            // Assigning the Json request to a string. To be handed over to the rest api.
            var jsonResult = JsonConvert.SerializeObject(tpsJsonRequest, Formatting.Indented);

            // Send the Json request to the rest api.
            return await SendFilesToRestApiProject.SendFilesToRestProjectAsync(customerId,
                ftpConnect,
                sftpConnect,
                jsonResult,
                filesToSend[0],
                jsonFileList.Count,
                ftpFileList,
                localFileList,
                jsonFileList.Select(f => f.Name).ToArray(),
                clientOrgNo);
        }
        catch (Exception ex)
        {
            WriteLogClass.WriteToLog(0, $"Exception at Json serialization: {ex.Message}", 0);
            return 0;
        }
    }
}