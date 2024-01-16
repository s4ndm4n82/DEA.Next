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

                string clientOrg = SendToWebServiceProjectFunctions.SetCustomerOrg(clientDetails.FtpDetails.FtpFolderLoop,
                                                                                   clientDetails.SendEmail,
                                                                                   clientDetails.ClientOrgNo,
                                                                                   ftpFolderName,
                                                                                   recipientEmail);

                string[] downloadedFiles = SendToWebServiceProjectFunctions.MakeDownloadedFileList(clientDetails,
                                                                                           filePath,
                                                                                           ftpFileList);

                if (!downloadedFiles.Any())
                {
                    WriteLogClass.WriteToLog(1, "No matching files in the download list ....", 1);
                    return -1;
                }

                return await MakeJsonRequestProjectsFunction.MakeJsonRequestProjects(ftpConnect,
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
            }
            catch (Exception ex)
            {
                WriteLogClass.WriteToLog(1, $"Exception at SendToWebService: {ex.Message}", 1);
                return -1;
            }
        }
    }
}