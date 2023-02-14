using FluentFTP;
using System.Net;
using WriteLog;

namespace ConnectFtp
{
    internal class ConnectFtpClass
    {
        public static async Task<AsyncFtpClient> ConnectFtp(string HostName, string HostIp, string UserName,string UserPassword)
        {
            var CloseToken = new CancellationToken();

            var FtpConnect = new AsyncFtpClient
            {
                Host = HostName,
                Credentials = new NetworkCredential(UserName, UserPassword)
            };
            try
            {
                await FtpConnect.Connect(CloseToken);
                WriteLogClass.WriteToLog(3, "FTP Connection successful ....", string.Empty);
            }
            catch
            {
                WriteLogClass.WriteToLog(3, $"Trying to connect using alt method ....", string.Empty);
                await ConnectFtpAlt(HostIp, UserName, UserPassword);
            }

            return FtpConnect;
        }

        private static async Task<AsyncFtpClient> ConnectFtpAlt(string _HostIp, string _UserName, string _UserPassword)
        {
            var CloseToken = new CancellationToken();
            
            var FtpConnect = new AsyncFtpClient();
                FtpConnect.Host = _HostIp;
                FtpConnect.Credentials = new NetworkCredential(_UserName, _UserPassword);

            try
            {
                await FtpConnect.Connect(CloseToken);
                WriteLogClass.WriteToLog(3, "FTP Alt Connection successful ....", string.Empty);
            }
            catch (Exception ex)
            {
                WriteLogClass.WriteToLog(2, $"Exception at FTP connection: {ex.Message}", string.Empty);
            }
            
            return FtpConnect;
        }
    }
}
