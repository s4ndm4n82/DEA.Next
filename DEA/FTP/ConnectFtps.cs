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
                WriteLogClass.WriteToLog(1, "FTPS Connection successful ....", 3);
            }
            catch
            {
                WriteLogClass.WriteToLog(1, $"Trying to connect using alt method ....", 3);
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
                WriteLogClass.WriteToLog(1, "FTPS Alt Connection successful ....", 3);
            }
            catch (Exception ex)
            {
                WriteLogClass.WriteToLog(0, $"Exception at FTPS connection: {ex.Message}", 0);
            }
            return FtpsConnect;
        }
    }
}
