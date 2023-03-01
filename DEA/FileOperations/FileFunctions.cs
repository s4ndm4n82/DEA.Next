using System.Net;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;
using UserConfigReader;
using RestSharp;
using TpsJsonString;
using WriteLog;
using WriteNamesToLog;
using FolderCleaner;
using HandleErrorFiles;
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
            WriteLogClass.WriteToLog(1, "Starting file upload process .... ", 4);

            UserConfigReaderClass.CustomerDetailsObject jsonData = UserConfigReaderClass.ReadAppDotConfig<UserConfigReaderClass.CustomerDetailsObject>();
            UserConfigReaderClass.Customerdetail clientDetails = jsonData.CustomerDetails!.FirstOrDefault(cid => cid.id == customerId)!;

            List<string> acceptedExtentions = clientDetails.DocumentDetails!.DocumentExtensions!;

            string[] downloadedFiles = Directory.GetFiles(filePath, "*.*", SearchOption.TopDirectoryOnly).Where(f => acceptedExtentions.IndexOf(Path.GetExtension(f)) >= 0).ToArray();

            string clientOrg = clientDetails.ClientOrgNo!;

            clientOrg = !string.IsNullOrEmpty(recipientEmail) ? recipientEmail : null!;

            if (await MakeJsonRequest(ftpConnect,
                                      customerId,
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
                                                        int customerId,
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

            string result = JsonConvert.SerializeObject(TpsJsonRequest, Formatting.Indented);

            try
            {
                if (await SendFilesToRest(ftpConnect, result, filesToSend[0], customerId, customerProjectId, customerQueue, fileList.Count, ftpFileList, localFileList))
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
                WriteLogClass.WriteToLog(0, $"Exception at Json serialization: {ex.Message}", 0);
                return false;
            }
            
        }

        private static async Task<bool> SendFilesToRest(AsyncFtpClient ftpConnect,
                                                        string jsonResult,
                                                        [NotNull]string fullFilePath,
                                                        int customerId,
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
                var tpsRequest = new RestRequest("tps_test_processing/Import?") // Test service. Uncomment the above one and comment this one when putting to production.
                {
                    Method = Method.Post,
                    RequestFormat = DataFormat.Json
                };
                tpsRequest.AddBody(jsonResult);

                var serverResponse = await client.ExecuteAsync(tpsRequest); // Executes the request and send to the server.

                if (serverResponse.StatusCode == HttpStatusCode.OK)
                {
                    WriteLogClass.WriteToLog(1, $"Uploaded {fileCount} file to project {projectId} using queue {queue} ....", 4);
                    WriteLogClass.WriteToLog(1, $"Uploaded filenames: {WriteNamesToLogClass.GetFileNames(fullFilePath)}", 4);

                    // This will run if it's not FTP.
                    if (ftpConnect == null)
                    {
                        return FolderCleanerClass.GetFolders(fullFilePath, "email");
                    }
                    else
                    {
                        if (await FolderCleanerClass.GetFtpPathAsync(ftpConnect, ftpFileList, localFileList))
                        {
                            // Deletes the file from local hold folder when sending is successful.
                            return FolderCleanerClass.GetFolders(fullFilePath, string.Empty);                           
                        }
                    }
                    return false;
                }
                else
                {
                    WriteLogClass.WriteToLog(0, $"Server status code: {serverResponse.StatusCode}, Server Response Error: {serverResponse.Content}", 0);
                    HandleErrorFilesClass.MoveFilesToErrorFolder(fullFilePath, customerId);
                    return false;
                }
            }
            catch (Exception ex)
            {
                WriteLogClass.WriteToLog(0, $"Exception at rest sharp request: {ex.Message}", 0);
                return false;
            }            
        }
    }
}