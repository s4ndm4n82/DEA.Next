using System.IO;
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
                InitiateDownload(FtpOnlyClient);                
            }

            return true;
        }

        public static async void InitiateDownload(UserConfigReaderClass.Customerdetail FtpClientDetails)
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

            var FtpSwitch = true;

            if (!Directory.Exists(FtpHoldFolder))
            {
                try
                {
                    Directory.CreateDirectory(FtpHoldFolder);                    
                }
                catch (Exception ex)
                {
                    WriteLogClass.WriteToLog(2, $"Exception at Ftp hold folder creation: {ex.Message}");
                    FtpSwitch = false;
                }
            }

            if (FtpSwitch)
            {
                using (var FtpConnect = await ConnectFtpClass.ConnectFtp(FtpHostName!, FtpHosIp!, FtpUser!, FtpPassword!))
                {
                    if (await FtpConnect.DirectoryExists(FtpPath))
                    {
                        /*foreach (FtpListItem item in await FtpConnect.GetListing($@"/{FtpMainFolder}/{FtpSubFolder}"))
                        {
                            WriteLogClass.WriteToLog(3, $"File {item.FullName}");
                        }*/
                        await DownloadFiles(FtpConnect, FtpPath, FtpHoldFolder);
                    }
                }
            }
        }

        public static async Task<bool> DownloadFiles(AsyncFtpClient _FtpConnect, string _FtpPath, string _FtpHoldFolder)
        {
            FtpListItem[] FtpFileItemList = await _FtpConnect.GetListing(_FtpPath);

            foreach (var FtpFileItem in FtpFileItemList)
            {
                await _FtpConnect.DownloadFile(_FtpHoldFolder, FtpFileItem.FullName, FtpLocalExists.Resume, FtpVerify.Retry);
            }

            return true;
        }
    }
}
