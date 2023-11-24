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
            UserConfigReaderClass.Customerdetail clientDetails = jsonData.CustomerDetails!.FirstOrDefault(cid => cid.Id == customerId)!;

            List<string> acceptedExtentions = clientDetails.DocumentDetails!.DocumentExtensions!;

            string[] downloadedFiles = Directory.GetFiles(filePath, "*.*", SearchOption.TopDirectoryOnly).Where(f => acceptedExtentions.IndexOf(Path.GetExtension(f).ToLower()) >= 0).ToArray();

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
                string dirPath = Directory.GetParent(fullFilePath).FullName; // Gets the directory path of the file.

                if (serverResponse.StatusCode == HttpStatusCode.OK)
                {
                    WriteLogClass.WriteToLog(1, $"Uploaded {fileCount} file to project {projectId} using queue {queue} ....", 4);
                    WriteLogClass.WriteToLog(1, $"Uploaded filenames: {WriteNamesToLogClass.GetFileNames(dirPath)}", 4);

                    /* Below if ... else stament is written using the ternary operator. The function of the code described below.
                     * if the file delivery method is email then it will run the funcation from folder cleaner class which will delete
                     * the file from the hold folder and when it success returns 1. If the condition is not met it'll the function GetFtpPathAsync()
                     * from the FolderCleanerClass and then removes the files from the local hold folder when the sending is successful. And returns 1
                     * on fail returns 0.*/
                    return clientDetails.FileDeliveryMethod.ToLower() == "email" ?
                           FolderCleanerClass.GetFolders(fullFilePath, null, null, clientOrgNo) ? 1 : 0 :
                           await FolderCleanerClass.GetFtpPathAsync(ftpConnect, ftpFileList, localFileList) &&
                           FolderCleanerClass.GetFolders(dirPath, jsonFileList, customerId, null) ? 1 : 0;
                }
                else
                {
                    WriteLogClass.WriteToLog(0, $"Server status code: {serverResponse.StatusCode}, Server Response Error: {serverResponse.Content}", 0);

                    /* Below if ... else stament is written using the ternary operator. The function of the code described below.
                     * If the file upload is not successful then it will run the funcation from HandleErrorFilesClass which will move the files
                     * to the "Error" folder and keeps the files there. If the file delivery method is email then it will run the funcation from
                     * FolderCleanerClass which will delete the file from the hold folder and when it success returns 2. If the condition is not met
                     * it'll the function GetFtpPathAsync() from the FolderCleanerClass and then removes the files from the local hold folder when
                     * the sending is successful. And returns 2 on fail returns 0.*/
                    return HandleErrorFilesClass.MoveAllFilesToErrorFolder(fullFilePath, customerId, clientOrgNo) ?
                           clientDetails.FileDeliveryMethod.ToLower() == "email" ?
                               FolderCleanerClass.GetFolders(fullFilePath, null, null, clientOrgNo) ? 2 : 0 :
                               await FolderCleanerClass.GetFtpPathAsync(ftpConnect, ftpFileList, localFileList) ? 2 : 0 : 0;
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