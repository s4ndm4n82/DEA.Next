using DEA.Next.FileOperations.TpsServerResponseFunctions;
using FluentFTP;
using Renci.SshNet;
using RestSharp;
using System.Net;
using DEA.Next.HelperClasses.ConfigFileFunctions;

namespace DEA.Next.FileOperations.TpsFileUploadFunctions;

/// <summary>
/// Send the created Json request to the TPS api and upload the file details.
/// </summary>
internal class SendFilesToRestApiDataFile
{
    public static async Task<int> SendFilesToRestDataFileAsync(AsyncFtpClient? ftpConnect,
        SftpClient? sftpConnect,
        Guid customerId,
        string jsonRequest,
        string downloadFolderPath,
        string fileName,
        string[] ftpFileList,
        string[] localFileList)
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

        tpsRequest.AddBody(jsonRequest);
        var serverResponse = await client.ExecuteAsync(tpsRequest); // Executes the request and send to the server.

        if (serverResponse.StatusCode == HttpStatusCode.OK)
            return await TpsServerOnSuccess.ServerOnSuccessDataFileAsync(ftpConnect,
                sftpConnect,
                customerId,
                fileName,
                downloadFolderPath,
                ftpFileList,
                localFileList);
        
        if (serverResponse.Content != null)
            return await TpsServerOnFaile.ServerOnFailDataFileAsync(ftpConnect,
                sftpConnect,
                customerId,
                downloadFolderPath,
                serverResponse.Content,
                ftpFileList,
                localFileList,
                serverResponse.StatusCode);

        return await TpsServerOnSuccess.ServerOnSuccessDataFileAsync(ftpConnect,
            sftpConnect,
            customerId,
            fileName,
            downloadFolderPath,
            ftpFileList,
            localFileList);
    }

}