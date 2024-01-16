using DEA.Next.FileOperations.TpsServerReponseFunctions;
using FluentFTP;
using RestSharp;
using System.Net;
using UserConfigRetriverClass;
using UserConfigSetterClass;
using WriteLog;

namespace DEA.Next.FileOperations.TpsFileUploadFunctions
{
    internal class SendFilesToRestApiProject
    {
        public static async Task<int> SendFilesToRestProjectAsync(AsyncFtpClient ftpConnect,
                                                                  string jsonResult,
                                                                  string fullFilePath,
                                                                  int customerId,
                                                                  string projectId,
                                                                  string queue,
                                                                  int fileCount,
                                                                  string[] ftpFileList,
                                                                  string[] localFileList,
                                                                  string[] jsonFileList,
                                                                  string clientOrgNo)
        {
            try
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
                string dirPath = Directory.GetParent(fullFilePath).FullName; // Gets the directory path of the file.

                if (serverResponse.StatusCode != HttpStatusCode.OK)
                {
                    return await TpsServerOnFaile.ServerOnFail(clientDetails.FileDeliveryMethod.ToLower(),
                                                               fullFilePath,
                                                               customerId,
                                                               clientOrgNo,
                                                               ftpConnect,
                                                               ftpFileList,
                                                               localFileList,
                                                               serverResponse.StatusCode,
                                                               serverResponse.Content);
                }

                return await TpsServerOnSuccess.ServerOnSuccess(projectId,
                                                                queue,
                                                                fileCount,
                                                                clientDetails.FileDeliveryMethod.ToLower(),
                                                                fullFilePath,
                                                                dirPath,
                                                                jsonFileList,
                                                                customerId,
                                                                clientOrgNo,
                                                                ftpConnect,
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
}
