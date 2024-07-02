using ConnectFtp;
using ConnectFtps;
using ConnectSftp;
using DEA.Next.FileOperations.TpsFileFunctions;
using DEA.Next.FileOperations.TpsJsonStringCreatorFunctions;
using DEA.Next.HelperClasses.OtherFunctions;
using FluentFTP;
using Renci.SshNet;
using UserConfigRetriverClass;
using UserConfigSetterClass;
using WriteLog;
using static UserConfigSetterClass.UserConfigSetter;

namespace FileFunctions
{
    internal class SendToWebServiceProject
    {
        public static async Task<int> SendToWebServiceProjectAsync(AsyncFtpClient ftpConnect,
                                                                   SftpClient sftpConnect,
                                                                   string filePath,
                                                                   int customerId,
                                                                   string[] ftpFileList,
                                                                   string[] localFileList,
                                                                   string ftpFolderName,
                                                                   string recipientEmail)
        {
            Ftpdetails ftpDetails = await UserConfigRetriver.RetriveFtpConfigById(customerId);

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
                // Creates the file list of the downloaded files.
                string[] downloadedFiles = SendToWebServiceHelpertFunctions.MakeDownloadedFileList(clientDetails,
                                                                                                   filePath,
                                                                                                   clientOrg,
                                                                                                   ftpFileList);
                if (!downloadedFiles.Any())
                {
                    WriteLogClass.WriteToLog(1, "No matching files in the download list ....", 1);
                    return -1;
                }

                if (!ftpConnect.IsConnected &&
                    string.Equals(ftpDetails.FtpType, MagicWords.ftp, StringComparison.OrdinalIgnoreCase))
                {
                    ftpConnect = await ConnectFtpClass.ConnectFtp(ftpDetails.FtpProfile,
                                                                  ftpDetails.FtpHostName,
                                                                  ftpDetails.FtpUser,
                                                                  ftpDetails.FtpPassword,
                                                                  ftpDetails.FtpPort);
                }

                if (!ftpConnect.IsConnected &&
                    string.Equals(ftpDetails.FtpType, MagicWords.ftp, StringComparison.OrdinalIgnoreCase))
                {
                    ftpConnect = await ConnectFtpsClass.ConnectFtps(ftpDetails.FtpProfile,
                                                                     ftpDetails.FtpHostName,
                                                                     ftpDetails.FtpUser,
                                                                     ftpDetails.FtpPassword,
                                                                     ftpDetails.FtpPort);
                }

                if (!sftpConnect.IsConnected &&
                    string.Equals(ftpDetails.FtpType, MagicWords.sftp, StringComparison.OrdinalIgnoreCase))
                {
                    sftpConnect = await ConnectSftpClass.ConnectSftp(ftpDetails.FtpHostName,
                                                                      ftpDetails.FtpUser,
                                                                      ftpDetails.FtpPassword,
                                                                      ftpDetails.FtpPort);
                }

                return await MakeJsonRequestProjectsFunction.MakeJsonRequestProjects(ftpConnect,
                                                                                     sftpConnect,
                                                                                     customerId,
                                                                                     clientOrg,
                                                                                     downloadedFiles,
                                                                                     ftpFileList,
                                                                                     localFileList);
            }
            catch (Exception ex)
            {
                WriteLogClass.WriteToLog(0, $"Exception at SendToWebService: {ex.Message}", 0);
                return -1;
            }
        }
    }
}