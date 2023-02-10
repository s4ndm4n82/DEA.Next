using System.Net;
using Newtonsoft.Json;
using UserConfigReader;
using RestSharp;
using TpsJsonString;
using WriteLog;
using FolderCleaner;
using System.Diagnostics.CodeAnalysis;

namespace FileFunctions
{
    internal class FileFunctionsClass
    {
        public static async Task<bool> SendToWebService(string filePath, int customerId)
        {   
            UserConfigReaderClass.CustomerDetailsObject jsonData = UserConfigReaderClass.ReadAppDotConfig<UserConfigReaderClass.CustomerDetailsObject>();
            UserConfigReaderClass.Customerdetail clientDetails = jsonData.CustomerDetails!.FirstOrDefault(cid => cid.id == customerId)!;

            string[] downloadedFiles = Directory.GetFiles(filePath);

            if (await MakeJsonRequest(clientDetails.Token!, clientDetails.UserName!, clientDetails.TemplateKey!, clientDetails.Queue!, clientDetails.ProjetID!, downloadedFiles))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private static async Task<bool> MakeJsonRequest(string customerToken, string customerUserName, string customerTemplateKey, string customerQueue, string customerProjectId, string[] filesToSend)
        {
            var fileList = new List<TpsJasonStringClass.FileList>();
            foreach (var file in filesToSend)
            {   
                fileList.Add(new TpsJasonStringClass.FileList() { Name = Path.GetFileName(file), Data = Convert.ToBase64String(File.ReadAllBytes(file)) });
            }

            TpsJasonStringClass.TpsJsonObject TpsJsonRequest = new()
            {
                Token = $"{customerToken}",
                Username = $"{customerUserName}",
                TemplateKey = $"{customerTemplateKey}",
                Queue = $"{customerQueue}",
                ProjectID = $"{customerProjectId}",
                Files = fileList
            };

            var result = JsonConvert.SerializeObject(TpsJsonRequest);
            
            try
            {
                if (await SendFilesToRest(result, filesToSend[0]))
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
                WriteLogClass.WriteToLog(3, $"Exception at Json serialization: {ex.Message}", string.Empty);
                return false;
            }
            
        }

        private static async Task<bool> SendFilesToRest(string jsonResult, [NotNull]string fullFilePath)
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
                    WriteLogClass.WriteToLog(3, $"Server status code: {serverResponse.StatusCode}", string.Empty);

                    // Deletes the file from local hold folder when sending is successful.
                    FolderCleanerClass.GetFolders(fullFilePath);
                    return true;
                }
                else
                {
                    WriteLogClass.WriteToLog(3, $"Server status code: {serverResponse.StatusCode}", string.Empty);
                    return false;
                }
            }
            catch (Exception ex)
            {
                WriteLogClass.WriteToLog(3, $"Exception at rest sharp request: {ex.Message}", string.Empty);
            }

            return false;
        }
    }
}