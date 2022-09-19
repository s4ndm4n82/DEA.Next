using System.IO;
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
                CheckFielsAndFolders(FtpOnlyClient);                
            }

            return true;
        }

        public static async void CheckFielsAndFolders(UserConfigReaderClass.Customerdetail FtpClientDetails)
        {
            var FtpHostName = FtpClientDetails.FtpDetails!.FtpHostName;
            var FtpHosIp = FtpClientDetails.FtpDetails.FtpHostIp;
            var FtpUser = FtpClientDetails.FtpDetails.FtpUser;
            var FtpPassword = FtpClientDetails.FtpDetails.FtpPassword;
            var FtpMainFolder = FtpClientDetails.FtpDetails.FtpMainFolder;
            var FtpSubFolder = FtpClientDetails.FtpDetails.FtpSubFolder;
            var CustomerFolderPath = $@"/{FtpMainFolder}/{FtpSubFolder}";

            WriteLogClass.WriteToLog(3, $"Ftp Folder Path: {CustomerFolderPath}");
            using (var FtpConnect = await ConnectFtpClass.ConnectFtp(FtpHostName!, FtpHosIp!, FtpUser!, FtpPassword!))
            {   
                foreach (FtpListItem item in await FtpConnect.GetListing(CustomerFolderPath))
                {
                    WriteLogClass.WriteToLog(3, $"File {item.FullName}");
                }
            }
        }
    }
}
