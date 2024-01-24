using DEA.Next.FileOperations.TpsFileUploadFunctions;
using FluentFTP;
using Newtonsoft.Json;
using TpsJsonProjectUploadString;
using WriteLog;

namespace DEA.Next.FileOperations.TpsJsonStringCreatorFunctions
{
    internal class MakeJsonRequestProjectsFunction
    {
        public static async Task<int> MakeJsonRequestProjects(AsyncFtpClient ftpConnect,
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
            try
            {
                // Creating the file list to be added to the Json request.
                List<TpsJsonProjectUploadStringClass.FileList> jsonFileList = new();
                foreach (var file in filesToSend)
                {
                    jsonFileList.Add(new TpsJsonProjectUploadStringClass.FileList() { Name = Path.GetFileName(file), Data = Convert.ToBase64String(File.ReadAllBytes(file)) });
                }

                // Creating the field list to be added to the Json request.
                List<TpsJsonProjectUploadStringClass.FieldList> idField = new()
                {
                    new TpsJsonProjectUploadStringClass.FieldList() { Name = clientIdField, Value = clientOrgNo }
                };

                TpsJsonProjectUploadStringClass.TpsJsonProjectUploadObject TpsJsonRequest = new()
                {
                    Token = customerToken,
                    Username = customerUserName,
                    TemplateKey = customerTemplateKey,
                    Queue = customerQueue,
                    ProjectID = customerProjectId,
                    Fields = idField,
                    Files = jsonFileList
                };

                string jsonResult = JsonConvert.SerializeObject(TpsJsonRequest, Formatting.Indented);

                return await SendFilesToRestApiProject.SendFilesToRestProjectAsync(ftpConnect,
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
            }
            catch (Exception ex)
            {
                WriteLogClass.WriteToLog(0, $"Exception at Json serialization: {ex.Message}", 0);
                return 0;
            }
        }
    }
}
