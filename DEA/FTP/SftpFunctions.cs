using ConnectSftp;
using DEA;
using WriteLog;
using UserConfigReader;

namespace SftpFunctions
{
    internal class SftpFunctionsClass
    {
        // SFTP implimention will be holted until it's needed.
        public static bool GetSftpFiles(int CustomerId)
        {
            var JsonData = UserConfigReaderClass.ReadAppDotConfig<UserConfigReaderClass.CustomerDetailsObject>();

            var SftpOnlyClient = JsonData.CustomerDetails!.FirstOrDefault(cid => cid.id == CustomerId);

            if (SftpOnlyClient != null)
            {
                InitiateSftpDownload(SftpOnlyClient);
            }
            return true;
        }

        public static bool InitiateSftpDownload(UserConfigReaderClass.Customerdetail SftpClientDetails)
        {
            var FtpType = SftpClientDetails.FtpDetails!.FtpType;
            var FtpHostName = SftpClientDetails.FtpDetails!.FtpHostName;
            var FtpHosIp = SftpClientDetails.FtpDetails.FtpHostIp;
            var FtpUser = SftpClientDetails.FtpDetails.FtpUser;
            var FtpPassword = SftpClientDetails.FtpDetails.FtpPassword;
            var FtpMainFolder = SftpClientDetails.FtpDetails.FtpMainFolder;
            var FtpSubFolder = SftpClientDetails.FtpDetails.FtpSubFolder!.Replace(" ", "");
            var FtpPath = $@"/{FtpMainFolder}/{FtpSubFolder}";

            var LocalFtpFolder = GraphHelper.CheckFolders("FTP");
            var FtpHoldFolder = Path.Combine(LocalFtpFolder, FtpMainFolder!, FtpSubFolder!);

            using var SftpConnect = ConnectSftpClass.ConnectSftp(FtpHostName!, FtpHosIp!, FtpUser!, FtpPassword!);

            if (SftpConnect.Exists(FtpPath))
            {
                WriteLogClass.WriteToLog(3, $"FTP Path {FtpPath}", "FTP");
            }

            return true;
        }
    }
}
