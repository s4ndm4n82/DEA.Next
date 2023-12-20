using WriteLog;
using ConnectFtp;
using ConnectFtps;
using FluentFTP;
using UserConfigSetterClass;
using FolderFunctions;
using ProcessStatusMessageSetter;
using DownloadFtpFilesClass;
using FtpLoopDownloadClass;

namespace FtpFunctions
{
    internal class FtpFunctionsClass
    {
        /// <summary>
        /// This function is created to get the ftp files and this starts the ftp file download process.
        /// </summary>
        /// <param name="Customerid"></param>
        /// <returns></returns>
        public static async Task<int> GetFtpFiles(int customerId)
        {
            // Starts the file download process if the client details are not empty.
            if (customerId != null)
            {
                return await InitiateFtpDownload(customerId);
            }
            return 0;
        }

        /// <summary>
        /// Start the FTP file download process.
        /// </summary>
        /// <param name="FtpClientDetails">All the client details from the config file.</param>
        /// <returns>Return true or false.</returns>
        public static async Task<int> InitiateFtpDownload(int clientId)
        {
            int downloadResult = 0; // Return value
            AsyncFtpClient ftpConnectToken = null;
            UserConfigSetterClass.UserConfigSetter.Ftpdetails ftpDetails = await GetFtpDetails(clientId);

            string downloadFolder = Path.Combine(FolderFunctionsClass.CheckFolders("ftp")
                                                , ftpDetails.FtpMainFolder.Trim('/').Replace('/', '\\'));

            // If the user FTP config type is FTP.
            if (ftpDetails.FtpType == FtpNames.Ftp)
            {
                ftpConnectToken = await ConnectFtpClass.ConnectFtp(ftpDetails.FtpHostName,
                                                                                  ftpDetails.FtpHostIp,
                                                                                  ftpDetails.FtpUser,
                                                                                  ftpDetails.FtpPassword);
            }

            // If the user FTP config type is FTPS.
            if (ftpDetails.FtpType == "FTPS")
            {
                ftpConnectToken = await ConnectFtpsClass.ConnectFtps(ftpDetails.FtpHostName,
                                                                     ftpDetails.FtpHostIp,
                                                                     ftpDetails.FtpUser,
                                                                     ftpDetails.FtpPassword);
            }


            // If the connection token equals null then returns early terminating the execution.
            if (ftpConnectToken == null)
            {
                WriteLogClass.WriteToLog(1, "Connection to FTP server failed ....", 3);
                return 0;
            }

            // Starts the file download process
            using (ftpConnectToken)
                try
                {
                    WriteLogClass.WriteToLog(1, $"Starting file download from {ftpDetails.FtpMainFolder} ....", 3);

                    if (ftpDetails.FtpFolderLoop == 1)
                    {
                        downloadResult = await FtpLoopDownload.StartFtpLoopDownload(ftpConnectToken,
                                                                              ftpDetails.FtpMainFolder,
                                                                              downloadFolder,
                                                                              clientId);
                    }

                    if (ftpDetails.FtpFolderLoop == 0)
                    {
                        downloadResult = await DownloadFtpFiles.DownloadFtpFilesFunction(ftpConnectToken,
                                                                 ftpDetails.FtpMainFolder,
                                                                 downloadFolder,
                                                                 clientId);
                    }                    

                    WriteLogClass.WriteToLog(ProcessStatusMessageSetterClass.SetMessageTypeOther(downloadResult),
                                             $"{ProcessStatusMessageSetterClass.SetProcessStatusOther(downloadResult, "ftp")}\n", 3);
                    return downloadResult;
                }
                catch (Exception ex)
                {
                    WriteLogClass.WriteToLog(0, $"Exception at FTP file download: {ex.Message}", 0);
                    return downloadResult;
                }
        }

        private static async Task<UserConfigSetterClass.UserConfigSetter.Ftpdetails> GetFtpDetails(int clientId)
        {
            UserConfigSetterClass.UserConfigSetter.CustomerDetailsObject jsonDate = await UserConfigSetterClass.UserConfigSetter.ReadUserDotConfigAsync<UserConfigSetterClass.UserConfigSetter.CustomerDetailsObject>();
            UserConfigSetterClass.UserConfigSetter.Customerdetail customerDetails = jsonDate.CustomerDetails.FirstOrDefault(cid => cid.Id == clientId);
            UserConfigSetterClass.UserConfigSetter.Ftpdetails ftpDetails = customerDetails.FtpDetails;

            return ftpDetails;
        }

        private static class FtpNames
        {
            public const string Ftps = "FTPS";
            public const string Ftp = "FTP";
            public const string Sftp = "SFTP";
        }
    }
}
