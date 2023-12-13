using GraphHelper;
using WriteLog;
using ConnectFtp;
using ConnectFtps;
using FluentFTP;
using UserConfigReader;
using FileFunctions;
using FolderFunctions;
using ProcessStatusMessageSetter;
using AppConfigReader;

namespace FtpFunctions
{
    internal class FtpFunctionsClass
    {
        /// <summary>
        /// This function is created to get the ftp files and this starts the ftp file download process.
        /// </summary>
        /// <param name="Customerid"></param>
        /// <returns></returns>
        public static async Task<int> GetFtpFiles(int Customerid)
        {
            // Loads all the details from the customer details Json file.
            UserConfigReaderClass.CustomerDetailsObject JsonData = UserConfigReaderClass.ReadUserDotConfig<UserConfigReaderClass.CustomerDetailsObject>();

            // Just select the data corrosponding to the customer ID.
            UserConfigReaderClass.Customerdetail clientDetails = JsonData.CustomerDetails!.FirstOrDefault(cid => cid.Id == Customerid)!;

            // Starts the file download process if the client details are not empty.
            if (clientDetails != null)
            {
               return await InitiateFtpDownload(clientDetails!);
            }
            return 0;
        }
        
        /// <summary>
        /// Start the FTP file download process.
        /// </summary>
        /// <param name="FtpClientDetails">All the client details from the config file.</param>
        /// <returns>Return true or false.</returns>
        public static async Task<int> InitiateFtpDownload(UserConfigReaderClass.Customerdetail FtpClientDetails)
        {
            int downloadResult = 0; // Return value

            // All the details from the clinet configuration.
            UserConfigReaderClass.Ftpdetails clientFtpDetails = FtpClientDetails.FtpDetails;           

            // Ftp details retrived from the Json file.
            int clientID = FtpClientDetails.Id;
            string ftpType = clientFtpDetails.FtpType.ToLower();
            string ftpHostName = clientFtpDetails.FtpHostName;
            string ftpHosIp = clientFtpDetails.FtpHostIp;
            string ftpUser = clientFtpDetails.FtpUser;
            string ftpPassword = clientFtpDetails.FtpPassword;
            string ftpFolderPath = clientFtpDetails.FtpMainFolder;
            string downloadFolder = Path.Combine(FolderFunctionsClass.CheckFolders("ftp")
                                                , ftpFolderPath.Trim('/').Replace('/', '\\'));

            // Get's the FTP conntection token
            AsyncFtpClient ftpConnectToken = await ConnectFtpClass.ConnectFtp(ftpHostName, ftpHosIp, ftpUser, ftpPassword);

            // If the user FTP config type is FTPS.
            if (ftpType == "FTPS")
            {
                ftpConnectToken = await ConnectFtpsClass.ConnectFtps(ftpHostName, ftpHosIp, ftpUser, ftpPassword);
            }

            // If the connection token equals null then returns early terminating the execution.
            if (ftpConnectToken == null)
            {
                WriteLogClass.WriteToLog(1, "Connection to FTP server failed ....", 3);
                return downloadResult;
            }

            // Starts the file download process
            using (ftpConnectToken)
            try
            {
                WriteLogClass.WriteToLog(1, $"Starting file download from {ftpFolderPath} ....", 3);

                downloadResult = await DownloadFtpFiles(ftpConnectToken, ftpFolderPath, downloadFolder, clientID);

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

        /// <summary>
        /// Download the files from the FTP server in batches cofigured in the appsettings.json file.
        /// </summary>
        /// <param name="ftpConnect">FTP connection token.</param>
        /// <param name="ftpPath">FTP folder path</param>
        /// <param name="downloadFolderPath">Local download folder path.</param>
        /// <param name="clientID">ID of the client take from the config file.</param>
        /// <returns></returns>
        private static async Task<int> DownloadFtpFiles(AsyncFtpClient ftpConnect, string ftpPath, string downloadFolderPath, int clientID)
        {
            // Return value.
            int result = -1;

            try
            {
                // Reads the appsettings.json file.
                AppConfigReaderClass.AppSettingsRoot jsonData = AppConfigReaderClass.ReadAppDotConfig();
                // Gets the FTP file list.
                IEnumerable<FtpListItem> ftpFileNameList = await ftpConnect.GetListing(ftpPath);
                // Gets the files to download.
                IEnumerable<string> filesToDownload = ftpFileNameList
                                               .Where(f => f.Type == FtpObjectType.File)
                                               .Select(f => f.FullName).ToArray();

                // If there are no files to download then returns early terminating the execution.
                if (!filesToDownload.Any())
                {
                    return 4;
                }
                // Download folder path.
                string downloaFolder = Path.Combine(downloadFolderPath, GraphHelperClass.FolderNameRnd(10));

                // Starts the file download process.
                List<FtpResult> downloadResult = await ftpConnect.DownloadFiles(downloaFolder, filesToDownload, FtpLocalExists.Resume, FtpVerify.Retry);

                // Starts the file download process.
                int batchSize = jsonData.ProgramSettings.MaxBatchSize;
                int totalFtpFiles = filesToDownload.Count();
                int batchCurrentIndex = 0;

                while (batchCurrentIndex < totalFtpFiles) // Loop until all the files are downloaded.
                {
                    // Gets the current batch of files.
                    IEnumerable<FtpResult> currentBatch = downloadResult.Skip(batchCurrentIndex).Take(batchSize);
                    
                    foreach (FtpResult ftpFile in currentBatch)
                    {
                        result = await FilesUploadFuntcion(ftpConnect, currentBatch.Select(r => r.RemotePath.ToString()).ToArray(), downloaFolder, ftpFile.Name, clientID);
                    }                    

                    if (result == 3 || result == 4)
                    {
                        return result;
                    }

                    batchCurrentIndex += batchSize;
                }
                return result;
            }
            catch (Exception ex)
            {
                WriteLogClass.WriteToLog(0, $"Exception at file download: {ex.Message}", 0);
                return result;
            }
        }

        /// <summary>
        /// File download function. This will be parsing the files to the FTP client and downloading them the the local folder.
        /// </summary>
        /// <param name="ftpConnect">FTP connection token.</param>
        /// <param name="currentBatch">Files downloaded from the server.</param>
        /// <param name="ftpHoldFolder">Local download folder.</param>
        /// <param name="clientId">Client ID.</param>
        /// <param name="downloadResult"></param>
        /// <returns></returns>
        private static async Task<int> FilesUploadFuntcion(AsyncFtpClient ftpConnect,
                                                          string[] currentBatch,
                                                          string ftpHoldFolder,
                                                          string fileName,
                                                          int clientId)
        {
            string[] matchingFileName = currentBatch.Where(f => Path.GetFileNameWithoutExtension(f)
                                                           .Equals(Path.GetFileNameWithoutExtension(fileName), StringComparison.OrdinalIgnoreCase))
                                                           .ToArray();
            /*IEnumerable<string> unmatchedFileList = FolderCleanerClass.CheckMissedFiles(ftpHoldFolder, currentBatch);
            if (unmatchedFileList.Any())
            {
                WriteLogClass.WriteToLog(1, $"Ftp file count: {currentBatch.Count()}, Local file count: {unmatchedFileList.Count()} files doesn't match", 3);
                return 3;
            }*/
            string[] localFiles = Directory.GetFiles(ftpHoldFolder, "*.*", SearchOption.TopDirectoryOnly);
            return await FileFunctionsClass.SendToWebService(ftpConnect, ftpHoldFolder, fileName, clientId, matchingFileName, localFiles, null!);
        }
    }
}
