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
        public static async Task<int> SendToWebService(AsyncFtpClient ftpConnect,
                                                        string filePath,
                                                        int customerId,
                                                        IEnumerable<string> ftpFileList,
                                                        string[] localFileList,
                                                        string recipientEmail)
        {
            WriteLogClass.WriteToLog(1, "Starting file upload process .... ", 4);

            UserConfigReaderClass.CustomerDetailsObject jsonData = UserConfigReaderClass.ReadUserDotConfig<UserConfigReaderClass.CustomerDetailsObject>();
            UserConfigReaderClass.Customerdetail clientDetails = jsonData.CustomerDetails!.FirstOrDefault(cid => cid.id == customerId)!;

            List<string> acceptedExtentions = clientDetails.DocumentDetails!.DocumentExtensions!;

            string[] downloadedFiles = Directory.GetFiles(filePath, "*.*", SearchOption.TopDirectoryOnly).Where(f => acceptedExtentions.IndexOf(Path.GetExtension(f)) >= 0).ToArray();

            // If recipientEmail not empty clientOrg = revipientEmail.
            // If recipientEmail is empty clientOrg = clientDetails.ClientOrgNo
            string clientOrg = recipientEmail ?? clientDetails.ClientOrgNo ?? throw new Exception("ClientOrg is null.");

            int returnResult = await MakeJsonRequest(ftpConnect,
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
                                      localFileList);

            return returnResult;
        }

        private static async Task<int> MakeJsonRequest(AsyncFtpClient ftpConnect,
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
            int returnResult = 0;
            try
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

                string jsonResult = JsonConvert.SerializeObject(TpsJsonRequest, Formatting.Indented);

                returnResult = await SendFilesToRest(ftpConnect, jsonResult, filesToSend[0], customerId, customerProjectId, customerQueue, fileList.Count, ftpFileList, localFileList, clientOrgNo);

                return returnResult;
            }
            catch (Exception ex)
            {
                WriteLogClass.WriteToLog(0, $"Exception at Json serialization: {ex.Message}", 0);
                return returnResult;
            }
            
        }

        private static async Task<int> SendFilesToRest(AsyncFtpClient ftpConnect,
                                                        string jsonResult,
                                                        [NotNull]string fullFilePath,
                                                        int customerId,
                                                        string projectId,
                                                        string queue,
                                                        int fileCount,
                                                        IEnumerable<string> ftpFileList,
                                                        string[] localFileList,
                                                        string clientOrgNo)
        {
            try
            {
                // Loads all the details from the customer details Json file.
                UserConfigReaderClass.CustomerDetailsObject JsonData = UserConfigReaderClass.ReadUserDotConfig<UserConfigReaderClass.CustomerDetailsObject>();

                // Just select the data corrosponding to the customer ID.
                UserConfigReaderClass.Customerdetail clientDetails = JsonData.CustomerDetails!.FirstOrDefault(cid => cid.id == customerId)!;

                // Creating rest api request.
                RestClient client = new("https://capture.exacta.no/");

                //var tpsRequest = new RestRequest("tps_processing/Import?");
                RestRequest tpsRequest = new("tps_test_processing/Import?") // Test service. Uncomment the above one and comment this one when putting to production.
                {
                    Method = Method.Post,
                    RequestFormat = DataFormat.Json
                };
                tpsRequest.AddBody(jsonResult);

                RestResponse serverResponse = await client.ExecuteAsync(tpsRequest); // Executes the request and send to the server.
                string dirPath = Directory.GetParent(fullFilePath).FullName;

                if (serverResponse.StatusCode == HttpStatusCode.OK)
                {
                    WriteLogClass.WriteToLog(1, $"Uploaded {fileCount} file to project {projectId} using queue {queue} ....", 4);
                    WriteLogClass.WriteToLog(1, $"Uploaded filenames: {WriteNamesToLogClass.GetFileNames(dirPath)}", 4);

                    // This will run if it's not FTP.
                    if (clientDetails.FileDeliveryMethod.ToLower() == "email")
                    {
                        if (FolderCleanerClass.GetFolders(fullFilePath))
                        {
                            return 1;
                        }
                    }
                    else
                    {
                        if (await FolderCleanerClass.GetFtpPathAsync(ftpConnect, ftpFileList, localFileList))
                        {
                            // Deletes the file from local hold folder when sending is successful.
                            if (FolderCleanerClass.GetFolders(dirPath))
                            {
                                return 1;
                            }
                        }
                    }
                    return 0;
                }
                else
                {
                    WriteLogClass.WriteToLog(0, $"Server status code: {serverResponse.StatusCode}, Server Response Error: {serverResponse.Content}", 0);

                    if (HandleErrorFilesClass.MoveFilesToErrorFolder(fullFilePath, customerId, clientOrgNo))
                    {
                        // This will run if it's not FTP.
                        if (clientDetails.FileDeliveryMethod.ToLower() == "email")
                        {
                            if (FolderCleanerClass.GetFolders(fullFilePath))
                            {
                                return 2;
                            }
                        }
                        else
                        {
                            if (await FolderCleanerClass.GetFtpPathAsync(ftpConnect, ftpFileList, localFileList))
                            {
                                return 2;
                            }
                        }
                    }
                    return 0;
                }
            }
            catch (Exception ex)
            {
                WriteLogClass.WriteToLog(0, $"Exception at rest sharp request: {ex.Message}", 0);
                return 0;
            }            
        }
    }
}