using System.Text.Json;
using UserConfigReader;
using TpsJsonString;
using WriteLog;
using System.Text.Json.Serialization;

namespace FileFunctions
{
    internal class FileFunctionsClass
    {
        public static async Task<bool> SendToWebService(string filePath, int customerId)
        {
            UserConfigReaderClass.CustomerDetailsObject jsonData = await UserConfigReaderClass.ReadAppDotConfig<UserConfigReaderClass.CustomerDetailsObject>();
            UserConfigReaderClass.Customerdetail clientDetails = jsonData.CustomerDetails!.FirstOrDefault(cid => cid.Id == customerId)!;

            string[] downloadedFiles = Directory.GetFiles(filePath);

            MakeJsonRequest(clientDetails.Token!, clientDetails.UserName!, clientDetails.TemplateKey!, clientDetails.Queue!, clientDetails.ProjetID!, downloadedFiles);

            return true;
        }

        private static bool MakeJsonRequest(string customerToken, string customerUserName, string customerTemplateKey, string customerQueue, string customerProjectId, string[] filesToSend)
        {
            /*var fileList = new List<TpsJasonStringClass.FileList>();
            foreach (var file in filesToSend)
            {
                fileList.Add(new TpsJasonStringClass.FileList() { Name = Path.GetFileName(file), Data = Convert.ToBase64String(File.ReadAllBytes(file)) });
            }*/

            var fileList = filesToSend.Select(x => new TpsJasonStringClass.FileList() { Name = Path.GetFileName(x), Data = Convert.ToBase64String(File.ReadAllBytes(x)) });

            TpsJasonStringClass.TpsJsonObject TpmJsonRequest = new TpsJasonStringClass.TpsJsonObject()
            {
                Token = $"{customerToken}",
                Username = $"{customerUserName}",
                TemplateKey = $"{customerTemplateKey}",
                Queue = $"{customerQueue}",
                ProjectID = $"{customerProjectId}",
                Files = (List<TpsJasonStringClass.FileList>)fileList
            };


            var result = 
            return true;
        }
    }
}
