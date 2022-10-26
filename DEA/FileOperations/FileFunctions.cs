using Newtonsoft.Json;
using UserConfigReader;
using TpsJsonString;
using WriteLog;

namespace FileFunctions
{
    internal class FileFunctionsClass
    {
        public static Task<bool> SendToWebService(string filePath, int customerId)
        {   
            UserConfigReaderClass.CustomerDetailsObject jsonData = UserConfigReaderClass.ReadAppDotConfig<UserConfigReaderClass.CustomerDetailsObject>();
            UserConfigReaderClass.Customerdetail clientDetails = jsonData.CustomerDetails!.FirstOrDefault(cid => cid.id == customerId)!;

            string[] downloadedFiles = Directory.GetFiles(filePath);

            var flag = MakeJsonRequest(clientDetails.Token!, clientDetails.UserName!, clientDetails.TemplateKey!, clientDetails.Queue!, clientDetails.ProjetID!, downloadedFiles);

            if (flag)
            {
                return Task.FromResult(true);
            }
            else
            {
                return Task.FromResult(false);
            }
        }

        private static bool MakeJsonRequest(string customerToken, string customerUserName, string customerTemplateKey, string customerQueue, string customerProjectId, string[] filesToSend)
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

            try
            {
                var result = JsonConvert.SerializeObject(TpsJsonRequest);
                return true;
            }
            catch (Exception ex)
            {
                WriteLogClass.WriteToLog(3, $"Exception at Json serialization: {ex.Message}", string.Empty);
                return false;
            }
        }
    }
}