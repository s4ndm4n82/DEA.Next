using DEA.Next.FileOperations.TpsFileUploadFunctions;
using DEA.Next.FileOperations.TpsJsonStringClasses;
using FluentFTP;
using Newtonsoft.Json;
using Renci.SshNet;
using UserConfigRetriverClass;
using UserConfigSetterClass;
using WriteLog;

namespace DEA.Next.FileOperations.TpsJsonStringCreatorFunctions
{
    /// <summary>
    /// Creats the Json request for data file.
    /// </summary>
    internal class MakeJsonRequestDataFileFunction
    {
        public static async Task<int> MakeJsonRequestDataFileAsync(AsyncFtpClient ftpConnect,
                                                                   SftpClient sftpConnect,
                                                                   int customerId,
                                                                   string localFilePath,
                                                                   string[] fileToSend,
                                                                   string[] ftpFileList,
                                                                   string[] localFileList)
        {
            try
            {
                UserConfigSetter.Customerdetail customerDetails = await UserConfigRetriver.RetriveUserConfigById(customerId);

                // Get the filename.
                string fileName = Path.GetFileName(fileToSend.FirstOrDefault());

                // Convert the file to base64 string.
                string fileData = Convert.ToBase64String(File.ReadAllBytes(fileToSend.FirstOrDefault()));

                TpsJsonDataFileUploadString.TpsJsonDataFileUploadObject TpsJsonRequest = new()
                {
                    Token = customerDetails.Token,
                    Username = customerDetails.UserName,
                    ID = customerDetails.DocumentId,
                    FileData = fileData,
                };

                // Creat the Json request.
                string jsonRequest = JsonConvert.SerializeObject(TpsJsonRequest, Formatting.Indented);

                return await SendFilesToRestApiDataFile.SendFilesToRestDataFileAsync(ftpConnect,
                                                                                     sftpConnect,
                                                                                     customerId,
                                                                                     jsonRequest,
                                                                                     localFilePath,
                                                                                     fileName,
                                                                                     ftpFileList,
                                                                                     localFileList);
            }
            catch (Exception ex)
            {
                WriteLogClass.WriteToLog(0, $"Exception at MakeJsonRequestDataFileAsync: {ex.Message}", 0);
                return -1;
            }
        }
    }
}