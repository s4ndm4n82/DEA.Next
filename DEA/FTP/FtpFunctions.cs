using GraphHelper;
using WriteLog;
using ConnectFtp;
using ConnectFtps;
using FluentFTP;
using UserConfigReader;
using FileFunctions;
using FolderFunctions;
using ProcessStatusMessageSetter;
using FolderCleaner;

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

        public static async Task<int> InitiateFtpDownload(UserConfigReaderClass.Customerdetail FtpClientDetails)
        {
            int downloadResult = 0;

            // Client details retrived from the Json file.
            int clientID = FtpClientDetails.Id;
            string ftpType = FtpClientDetails.FtpDetails!.FtpType!.ToUpper();
            string ftpHostName = FtpClientDetails.FtpDetails!.FtpHostName!;
            string ftpHosIp = FtpClientDetails.FtpDetails.FtpHostIp!;
            string ftpUser = FtpClientDetails.FtpDetails.FtpUser!;
            string ftpPassword = FtpClientDetails.FtpDetails.FtpPassword!;
            string ftpMainFolder = FtpClientDetails.FtpDetails.FtpMainFolder!;
            string ftpSubFolder1 = FtpClientDetails.FtpDetails.FtpSubFolder1!.Replace(" ", "");
            string ftpSubFolder2 = FtpClientDetails.FtpDetails.FtpSubFolder2!.Replace(" ", "");
            string ftpPath;

            // Creating the FTP folder path. Can go up to 3 levels of folders.
            if (!string.IsNullOrEmpty(ftpSubFolder2))
            {
                ftpPath = $@"/{ftpMainFolder}/{ftpSubFolder1}/{ftpSubFolder2}";
            }
            else
            {
                ftpPath = $@"/{ftpMainFolder}/{ftpSubFolder1}";
            }            

            string LocalFtpFolder = FolderFunctionsClass.CheckFolders("ftp");
            string ftpHoldFolder;

            if (!string.IsNullOrEmpty(ftpSubFolder2))
            {
                ftpHoldFolder = Path.Combine(LocalFtpFolder, ftpMainFolder!, ftpSubFolder1!, ftpSubFolder2, GraphHelperClass.FolderNameRnd(10));
            }
            else
            {
                ftpHoldFolder = Path.Combine(LocalFtpFolder, ftpMainFolder!, ftpSubFolder1!, GraphHelperClass.FolderNameRnd(10));
            }
            
            AsyncFtpClient ftp = null!;

            if (ftpType == "FTP")
            {
                ftp = await ConnectFtpClass.ConnectFtp(ftpHostName!, ftpHosIp!, ftpUser!, ftpPassword!);
            }
            else if (ftpType == "FTPS")
            {
                ftp = await ConnectFtpsClass.ConnectFtps(ftpHostName!, ftpHosIp!, ftpUser!, ftpPassword!);
            }

            if (ftp == null)
            {
                WriteLogClass.WriteToLog(1, "Connection to FTP server failed ....", 3);
                return downloadResult;
            }

            using AsyncFtpClient ftpConnect = ftp!;

            if (await ftpConnect!.DirectoryExists(ftpPath))
            {
                WriteLogClass.WriteToLog(1, $"Starting file download from {ftpPath} ....", 3);

                downloadResult = await DownloadFtpFiles(ftpConnect, ftpPath, ftpHoldFolder, clientID);
                
                WriteLogClass.WriteToLog(ProcessStatusMessageSetterClass.SetMessageTypeOther(downloadResult), $"{ProcessStatusMessageSetterClass.SetProcessStatusOther(downloadResult, "ftp")}\n", 3);

                await ftpConnect.Disconnect(); // Disconnects from the FTP server.  
            }
            return downloadResult;
        }

        private static async Task<int> DownloadFtpFiles(AsyncFtpClient ftpConnect, string ftpPath, string ftpHoldFolder, int clientID)
        {
            try
            {
                // TODO: Add max file limit. Here and to config file.
                IEnumerable<FtpListItem> ftpFileNameList = await ftpConnect.GetListing(ftpPath);
                IEnumerable<string> filesToDownload = ftpFileNameList
                                               .Where(f => f.Type == FtpObjectType.File)
                                               .Select(f => f.FullName).ToArray();


                return await FileDownloadFuntcion(ftpConnect, filesToDownload, ftpHoldFolder, clientID);
            }
            catch (Exception ex)
            {
                WriteLogClass.WriteToLog(0, $"Exception at file download: {ex.Message}", 0);
                return -1;
            }
        }

        private static async Task

        private static async Task<int> FileDownloadFuntcion(AsyncFtpClient ftpConnect, IEnumerable<string> filesToDownload, string ftpHoldFolder, int clientId)
        {

            // Downloads files from the server and counts them.
            IEnumerable<FtpResult> downloadResult = await ftpConnect.DownloadFiles(ftpHoldFolder, filesToDownload, FtpLocalExists.Resume, FtpVerify.Retry);
            if (!downloadResult.Any())
            {
                return 4;
            }

            IEnumerable<string> unmatchedFileList = FolderCleanerClass.CheckMissedFiles(ftpHoldFolder, filesToDownload);
            if (unmatchedFileList.Any())
            {
                WriteLogClass.WriteToLog(1, $"Ftp file count: {filesToDownload.Count()}, Local file count: {unmatchedFileList.Count()} files doesn't match", 3);
                return 3;
            }

            string[] localFiles = Directory.GetFiles(ftpHoldFolder, "*.*", SearchOption.TopDirectoryOnly);
            return await FileFunctionsClass.SendToWebService(ftpConnect, ftpHoldFolder, clientId, filesToDownload, localFiles, null!);
        }
    }
}
