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
using Microsoft.Graph;

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
            UserConfigReaderClass.Customerdetail clientDetails = jsonData.CustomerDetails!.FirstOrDefault(cid => cid.Id == customerId)!;
            // Loading the accepted extension list.
            List<string> acceptedExtentions = clientDetails.DocumentDetails.DocumentExtensions;
            // Creating the list of file in the local download folder.
            string[] downloadedFiles = System.IO.Directory.GetFiles(filePath, "*.*", SearchOption.TopDirectoryOnly)
                                                          .Where(f => acceptedExtentions.IndexOf(Path.GetExtension(f).ToLower()) >= 0)
                                                          .ToArray();

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
                    fileList.Add(new TpsJasonStringClass.FileList() { Name = Path.GetFileName(file), Data = Convert.ToBase64String(System.IO.File.ReadAllBytes(file)) });
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

                returnResult = await SendFilesToRest(ftpConnect,
                                                    jsonResult,
                                                    filesToSend[0],
                                                    customerId,
                                                    customerProjectId,
                                                    customerQueue,
                                                    fileList.Count,
                                                    ftpFileList,
                                                    localFileList,
                                                    fileList.Select(f => f.Name).ToArray(),
                                                    clientOrgNo);

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
                                                        IEnumerable<string> jsonFileList,
                                                        string clientOrgNo)
        {
            try
            {
                // Loads all the details from the customer details Json file.
                UserConfigReaderClass.CustomerDetailsObject jsonData = UserConfigReaderClass.ReadUserDotConfig<UserConfigReaderClass.CustomerDetailsObject>();

                // Just select the data corrosponding to the customer ID.
                UserConfigReaderClass.Customerdetail clientDetails = jsonData.CustomerDetails!.FirstOrDefault(cid => cid.Id == customerId)!;

                // Creating rest api request.
                RestClient client = new($"{clientDetails.DomainDetails.MainDomain}");
                RestRequest tpsRequest = new($"{clientDetails.DomainDetails.TpsRequestUrl}")
                {
                    Method = Method.Post,
                    RequestFormat = DataFormat.Json
                };               

                tpsRequest.AddBody(jsonResult);

                RestResponse serverResponse = await client.ExecuteAsync(tpsRequest); // Executes the request and send to the server.
                string dirPath = System.IO.Directory.GetParent(fullFilePath).FullName; // Gets the directory path of the file.

                if (serverResponse.StatusCode != HttpStatusCode.OK)
                {
                    return await ServerOnFail(clientDetails.FileDeliveryMethod.ToLower(),
                                              fullFilePath,
                                              customerId,
                                              clientOrgNo,
                                              ftpConnect,
                                              ftpFileList,
                                              localFileList,
                                              serverResponse.StatusCode,
                                              serverResponse.Content);
                }

                return await ServerOnSuccess(projectId,
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

        private static async Task<int> ServerOnSuccess(string projectId,
                                                       string queue,
                                                       int fileCount,
                                                       string deliveryType,
                                                       string fullFilePath,
                                                       string downloadFolderPath,
                                                       IEnumerable<string> jsonFileList,
                                                       int customerId,
                                                       string clientOrgNo,
                                                       AsyncFtpClient ftpConnect,
                                                       IEnumerable<string> ftpFileList,
                                                       string[] localFileList)
        {
            try
            {
                WriteLogClass.WriteToLog(1, $"Uploaded {fileCount} file to project {projectId} using queue {queue} ....", 4);
                WriteLogClass.WriteToLog(1, $"Uploaded filenames: {WriteNamesToLogClass.GetFileNames(downloadFolderPath)}", 4);

                // This will run if it's not FTP.
                if (deliveryType == DeliveryType.email)
                {
                    if (FolderCleanerClass.GetFolders(fullFilePath, null, null, clientOrgNo))
                    {
                        return 1;
                    }
                }
                else if (deliveryType == DeliveryType.ftp)
                {
                    if (!await FolderCleanerClass.StartFtpFileDelete(ftpConnect, ftpFileList, localFileList))
                    {
                        return -1;
                    }

                    // Deletes the file from local hold folder when sending is successful.
                    if (!FolderCleanerClass.GetFolders(downloadFolderPath, jsonFileList, customerId, null))
                    {
                        return -1;
                    }

                    return 1;
                }
                return 0; // Default return.
            }
            catch (Exception ex)
            {
                WriteLogClass.WriteToLog(1, $"Error in ServerOnSuccess: {ex.Message}", 1);
                return -1;
            }
            
        }

        private static async Task<int> ServerOnFail(string deliveryType,
                                                    string fullFilePath,
                                                    int customerId,
                                                    string clientOrgNo,
                                                    AsyncFtpClient ftpConnect,
                                                    IEnumerable<string> ftpFileList,
                                                    string[] localFileList,
                                                    HttpStatusCode serverStatusCode,
                                                    string serverResponseContent)
        {
            try
            {
                WriteLogClass.WriteToLog(0, $"Server status code: {serverStatusCode}, Server Response Error: {serverResponseContent}", 0);

                if (!HandleErrorFilesClass.MoveAllFilesToErrorFolder(fullFilePath, customerId, clientOrgNo))
                {
                    WriteLogClass.WriteToLog(1, "Moving files failed ....", 1);
                    return -1;
                }

                // This will run if it's not FTP.
                if (deliveryType == DeliveryType.email)
                {
                    if (FolderCleanerClass.GetFolders(fullFilePath, null, null, clientOrgNo))
                    {
                        return 2;
                    }
                }
                else if (deliveryType == DeliveryType.ftp)
                {
                    if (await FolderCleanerClass.StartFtpFileDelete(ftpConnect, ftpFileList, localFileList))
                    {
                        return 2;
                    }
                }
                return 0; // Deafult return
            }
            catch (Exception ex)
            {
                WriteLogClass.WriteToLog(1, $"Error in ServerOnFail: {ex.Message}", 1);
                return -1;
            }
        }

        public static class DeliveryType
        {
            public const string email = "email";
            public const string ftp = "ftp";
        }
    }
}