using DEA.Next.FileOperations.TpsServerResponseFunctions;
using FluentFTP;
using Renci.SshNet;
using RestSharp;
using System.Net;
using DEA.Next.HelperClasses.ConfigFileFunctions;
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
                UserConfigSetter.Customerdetail customerDetails = await UserConfigRetriever.RetrieveUserConfigById(customerId);

                // Creating rest api request.
                RestClient client = new($"{customerDetails.DomainDetails.MainDomain}");
                RestRequest tpsRequest = new($"{customerDetails.DomainDetails.TpsRequestUrl}")
                {
                    Method = Method.Post,
                    RequestFormat = DataFormat.Json
                };

                tpsRequest.AddBody(jsonResult);

                RestResponse serverResponse = await client.ExecuteAsync(tpsRequest); // Executes the request and send to the server.

                // Gets the directory path of the file.
                var parentDirectory = Directory.GetParent(fullFilePath);
                var dirPath = parentDirectory != null ? parentDirectory.FullName : string.Empty;

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
                                                                            serverResponse.Content ?? "No content found");
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
}
