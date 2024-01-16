using DEA.Next.FileOperations.TpsFileUploadFunctions;
using DEA.Next.FileOperations.TpsJsonStringClasses;
using FluentFTP;
using Newtonsoft.Json;
using UserConfigRetriverClass;
using UserConfigSetterClass;

namespace DEA.Next.FileOperations.TpsJsonStringCreatorFunctions
{
    internal class MakeJsonRequestDataFileFunction
    {
        public static async Task<int> MakeJsonRequestDataFileAsync(AsyncFtpClient ftpConnect,
                                                                   int customerId,
                                                                   string[] fileToSend,
                                                                   string[] ftpFileList,
                                                                   string[] localFileList)
        {
            UserConfigSetter.Customerdetail customerDetails = await UserConfigRetriver.RetriveUserConfigById(customerId);
            string fileName = Path.GetFileName(fileToSend.FirstOrDefault());
            string fileData = Convert.ToBase64String(File.ReadAllBytes(fileToSend.FirstOrDefault()));

            var fileName2 = fileName.ToString();
            TpsJsonDataFileUploadString.TpsJsonDataFileUploadObject TpsJsonRequest = new()
            {
                Token = customerDetails.Token,
                Username = customerDetails.UserName,
                ID = customerDetails.DocumentId,
                FileData = fileData,
            };

            string jsonRequest = JsonConvert.SerializeObject(TpsJsonRequest, Formatting.Indented);

            return 
        }
    }
}