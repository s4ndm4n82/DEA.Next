using System.Net;
using FluentFTP;
using WriteLog;

namespace ConnectFtps
{
    internal class ConnectFtpsClass
    {
        public static async Task<AsyncFtpClient> ConnectFtps(string hostName, string hostIp, string userName, string userPassword)
        {
            CancellationToken cancelToken = new();

            AsyncFtpClient ftpsConnect = new()
            {
                Host = hostName,
                Credentials = new NetworkCredential(userName, userPassword)
            };
            ftpsConnect.Config.EncryptionMode = FtpEncryptionMode.Explicit;
            ftpsConnect.Config.ValidateAnyCertificate = true;

            try
            {
                await ftpsConnect.Connect(cancelToken);
                WriteLogClass.WriteToLog(1, "FTPS Connection successful ....", 3);
            }
            catch
            {
                WriteLogClass.WriteToLog(1, $"Trying to connect using alt method ....", 3);
                ftpsConnect = await ConnectFtpsAlt(hostIp, userName, userPassword);
            }
            return ftpsConnect;
        }

        private static async Task<AsyncFtpClient> ConnectFtpsAlt(string _hostIp, string _userName, string _userPassword)
        {
            CancellationToken cancelToken = new();

            AsyncFtpClient ftpsConnect = new()
            {
                Host = _hostIp,
                Credentials = new NetworkCredential(_userName, _userPassword)
            };
            ftpsConnect.Config.EncryptionMode = FtpEncryptionMode.Explicit;
            ftpsConnect.Config.ValidateAnyCertificate = true;

            try
            {
                await ftpsConnect.Connect(cancelToken);
                WriteLogClass.WriteToLog(1, "FTPS Alt Connection successful ....", 3);
            }
            catch (Exception ex)
            {
                WriteLogClass.WriteToLog(0, $"Exception at FTPS connection: {ex.Message}", 0);
            }
            return ftpsConnect;
        }
    }
}
