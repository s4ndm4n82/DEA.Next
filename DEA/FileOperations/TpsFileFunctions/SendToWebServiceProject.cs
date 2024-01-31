using WriteLog;
using FluentFTP;
using UserConfigSetterClass;
using UserConfigRetriverClass;
using DEA.Next.FileOperations.TpsJsonStringCreatorFunctions;
using DEA.Next.FileOperations.TpsFileFunctions;

namespace FileFunctions
{
    internal class SendToWebServiceProject
    {
        public static async Task<int> SendToWebServiceProjectAsync(AsyncFtpClient ftpConnect,
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

                // Get the correct org number depending on what type of download method is used.
                string clientOrg = SendToWebServiceHelpertFunctions.SetCustomerOrg(clientDetails.FtpDetails.FtpFolderLoop,
                                                                                   clientDetails.SendEmail,
                                                                                   clientDetails.ClientOrgNo,
                                                                                   ftpFolderName,
                                                                                   recipientEmail);
                // Creats the file list of the downloaded files.
                string[] downloadedFiles = SendToWebServiceHelpertFunctions.MakeDownloadedFileList(clientDetails,
                                                                                                   filePath,
                                                                                                   clientOrg,
                                                                                                   ftpFileList);

                if (!downloadedFiles.Any())
                {
                    WriteLogClass.WriteToLog(1, "No matching files in the download list ....", 1);
                    return -1;
                }

                return await MakeJsonRequestProjectsFunction.MakeJsonRequestProjects(ftpConnect,
                                                                                     customerId,
                                                                                     clientOrg,
                                                                                     downloadedFiles,
                                                                                     ftpFileList,
                                                                                     localFileList);
            }
            catch (Exception ex)
            {
                WriteLogClass.WriteToLog(1, $"Exception at SendToWebService: {ex.Message}", 1);
                return -1;
            }
        }
    }
}