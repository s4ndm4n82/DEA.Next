using DEA.Next.FileOperations.TpsServerReponseFunctions;
using FluentFTP;
using RestSharp;
using System.Net;
using UserConfigRetriverClass;
using UserConfigSetterClass;

namespace DEA.Next.FileOperations.TpsFileUploadFunctions
{
    internal class SendFilesToRestApiDataFile
    {
        public static async Task<int> SendFilesToRestProjectAsync(AsyncFtpClient ftpConnect,
                                                                  int customerId,
                                                                  string jsonResult,
                                                                  string fullFilePath,
                                                                  string fileName,
                                                                  string customerOrg,
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

            tpsRequest.AddBody(jsonResult);
            RestResponse serverResponse = await client.ExecuteAsync(tpsRequest); // Executes the request and send to the server.
            string localFolderPath = Directory.GetParent(fullFilePath).FullName; // Gets the directory path of the file.

            if (serverResponse.StatusCode != HttpStatusCode.OK)
            {
                return await TpsServerOnFaile.ServerOnFail(clientDetails.FileDeliveryMethod, localFolderPath, customerId, fileName, ftpConnect, ftpFileList, localFileList, serverResponse.StatusCode, serverResponse.Content);
            }

            //return await TpsServerOnSuccess.ServerOnSuccess(null, null, ftpFileList.Count(), clientDetails.FileDeliveryMethod, fullFilePath, localFolderPath);

            return 0;
        }

    }
}
