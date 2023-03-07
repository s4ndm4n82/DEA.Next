using GraphHelper;
using WriteLog;
using ConnectFtp;
using ConnectFtps;
using FluentFTP;
using UserConfigReader;
using FileFunctions;
using FolderFunctions;

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
            UserConfigReaderClass.CustomerDetailsObject JsonData = UserConfigReaderClass.ReadAppDotConfig<UserConfigReaderClass.CustomerDetailsObject>();

            // Just select the data corrosponding to the customer ID.
            UserConfigReaderClass.Customerdetail clientDetails = JsonData.CustomerDetails!.FirstOrDefault(cid => cid.id == Customerid)!;

            // Starts the file download process if the client details are not empty.
            if (clientDetails != null)
            {
               return await InitiateFtpDownload(clientDetails!);
            }
            return 0;
        }

        public static async Task<int> InitiateFtpDownload(UserConfigReaderClass.Customerdetail FtpClientDetails)
        {
            int returnFlag = 0;
            // Client details retrived from the Json file.
            int clientID = FtpClientDetails.id;
            string clientName = FtpClientDetails.ClientName!;
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
            string FtpHoldFolder;

            if (!string.IsNullOrEmpty(ftpSubFolder2))
            {
                FtpHoldFolder = Path.Combine(LocalFtpFolder, ftpMainFolder!, ftpSubFolder1!, ftpSubFolder2, GraphHelperClass.FolderNameRnd(10));
            }
            else
            {
                FtpHoldFolder = Path.Combine(LocalFtpFolder, ftpMainFolder!, ftpSubFolder1!, GraphHelperClass.FolderNameRnd(10));
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
                return returnFlag;
            }

            using AsyncFtpClient ftpConnect = ftp!;

            if (await ftpConnect!.DirectoryExists(ftpPath))
            {
                WriteLogClass.WriteToLog(1, $"Starting file download from {ftpPath} ....", 3);

                returnFlag = await DownloadFtpFiles(ftpConnect, ftpPath, FtpHoldFolder, clientID);

                switch (returnFlag)
                {
                    case 1:
                        WriteLogClass.WriteToLog(1, $"Files from client {clientName} downloaded and uploaded for processing ....\n", 3);
                        break;
                    case 2:
                        WriteLogClass.WriteToLog(1, "Uploading files failed. File moved to error ....\n", 3);
                        break;
                    case 3:
                        WriteLogClass.WriteToLog(1, "File download failed ....\n", 3);
                        break;
                    default:
                        WriteLogClass.WriteToLog(1, "Operation failed ....\n", 3);
                        break;
                }

                await ftpConnect.Disconnect(); // Disconnects from the FTP server.                

                return returnFlag;
            }
            return returnFlag;
        }

        public static async Task<int> DownloadFtpFiles(AsyncFtpClient ftpConnect, string ftpPath, string ftpHoldFolder, int clientID)
        {
            // To capture the loop success flags.
            int fileNameFlag = 0;

            // Initiate FTP connect and gets the file list from the FTP server.
            List<FtpListItem> ftpFilesOnly = new List<FtpListItem>();

            foreach (var fileItem in await ftpConnect.GetListing(ftpPath))
            {
                // Only select files only. Sub directories are skipped.
                if (fileItem.Type == FtpObjectType.File)
                {
                    ftpFilesOnly.Add(new FtpListItem() { Name = fileItem.Name, FullName = fileItem.FullName });
                }                
            }
            
            // Puts the full filename into a string list.
            IEnumerable<string> filesToDownload = ftpFilesOnly.Select(f => f.FullName.ToString());

            // Downloads files from the server and counts them.
            List<FtpResult> downloadedFileList = await ftpConnect.DownloadFiles(ftpHoldFolder, filesToDownload, FtpLocalExists.Resume, FtpVerify.Retry);
            
            // Array to store downloaded file list.
            string[] localFiles;

            if (downloadedFileList.Count > 0)
            {
                string lastFolderName = Path.GetFileName(ftpHoldFolder);                
                string parentName = Directory.GetParent(Path.GetFileName(ftpHoldFolder))!.Name;

                WriteLogClass.WriteToLog(1, $"Downloaded {downloadedFileList.Count} file/s from {ftpPath} to \\{parentName}\\{lastFolderName} folder ....", 3);

                localFiles = Directory.GetFiles(ftpHoldFolder, "*.*", SearchOption.TopDirectoryOnly);

                foreach (string ftpFile in filesToDownload)
                {
                    string ftpFileName = Path.GetFileNameWithoutExtension(ftpFile);
                    
                    foreach (string localFile in localFiles)
                    {
                        string localFileName = Path.GetFileNameWithoutExtension(localFile);

                        if (ftpFileName.Equals(localFileName, StringComparison.OrdinalIgnoreCase))
                        {
                            fileNameFlag = 1;
                        }
                    }
                }

                if (fileNameFlag == 1)
                {
                    WriteLogClass.WriteToLog(1, $"Ftp file count: {filesToDownload.Count()}, Local file count: {localFiles.Length}", 3);

                    return await FileFunctionsClass.SendToWebService(ftpConnect, ftpHoldFolder, clientID, filesToDownload, localFiles, null!);
                }
                else
                {
                    WriteLogClass.WriteToLog(1, $"Ftp file count: {filesToDownload.Count()}, Local file count: {localFiles.Length} files doesn't match", 3);
                    return 3;
                }
            }
            else
            {
                WriteLogClass.WriteToLog(1, "Folder empty ... skipping", 3);
                return 0;
            }
        }        
    }
}
