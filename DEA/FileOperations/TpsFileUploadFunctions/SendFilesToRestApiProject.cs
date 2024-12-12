using DEA.Next.FileOperations.TpsServerResponseFunctions;
using FluentFTP;
using Renci.SshNet;
using RestSharp;
using System.Net;
using DEA.Next.Extensions;
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
            var (mainDomain, query) = await customerId.SplitUrl();
            
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

            if (!string.IsNullOrEmpty(dirPath))
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
            
            WriteLogClass.WriteToLog(0, "Directory path is empty ....", 0);
            return 0;

        }
        catch (Exception ex)
        {
            WriteLogClass.WriteToLog(0, $"Exception at SendFilesToRest function: {ex.Message}", 0);
            return 0;
        }
    }
}