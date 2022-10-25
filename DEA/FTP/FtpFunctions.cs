using DEA;
using WriteLog;
using ConnectFtp;
using ConnectFtps;
using FluentFTP;
using UserConfigReader;

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
            var ftpType = FtpClientDetails.FtpDetails!.FtpType;
            var ftpHostName = FtpClientDetails.FtpDetails!.FtpHostName;
            var ftpHosIp = FtpClientDetails.FtpDetails.FtpHostIp;
            var ftpUser = FtpClientDetails.FtpDetails.FtpUser;
            var ftpPassword = FtpClientDetails.FtpDetails.FtpPassword;
            var ftpMainFolder = FtpClientDetails.FtpDetails.FtpMainFolder;
            var ftpSubFolder = FtpClientDetails.FtpDetails.FtpSubFolder!.Replace(" ", "");
            var ftpPath = $@"/{ftpMainFolder}/{ftpSubFolder}";

            var LocalFtpFolder = GraphHelper.CheckFolders("FTP");
            var FtpHoldFolder = Path.Combine(LocalFtpFolder, ftpMainFolder!, ftpSubFolder!);
            AsyncFtpClient ftp = null!;

            if (ftpType == "FTP")
            {
                ftp = await ConnectFtpClass.ConnectFtp(ftpHostName!, ftpHosIp!, ftpUser!, ftpPassword!);
            }
            else if (ftpType == "FTPS")
            {
                ftp = await ConnectFtpsClass.ConnectFtps(ftpHostName!, ftpHosIp!, ftpUser!, ftpPassword!);
            }

            using var ftpConnect = ftp;

            if (await ftpConnect.DirectoryExists(ftpPath))
            {
                WriteLogClass.WriteToLog(3, $"Starting file download from {ftpPath} ....", "FTP");

                if (await DownloadFtpFiles(ftpConnect, ftpPath, FtpHoldFolder))
                {
                    WriteLogClass.WriteToLog(3, "File download successful ....", "FTP");
                    return true;
                }
                else
                {
                    WriteLogClass.WriteToLog(3, "File download is not successful ....", "FTP");
                }
            }
            return false;
        }

        public static async Task<bool> DownloadFtpFiles(AsyncFtpClient ftpConnect, string ftpPath, string ftpHoldFolder)
        {
            FtpListItem[] FtpFileItemList = await ftpConnect.GetListing(ftpPath);

            IEnumerable<string> FilesToDownload = FtpFileItemList.Select(f => f.FullName.ToString());

            var Count = await ftpConnect.DownloadFiles(ftpHoldFolder, FilesToDownload, FtpLocalExists.Resume, FtpVerify.Retry);
            
            if (Count > 0)
            {
                WriteLogClass.WriteToLog(3, $"Downloaded {Count} file/s from {ftpPath} to {ftpHoldFolder} ....", "FTP");

                var localFiles = Directory.GetFiles(ftpHoldFolder, "*.*", SearchOption.AllDirectories);
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
                                WriteLogClass.WriteToLog(3, $"Deleted file {fileName} from {ftpPath} ....", "FTP");
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
                }                
                return true;
            }
            else
            {
                WriteLogClass.WriteToLog(3, "Folder empty ... skipping", "FTP");
                return false;
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
                WriteLogClass.WriteToLog(2, $"Exception at FTP file deletetion: {ex.Message}", "FTP");
                return false;
            }           
            
        }
    }
}
