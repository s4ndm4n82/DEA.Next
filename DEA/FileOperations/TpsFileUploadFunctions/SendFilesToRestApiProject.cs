using DEA.Next.FileOperations.TpsServerReponseFunctions;
using FluentFTP;
using Renci.SshNet;
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
                                                                  SftpClient sftpConnect,
                                                                  string jsonResult,
                                                                  string fullFilePath,
                                                                  int customerId,
                                                                  int fileCount,
                                                                  string[] ftpFileList,
                                                                  string[] localFileList,
                                                                  string[] jsonFileList,
                                                                  string clientOrgNo)
        {
            try
            {
                UserConfigSetter.Customerdetail customerDetails = await UserConfigRetriver.RetriveUserConfigById(customerId);

                // Creating rest api request.
                RestClient client = new($"{customerDetails.DomainDetails.MainDomain}");
                RestRequest tpsRequest = new($"{customerDetails.DomainDetails.TpsRequestUrl}")
                {
                    Method = Method.Post,
                    RequestFormat = DataFormat.Json
                };

                tpsRequest.AddBody(jsonResult);

                RestResponse serverResponse = await client.ExecuteAsync(tpsRequest); // Executes the request and send to the server.
                string dirPath = Directory.GetParent(fullFilePath).FullName; // Gets the directory path of the file.

                if (serverResponse.StatusCode != HttpStatusCode.OK)
                {
                    return await TpsServerOnFaile.ServerOnFailProjectsAsync(customerDetails.FileDeliveryMethod.ToLower(),
                                                                            fullFilePath,
                                                                            customerId,
                                                                            clientOrgNo,
                                                                            ftpConnect,
                                                                            ftpFileList,
                                                                            localFileList,
                                                                            serverResponse.StatusCode,
                                                                            serverResponse.Content);
                }

                return await TpsServerOnSuccess.ServerOnSuccessProjectAsync(customerDetails.ProjetID,
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
}
