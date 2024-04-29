using WriteLog;
using ConnectFtp;
using ConnectFtps;
using FluentFTP;
using UserConfigSetterClass;
using FolderFunctions;
using ProcessStatusMessageSetter;
using DownloadFtpFilesClass;
using FtpLoopDownloadClass;
using DEA.Next.HelperClasses.OtherFunctions;
using UserConfigRetriverClass;

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
            if (customerId != default)
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
        private static async Task<int> InitiateFtpDownload(int clientId)
        {
            int downloadResult = 0; // Return value
            AsyncFtpClient? ftpConnectToken = null;
            UserConfigSetter.Ftpdetails ftpDetails = await UserConfigRetriver.RetriveFtpConfigById(clientId);

            string downloadFolder = Path.Combine(FolderFunctionsClass.CheckFolders(MagicWords.ftp)
                                                , ftpDetails.FtpMainFolder.Trim('/').Replace('/', '\\'));

            // If the user FTP config type is FTP.
            if (string.Equals(ftpDetails.FtpType, MagicWords.ftp, StringComparison.OrdinalIgnoreCase))
            {
                ftpConnectToken = await ConnectFtpClass.ConnectFtp(ftpDetails.FtpProfile,
                                                                   ftpDetails.FtpHostName,                                                                   
                                                                   ftpDetails.FtpUser,
                                                                   ftpDetails.FtpPassword,
                                                                   ftpDetails.FtpPort);
            }

            // If the user FTP config type is FTPS.
            if (string.Equals(ftpDetails.FtpType, MagicWords.ftps, StringComparison.OrdinalIgnoreCase))
            {
                ftpConnectToken = await ConnectFtpsClass.ConnectFtps(ftpDetails.FtpProfile,
                                                                     ftpDetails.FtpHostName,
                                                                     ftpDetails.FtpUser,
                                                                     ftpDetails.FtpPassword,
                                                                     ftpDetails.FtpPort);
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
                        downloadResult = await FtpFilesDownload.DownloadFtpFilesFunction(ftpConnectToken,
                                                                                         ftpDetails.FtpMainFolder,
                                                                                         downloadFolder,
                                                                                         string.Empty,
                                                                                         clientId);
                    }                    

                    WriteLogClass.WriteToLog(ProcessStatusMessageSetterClass.SetMessageTypeOther(downloadResult),
                                             $"{ProcessStatusMessageSetterClass.SetProcessStatusOther(downloadResult, MagicWords.ftp)}\n", 3);
                    return downloadResult;
                }
                catch (Exception ex)
                {
                    WriteLogClass.WriteToLog(0, $"Exception at FTP file download: {ex.Message}", 0);
                    return downloadResult;
                }
        }

        public static async Task<bool> MoveFtpFiles(AsyncFtpClient ftpConnect,
                                                    int clientId,
                                                    IEnumerable<string> ftpFilesList)
        {
            UserConfigSetter.Ftpdetails ftpDetails = await UserConfigRetriver.RetriveFtpConfigById(clientId);

            try
            {
                if (!ftpFilesList.Any())
                {
                    WriteLogClass.WriteToLog(0, $"Ftp files list is empty ....", 3);
                    return false;
                }

                if (ftpConnect == null)
                {
                    WriteLogClass.WriteToLog(0, $"Ftp connection is null ....", 3);
                    return false;
                }

                int loopCount = 0;

                foreach (string ftpFile in ftpFilesList)
                {
                    if (!await ftpConnect.FileExists(ftpFile))
                    {
                        WriteLogClass.WriteToLog(0, $"Source file does not exist: {ftpFile} ....", 3);
                        continue;
                    }

                    string ftpFileName = Path.GetFileName(ftpFile);
                    string ftpDestinationPath = string.Concat(ftpDetails.FtpSubFolder, "/", ftpFileName);

                    WriteLogClass.WriteToLog(1, $"Moving file: {ftpFileName} to {ftpDestinationPath} ....", 3);
                    await ftpConnect.MoveFile(ftpFile, ftpDestinationPath);
                    WriteLogClass.WriteToLog(1, $"File moved: {ftpFileName} to {ftpDestinationPath} ....", 3);
                    loopCount++;
                }

                if (loopCount == ftpFilesList.Count())
                {
                    WriteLogClass.WriteToLog(1, $"Files moved to {ftpDetails.FtpSubFolder} successfully ....", 3);
                    return true;
                }

                WriteLogClass.WriteToLog(0, $"Moving files to {ftpDetails.FtpSubFolder} unsuccessfull ....", 3);
                return false;
            }
            catch (Exception ex)
            {
                WriteLogClass.WriteToLog(0, $"Exception at FTP file move: {ex.Message}", 0);
                return false;
            }
        }
    }
}
