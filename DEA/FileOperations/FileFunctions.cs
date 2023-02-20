using System.Net;
using Newtonsoft.Json;
using UserConfigReader;
using RestSharp;
using TpsJsonString;
using WriteLog;
using FolderCleaner;
using System.Diagnostics.CodeAnalysis;
using FluentFTP;

namespace FileFunctions
{
    internal class FileFunctionsClass
    {
        public static async Task<bool> SendToWebService(AsyncFtpClient ftpConnect,
                                                        string filePath,
                                                        int customerId,
                                                        IEnumerable<string> ftpFileList,
                                                        string[] localFileList,
                                                        string recipientEmail)
        {
            WriteLogClass.WriteToLog(3, "Starting file upload process .... ", 4);

            UserConfigReaderClass.CustomerDetailsObject jsonData = UserConfigReaderClass.ReadAppDotConfig<UserConfigReaderClass.CustomerDetailsObject>();
            UserConfigReaderClass.Customerdetail clientDetails = jsonData.CustomerDetails!.FirstOrDefault(cid => cid.id == customerId)!;

            string[] downloadedFiles = Directory.GetFiles(filePath);
            string clientOrg = clientDetails.ClientOrgNo!;

            if (!string.IsNullOrEmpty(recipientEmail))
            {
                clientOrg = recipientEmail.Split('@')[0];
            }

            if (await MakeJsonRequest(ftpConnect,
                                      clientDetails.Token!,
                                      clientDetails.UserName!,
                                      clientDetails.TemplateKey!,
                                      clientDetails.Queue!,
                                      clientDetails.ProjetID!,
                                      clientOrg,
                                      clientDetails.ClientIdField!,
                                      downloadedFiles,
                                      ftpFileList,
                                      localFileList))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private static async Task<bool> MakeJsonRequest(AsyncFtpClient ftpConnect,
                                                        string customerToken,
                                                        string customerUserName,
                                                        string customerTemplateKey,
                                                        string customerQueue,
                                                        string customerProjectId,
                                                        string clientOrgNo,
                                                        string clientIdField,
                                                        string[] filesToSend,
                                                        IEnumerable<string> ftpFileList,
                                                        string[] localFileList)
        {
            // Creating the file list to be added to the Json request.
            List<TpsJasonStringClass.FileList> fileList = new();
            foreach (var file in filesToSend)
            {   
                fileList.Add(new TpsJasonStringClass.FileList() { Name = Path.GetFileName(file), Data = Convert.ToBase64String(File.ReadAllBytes(file)) });
            }

            // Creating the field list to be added to the Json request.
            List<TpsJasonStringClass.FieldList> idField = new()
            {
                new TpsJasonStringClass.FieldList() { Name = clientIdField, Value = clientOrgNo }
            };

            TpsJasonStringClass.TpsJsonObject TpsJsonRequest = new()
            {
                Token = $"{customerToken}",
                Username = $"{customerUserName}",
                TemplateKey = $"{customerTemplateKey}",
                Queue = $"{customerQueue}",
                ProjectID = $"{customerProjectId}",
                Fields = idField,
                Files = fileList
            };

            var result = JsonConvert.SerializeObject(TpsJsonRequest);

            try
            {
                if (await SendFilesToRest(ftpConnect, result, filesToSend[0], customerProjectId, customerQueue, filesToSend.Length, ftpFileList, localFileList))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                WriteLogClass.WriteToLog(3, $"Exception at Json serialization: {ex.Message}", 4);
                return false;
            }
            
        }

        private static async Task<bool> SendFilesToRest(AsyncFtpClient ftpConnect,
                                                        string jsonResult,
                                                        [NotNull]string fullFilePath,
                                                        string projectId,
                                                        string queue,
                                                        int fileCount,
                                                        IEnumerable<string> ftpFileList,
                                                        string[] localFileList)
        {
            try
            {
                // Creating rest api request.
                var client = new RestClient("https://capture.exacta.no/");

                //var tpsRequest = new RestRequest("tps_processing/Import?");
                var tpsRequest = new RestRequest("tps_test_processing/Import?"); // Test service. Uncomment the above one and comment this one when putting to production.
                tpsRequest.Method = Method.Post;
                tpsRequest.RequestFormat = DataFormat.Json;
                tpsRequest.AddBody(jsonResult);

                var serverResponse = await client.ExecuteAsync(tpsRequest); // Executes the request and send to the server.

                if (serverResponse.StatusCode == HttpStatusCode.OK)
                {
                    WriteLogClass.WriteToLog(3, $"Uploaded {fileCount} file to project {projectId} using queue {queue} ....", 4);

                    /* Uncomment this area when deploying to production
                    if (await FolderCleanerClass.GetFtpPathAsync(ftpConnect, ftpFileList, localFileList))
                    {
                        // Deletes the file from local hold folder when sending is successful.
                        return FolderCleanerClass.GetFolders(fullFilePath);
                    }*/

                    if (ftpConnect == null)
                    {
                        return FolderCleanerClass.GetFolders(fullFilePath, "email");
                    }
                    return false;
                }
                else
                {
                    WriteLogClass.WriteToLog(3, $"Server status code: {serverResponse.StatusCode}, Server Response Error: {serverResponse.Content}", 4);
                    return false;
                }
            }
            catch (Exception ex)
            {
                WriteLogClass.WriteToLog(3, $"Exception at rest sharp request: {ex.Message}", 4);
                return false;
            }            
        }
    }
}