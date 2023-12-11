using ConnectSftp;
using GraphHelper;
using WriteLog;
using UserConfigReader;
using FolderFunctions;

namespace SftpFunctions
{
    internal class SftpFunctionsClass
    {
        // SFTP implimention will be holted until it's needed.
        public static bool GetSftpFiles(int CustomerId)
        {
            var JsonData = UserConfigReaderClass.ReadUserDotConfig<UserConfigReaderClass.CustomerDetailsObject>();

            var SftpOnlyClient = JsonData.CustomerDetails!.FirstOrDefault(cid => cid.Id == CustomerId);

            if (SftpOnlyClient != null)
            {
                InitiateSftpDownload(SftpOnlyClient);
            }
            return true;
        }

        public static bool InitiateSftpDownload(UserConfigReaderClass.Customerdetail SftpClientDetails)
        {
            int clientID = SftpClientDetails.Id;
            string ftpType = SftpClientDetails.FtpDetails!.FtpType!;
            string ftpHostName = SftpClientDetails.FtpDetails!.FtpHostName!;
            string ftpHosIp = SftpClientDetails.FtpDetails.FtpHostIp!;
            string ftpUser = SftpClientDetails.FtpDetails.FtpUser!;
            string ftpPassword = SftpClientDetails.FtpDetails.FtpPassword!;
            string ftpMainFolder = SftpClientDetails.FtpDetails.FtpMainFolder!;
            string ftpSubFolder1 = SftpClientDetails.FtpDetails.FtpSubFolder1!.Replace(" ", "");
            string ftpSubFolder2 = SftpClientDetails.FtpDetails.FtpSubFolder2!.Replace(" ", "");
            string ftpPath;

            if (string.IsNullOrEmpty(ftpSubFolder2))
            {
                ftpPath = $@"/{ftpMainFolder}/{ftpSubFolder1}/{ftpSubFolder2}";
            }
            else
            {
                ftpPath = $@"/{ftpMainFolder}/{ftpSubFolder1}";
            }

            var LocalFtpFolder = FolderFunctionsClass.CheckFolders("ftp");
            var FtpHoldFolder = Path.Combine(LocalFtpFolder, ftpMainFolder!, ftpSubFolder1!);

            using var SftpConnect = ConnectSftpClass.ConnectSftp(ftpHostName!, ftpHosIp!, ftpUser!, ftpPassword!);

            if (SftpConnect.Exists(ftpPath))
            {
                WriteLogClass.WriteToLog(1, $"FTP Path {ftpPath}", 3);
            }

            return true;
        }
    }
}
