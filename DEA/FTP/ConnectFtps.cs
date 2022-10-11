using System.Net;
using FluentFTP;
using WriteLog;

namespace ConnectFtps
{
    internal class ConnectFtpsClass
    {
        public static async Task<AsyncFtpClient> ConnectFtps(string HostName, string HostIp, string UserName, string UserPassword)
        {
            var CancelToken = new CancellationToken();

            var FtpsConnect = new AsyncFtpClient
            {
                Host = HostName,
                Credentials = new NetworkCredential(UserName, UserPassword)
            };
            FtpsConnect.Config.EncryptionMode = FtpEncryptionMode.Explicit;
            FtpsConnect.Config.ValidateAnyCertificate = true;

            try
            {
                await FtpsConnect.Connect(CancelToken);
                WriteLogClass.WriteToLog(3, "FTPS Connection successful ....", "FTP");
            }
            catch
            {
                WriteLogClass.WriteToLog(3, $"Trying to connect using alt method ....", "FTP");
                FtpsConnect = await ConnectFtpsAlt(HostIp, UserName, UserPassword);
            }
            return FtpsConnect;
        }

        private static async Task<AsyncFtpClient> ConnectFtpsAlt(string _HostIp, string _UserName, string _UserPassword)
        {
            var CancelToken = new CancellationToken();

            var FtpsConnect = new AsyncFtpClient();

            FtpsConnect.Host = _HostIp;
            FtpsConnect.Credentials = new NetworkCredential(_UserName, _UserPassword);
            FtpsConnect.Config.EncryptionMode = FtpEncryptionMode.Explicit;
            FtpsConnect.Config.ValidateAnyCertificate = true;

            try
            {
                await FtpsConnect.Connect(CancelToken);
                WriteLogClass.WriteToLog(3, "FTPS Alt Connection successful ....", "FTP");
            }
            catch (Exception ex)
            {
                WriteLogClass.WriteToLog(2, $"Exception at FTPS connection: {ex.Message}", "FTP");
            }
            return FtpsConnect;
        }
    }
}
