using System.Net;
using Newtonsoft.Json;
using RestSharp;
using TpsJsonString;
using WriteLog;
using WriteNamesToLog;
using FolderCleaner;
using HandleErrorFiles;
using FluentFTP;
using UserConfigSetterClass;
using UserConfigRetriverClass;

namespace FileFunctions
{
    internal class FileFunctionsClass
    {
        public static async Task<int> SendToWebService(AsyncFtpClient ftpConnect,
                                                        string filePath,
                                                        int customerId,
                                                        string[] ftpFileList,
                                                        string[] localFileList,
                                                        string ftpFolderName,
                                                        string recipientEmail)
        {
            try
            {
                WriteLogClass.WriteToLog(1, "Starting file upload process .... ", 4);
                
                UserConfigSetter.Customerdetail clientDetails = await UserConfigRetriver.RetriveUserConfigById(customerId);

                string clientOrg = clientDetails.ClientOrgNo;

                if (clientDetails.FtpDetails.FtpFolderLoop == 1)
                {
                    clientOrg = ftpFolderName;
                }

                if (clientDetails.SendEmail == 1)
                {
                    clientOrg = recipientEmail;
                }

                // Loading the accepted extension list.
                List<string> acceptedExtentions = clientDetails
                                                  .DocumentDetails
                                                  .DocumentExtensions
                                                  .Select(e => e.ToLower())
                                                  .ToList();

                // Creating the list of file in the local download folder.
                string[] downloadedFiles = Directory.GetFiles(filePath, "*.*", SearchOption.TopDirectoryOnly)
                                                              .Where(f => acceptedExtentions.Contains(Path.GetExtension(f).ToLower()))
                                                              .Where(f => ftpFileList.Any(g => Path.GetFileNameWithoutExtension(f).Equals(Path.GetFileNameWithoutExtension(g), StringComparison.OrdinalIgnoreCase)))
                                                              .ToArray();

                if (!acceptedExtentions.Any())
                {
                    WriteLogClass.WriteToLog(1, "No matching files in the download list ....", 1);
                    return -1;
                }

                int returnResult = await MakeJsonRequest(ftpConnect,
                                          customerId,
                                          clientDetails.Token,
                                          clientDetails.UserName,
                                          clientDetails.TemplateKey,
                                          clientDetails.Queue,
                                          clientDetails.ProjetID,
                                          clientOrg,
                                          clientDetails.ClientIdField,
                                          downloadedFiles,
                                          ftpFileList,
                                          localFileList);

                return returnResult;
            }
            catch (Exception ex)
            {
                WriteLogClass.WriteToLog(1, $"Exception at SendToWebService: {ex.Message}", 1);
                return -1;
            }
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
                                                        string[] ftpFileList,
                                                        string[] localFileList)
        {
            int returnResult = 0;
            try
            {
                // Creating the file list to be added to the Json request.
                List<TpsJasonStringClass.FileList> jsonFileList = new();
                foreach (var file in filesToSend)
                {
                    jsonFileList.Add(new TpsJasonStringClass.FileList() { Name = Path.GetFileName(file), Data = Convert.ToBase64String(File.ReadAllBytes(file)) });
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
                    Files = jsonFileList
                };

                string jsonResult = JsonConvert.SerializeObject(TpsJsonRequest, Formatting.Indented);

                returnResult = await SendFilesToRest(ftpConnect,
                                                    jsonResult,
                                                    filesToSend[0],
                                                    customerId,
                                                    customerProjectId,
                                                    customerQueue,
                                                    jsonFileList.Count,
                                                    ftpFileList,
                                                    localFileList,
                                                    jsonFileList.Select(f => f.Name).ToArray(),
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
                                                       string[] jsonFileList,
                                                       int customerId,
                                                       string clientOrgNo,
                                                       AsyncFtpClient ftpConnect,
                                                       string[] ftpFileList,
                                                       string[] localFileList)
        {
            try
            {
                WriteLogClass.WriteToLog(1, $"Uploaded {fileCount} file to project {projectId} using queue {queue} ....", 4);
                WriteLogClass.WriteToLog(1, $"Uploaded filenames: {WriteNamesToLogClass.GetFileNames(jsonFileList)}", 4);

                // This will run if it's not FTP.
                if (deliveryType == DeliveryType.email)
                {   
                    if (! await FolderCleanerClass.GetFolders(fullFilePath, jsonFileList, null, clientOrgNo, DeliveryType.email))
                    {
                        return -1;
                    }
                }
                else if (deliveryType == DeliveryType.ftp)
                {
                    if (!await FolderCleanerClass.StartFtpFileDelete(ftpConnect, ftpFileList, localFileList))
                    {
                        return -1;
                    }

                    // Deletes the file from local hold folder when sending is successful.
                    if (!await FolderCleanerClass.GetFolders(downloadFolderPath, jsonFileList, customerId, null, DeliveryType.ftp))
                    {
                        return -1;
                    }                    
                }
                return 1;
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
                                                    string[] ftpFileList,
                                                    string[] localFileList,
                                                    HttpStatusCode serverStatusCode,
                                                    string serverResponseContent)
        {
            try
            {
                WriteLogClass.WriteToLog(0, $"Server status code: {serverStatusCode}, Server Response Error: {serverResponseContent}", 0);

                if (!await HandleErrorFilesClass.MoveFilesToErrorFolder(fullFilePath, ftpFileList, customerId, clientOrgNo))
                {
                    WriteLogClass.WriteToLog(1, "Moving files failed ....", 1);
                    return -1;
                }

                // This will run if it's not FTP.
                if (deliveryType == DeliveryType.email)
                {
                    if (await FolderCleanerClass.GetFolders(fullFilePath, ftpFileList , null, clientOrgNo, DeliveryType.email))
                    {
                        return 2;
                    }
                }
                else if (deliveryType == DeliveryType.ftp)
                {
                    if (await FolderCleanerClass.StartFtpFileDelete(ftpConnect, ftpFileList, localFileList))
                    {
                        if (FolderCleanerClass.DeleteEmptyFolders(fullFilePath))
                        {
                            return 2;
                        }
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