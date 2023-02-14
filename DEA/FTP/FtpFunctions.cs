using DEA;
using WriteLog;
using ConnectFtp;
using ConnectFtps;
using FluentFTP;
using UserConfigReader;
using FileFunctions;

namespace FtpFunctions
{
    internal class FtpFunctionsClass
    {
        public static async Task<bool> GetFtpFiles(int Customerid)
        {
            var JsonData = UserConfigReaderClass.ReadAppDotConfig<UserConfigReaderClass.CustomerDetailsObject>();

            var clientDetails = JsonData.CustomerDetails!.FirstOrDefault(cid => cid.id == Customerid);

            if (clientDetails != null)
            {
                await InitiateFtpDownload(clientDetails!);
            }
            return true;
        }

        public static async Task<bool> InitiateFtpDownload(UserConfigReaderClass.Customerdetail FtpClientDetails)
        {
            int clientID = FtpClientDetails.id;
            string ftpType = FtpClientDetails.FtpDetails!.FtpType!;
            string ftpHostName = FtpClientDetails.FtpDetails!.FtpHostName!;
            string ftpHosIp = FtpClientDetails.FtpDetails.FtpHostIp!;
            string ftpUser = FtpClientDetails.FtpDetails.FtpUser!;
            string ftpPassword = FtpClientDetails.FtpDetails.FtpPassword!;
            string ftpMainFolder = FtpClientDetails.FtpDetails.FtpMainFolder!;
            string ftpSubFolder1 = FtpClientDetails.FtpDetails.FtpSubFolder1!.Replace(" ", "");
            string ftpSubFolder2 = FtpClientDetails.FtpDetails.FtpSubFolder2!.Replace(" ", "");
            string ftpPath;

            if (!string.IsNullOrEmpty(ftpSubFolder2))
            {
                ftpPath = $@"/{ftpMainFolder}/{ftpSubFolder1}/{ftpSubFolder2}";
            }
            else
            {
                ftpPath = $@"/{ftpMainFolder}/{ftpSubFolder1}";
            }            

            string LocalFtpFolder = GraphHelper.CheckFolders("FTP");
            string FtpHoldFolder = Path.Combine(LocalFtpFolder, ftpMainFolder!, ftpSubFolder1!);
            
            AsyncFtpClient ftp = null!;

            if (ftpType == "FTP")
            {
                ftp = await ConnectFtpClass.ConnectFtp(ftpHostName!, ftpHosIp!, ftpUser!, ftpPassword!);
            }
            else if (ftpType == "FTPS")
            {
                ftp = await ConnectFtpsClass.ConnectFtps(ftpHostName!, ftpHosIp!, ftpUser!, ftpPassword!);
            }

            using AsyncFtpClient ftpConnect = ftp;

            if (await ftpConnect.DirectoryExists(ftpPath))
            {
                WriteLogClass.WriteToLog(3, $"Starting file download from {ftpPath} ....", string.Empty);

                if (await DownloadFtpFiles(ftpConnect, ftpPath, FtpHoldFolder, clientID))
                {
                    return await FileFunctionsClass.SendToWebService(FtpHoldFolder, clientID);
                }
                else
                {
                    WriteLogClass.WriteToLog(3, "File download is not successful ....", string.Empty);
                }
            }
            return false;
        }

        public static async Task<bool> DownloadFtpFiles(AsyncFtpClient ftpConnect, string ftpPath, string ftpHoldFolder, int clientID)
        {
            // To capture the success flag from upload function.
            bool returnFlag = false;

            // Initiate FTP connect and gets the file list from the FTP server.
            List<FtpListItem> ftpFilesOnly = new List<FtpListItem>();

            foreach (var fileItem in await ftpConnect.GetListing(ftpPath))
            {
                // Only select files only. Sub directories are skipped.
                if (fileItem.Type == FtpObjectType.File)
                {
                    ftpFilesOnly.Add(new FtpListItem() { FullName = fileItem.FullName });
                }                
            }
            
            // Puts the full filename into a string list.
            IEnumerable<string> FilesToDownload = ftpFilesOnly.Select(f => f.FullName.ToString());

            // Downloads files from the server and counts them.
            List<FtpResult> downloadedFileList = await ftpConnect.DownloadFiles(ftpHoldFolder, FilesToDownload, FtpLocalExists.Resume, FtpVerify.Retry);
            
            if (downloadedFileList.Count > 0)
            {
                string lastFolderName = Path.GetFileName(ftpHoldFolder); //Path.GetFullPath(ftpHoldFolder).Split(Path.DirectorySeparatorChar).Last();                
                string parentName = Path.GetFileName(Directory.GetParent(ftpHoldFolder)!.FullName);

                WriteLogClass.WriteToLog(3, $"Downloaded {downloadedFileList.Count} file/s from {ftpPath} to \\{parentName}\\{lastFolderName} folder ....", string.Empty);
                return returnFlag = true;
                /*var localFiles = Directory.GetFiles(ftpHoldFolder, "*.*", SearchOption.AllDirectories);
                var skip = false;

                foreach (var _LocalFile in localFiles)
                {
                    var localFileName = Path.GetFileName(_LocalFile);
                    foreach (var FileName in FilesToDownload)
                    {
                        var fileName = Path.GetFileName(FileName);

                        if (localFileName == fileName)
                        {
                            if (await DeleteFtpFiles(ftpConnect, FileName))
                            {
                                WriteLogClass.WriteToLog(3, $"Deleted file {fileName} from {ftpPath} ....", string.Empty);
                                skip = true;
                                break;
                            }
                        }
                    }

                    if (skip)
                    {
                        continue;
                    }
                    else
                    {
                        break;
                    }
                }*/
            }
            else
            {
                WriteLogClass.WriteToLog(3, "Folder empty ... skipping", string.Empty);
                return returnFlag;
            }
        }

        public static async Task<bool> DeleteFtpFiles(AsyncFtpClient ftpConnect, string ftpFileName)
        {
            try
            {
                await ftpConnect.DeleteFile(ftpFileName);
                return true;
            }
            catch (Exception ex)
            {
                WriteLogClass.WriteToLog(2, $"Exception at FTP file deletetion: {ex.Message}", string.Empty);
                return false;
            }           
            
        }
    }
}
