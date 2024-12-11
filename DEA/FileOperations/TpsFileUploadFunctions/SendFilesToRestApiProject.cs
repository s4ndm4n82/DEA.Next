using DEA.Next.FileOperations.TpsServerResponseFunctions;
using FluentFTP;
using Renci.SshNet;
using RestSharp;
using System.Net;
using DEA.Next.HelperClasses.ConfigFileFunctions;
using WriteLog;

namespace DEA.Next.FileOperations.TpsFileUploadFunctions;

internal class SendFilesToRestApiProject
{
    public static async Task<int> SendFilesToRestProjectAsync(Guid customerId,
        AsyncFtpClient ftpConnect,
        SftpClient sftpConnect,
        string jsonResult,
        string fullFilePath,
        int fileCount,
        string[] ftpFileList,
        string[] localFileList,
        string[] jsonFileList,
        string clientOrgNo)
    {
        try
        {
            var customerDetails = await UserConfigRetriever.RetrieveUserConfigById(customerId);

            var index = customerDetails.Domain.LastIndexOf('/');
            var mainDomain = customerDetails.Domain[..index];
            var query = customerDetails.Domain[(index + 1)..];

            // Creating rest api request.
            var client = new RestClient(mainDomain);
            var tpsRequest = new RestRequest(query)
            {
                Method = Method.Post,
                RequestFormat = DataFormat.Json
            };

            tpsRequest.AddBody(jsonResult);
            var serverResponse = await client.ExecuteAsync(tpsRequest); // Executes the request and send to the server.
            
            var dirPath = Directory.GetParent(fullFilePath)?.FullName; // Gets the directory path of the file.

            if (serverResponse.StatusCode != HttpStatusCode.OK)
            {
                return await TpsServerOnFaile.ServerOnFailProjectsAsync(customerDetails.FileDeliveryMethod.ToLower(),
                    fullFilePath,
                    customerId,
                    clientOrgNo,
                    ftpConnect,
                    sftpConnect,
                    ftpFileList,
                    localFileList,
                    serverResponse.StatusCode,
                    serverResponse.Content);
            }

            return await TpsServerOnSuccess.ServerOnSuccessProjectAsync(customerDetails.ProjectId,
                customerDetails.Queue,
                fileCount,
                customerDetails.FileDeliveryMethod.ToLower(),
                fullFilePath,
                dirPath,
                jsonFileList,
                customerId,
                clientOrgNo,
                ftpConnect,
                sftpConnect,
                ftpFileList,
                localFileList);
        }
        catch (Exception ex)
        {
            WriteLogClass.WriteToLog(0, $"Exception at SendFilesToRest function: {ex.Message}", 0);
            return 0;
        }
    }
}