﻿using DEA.Next.FileOperations.TpsFileUploadFunctions;
using DEA.Next.FileOperations.TpsJsonStringClasses;
using DEA.Next.HelperClasses.ConfigFileFunctions;
using FluentFTP;
using Newtonsoft.Json;
using Renci.SshNet;
using WriteLog;

namespace DEA.Next.FileOperations.TpsJsonStringCreatorFunctions;

/// <summary>
/// Creats the Json request for data file.
/// </summary>
internal class MakeJsonRequestDataFileFunction
{
    public static async Task<int> MakeJsonRequestDataFileAsync(AsyncFtpClient? ftpConnect,
        SftpClient? sftpConnect,
        Guid customerId,
        string localFilePath,
        string[] fileToSend,
        string[] ftpFileList,
        string[] localFileList)
    {
        try
        {
            var customerDetails = await UserConfigRetriever.RetrieveUserConfigById(customerId);

            // Get the filename.
            var fileName = Path.GetFileName(fileToSend.FirstOrDefault());

            // Convert the file to base64 string.
            var fileData = Convert
                .ToBase64String(await File
                    .ReadAllBytesAsync(fileToSend
                        .FirstOrDefault() ?? string.Empty));

            TpsJsonDataFileUploadString.TpsJsonDataFileUploadObject tpsJsonRequest = new()
            {
                Token = customerDetails.Token,
                Username = customerDetails.UserName,
                ID = customerDetails.DocumentId,
                Encoding = customerDetails.DocumentEncoding,
                FileData = fileData,
            };

            // Creat the Json request.
            var jsonRequest = JsonConvert.SerializeObject(tpsJsonRequest, Formatting.Indented);

            if (!string.IsNullOrWhiteSpace(fileName))
                return await SendFilesToRestApiDataFile.SendFilesToRestDataFileAsync(ftpConnect,
                    sftpConnect,
                    customerId,
                    jsonRequest,
                    localFilePath,
                    fileName,
                    ftpFileList,
                    localFileList);
            
            WriteLogClass.WriteToLog(0, "File name is null or whitespace ....", 0);
            return -1;
        }
        catch (Exception ex)
        {
            WriteLogClass.WriteToLog(0, $"Exception at MakeJsonRequestDataFileAsync: {ex.Message}", 0);
            return -1;
        }
    }
}