﻿using DEA.Next.FileOperations.TpsServerReponseFunctions;
using FluentFTP;
using RestSharp;
using System.Net;
using UserConfigRetriverClass;
using UserConfigSetterClass;

namespace DEA.Next.FileOperations.TpsFileUploadFunctions
{
    /// <summary>
    /// Send the created Json request to the TPS api and upload the file details.
    /// </summary>
    internal class SendFilesToRestApiDataFile
    {
        public static async Task<int> SendFilesToRestDataFileAsync(AsyncFtpClient ftpConnect,
                                                                  int customerId,
                                                                  string jsonReuest,
                                                                  string downloadFolderPath,
                                                                  string fileName,
                                                                  string[] ftpFileList,
                                                                  string[] localFileList)
        {
            UserConfigSetter.Customerdetail clientDetails = await UserConfigRetriver.RetriveUserConfigById(customerId);

            // Creating rest api request.
            RestClient client = new($"{clientDetails.DomainDetails.MainDomain}");
            RestRequest tpsRequest = new($"{clientDetails.DomainDetails.TpsRequestUrl}")
            {
                Method = Method.Post,
                RequestFormat = DataFormat.Json
            };

            tpsRequest.AddBody(jsonReuest);
            RestResponse serverResponse = await client.ExecuteAsync(tpsRequest); // Executes the request and send to the server.

            if (serverResponse.StatusCode != HttpStatusCode.OK)
            {
                return await TpsServerOnFaile.ServerOnFailDataFileAsync(ftpConnect,
                                                                       customerId,
                                                                       downloadFolderPath,
                                                                       serverResponse.Content,
                                                                       ftpFileList,
                                                                       localFileList,
                                                                       serverResponse.StatusCode);
            }

            return await TpsServerOnSuccess.ServerOnSuccessDataFileAsync(ftpConnect,
                                                                         customerId,
                                                                         fileName,
                                                                         downloadFolderPath,
                                                                         ftpFileList,
                                                                         localFileList);
        }

    }
}