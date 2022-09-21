using DEA;
using WriteLog;
using ConnectFtp;
using FluentFTP;
using UserConfigReader;

namespace FtpFunctions
{
    internal class FtpFunctionsClass
    {
        public static async Task<bool> GetFtpFiles()
        {
            var JsonData = await UserConfigReaderClass.ReadAppDotConfig<UserConfigReaderClass.CustomerDetailsObject>();

            var FtpOnlyClients = JsonData.CustomerDetails!.Where(type => type.FtpDetails!.FtpType!.ToUpper() == "FTP");

            foreach(var FtpOnlyClient in FtpOnlyClients)
            {
                if(!await InitiateFtpDownload(FtpOnlyClient))
                {                    
                    continue;                    
                }
            }

            return true;
        }

        public static async Task<bool> InitiateFtpDownload(UserConfigReaderClass.Customerdetail FtpClientDetails)
        {
            var FtpHostName = FtpClientDetails.FtpDetails!.FtpHostName;
            var FtpHosIp = FtpClientDetails.FtpDetails.FtpHostIp;
            var FtpUser = FtpClientDetails.FtpDetails.FtpUser;
            var FtpPassword = FtpClientDetails.FtpDetails.FtpPassword;
            var FtpMainFolder = FtpClientDetails.FtpDetails.FtpMainFolder;
            var FtpSubFolder = FtpClientDetails.FtpDetails.FtpSubFolder;
            var FtpPath = $@"/{FtpMainFolder}/{FtpSubFolder}";

            var LocalFtpFolder = GraphHelper.CheckFolders("FTP");
            var FtpHoldFolder = Path.Combine(LocalFtpFolder, FtpMainFolder!, FtpSubFolder!);

            using (var FtpConnect = await ConnectFtpClass.ConnectFtp(FtpHostName!, FtpHosIp!, FtpUser!, FtpPassword!))
            {
                if (await FtpConnect.DirectoryExists(FtpPath))
                {
                    WriteLogClass.WriteToLog(3, $"Starting file download from {FtpPath} ....");

                    if (await DownloadFtpFiles(FtpConnect, FtpPath, FtpHoldFolder))
                    {
                        WriteLogClass.WriteToLog(3, "File download successful ....");
                        return true;
                    }
                    else
                    {
                        WriteLogClass.WriteToLog(3, "File download is not successful ....");
                    }                    
                }
                return false;
            }
        }

        public static async Task<bool> DownloadFtpFiles(AsyncFtpClient _FtpConnect, string _FtpPath, string _FtpHoldFolder)
        {
            FtpListItem[] FtpFileItemList = await _FtpConnect.GetListing(_FtpPath);

            IEnumerable<string> FilesToDownload = FtpFileItemList.Select(f => f.FullName.ToString());

            var Count = await _FtpConnect.DownloadFiles(_FtpHoldFolder, FilesToDownload, FtpLocalExists.Resume, FtpVerify.Retry);
            
            if (Count > 0)
            {
                WriteLogClass.WriteToLog(3, $"Downloaded {Count} file/s from {_FtpPath} to {_FtpHoldFolder} ....");

                var _LocalFiles = Directory.GetFiles(_FtpHoldFolder, "*.*", SearchOption.AllDirectories);
                var skip = false;

                foreach (var _LocalFile in _LocalFiles)
                {
                    var _LocalFileName = Path.GetFileName(_LocalFile);
                    foreach (var FileName in FilesToDownload)
                    {
                        var _FileName = Path.GetFileName(FileName);

                        if (_LocalFileName == _FileName)
                        {
                            if (await DeleteFtpFiles(_FtpConnect, FileName))
                            {
                                WriteLogClass.WriteToLog(3, $"Deleted file {_FileName} from {_FtpPath} ....");
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
                WriteLogClass.WriteToLog(3, "Folder empty ... skipping");
                return false;
            }            
        }

        public static async Task<bool> DeleteFtpFiles(AsyncFtpClient __FtpConnect, string FtpFileName)
        {
            try
            {
                await __FtpConnect.DeleteFile(FtpFileName);
                return true;
            }
            catch (Exception ex)
            {
                WriteLogClass.WriteToLog(2, $"Exception at FTP file deletetion: {ex.Message}");
                return false;
            }           
            
        }
    }
}
