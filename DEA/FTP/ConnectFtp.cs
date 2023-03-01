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
                WriteLogClass.WriteToLog(1, "FTP Connection successful ....", 3);
            }
            catch
            {
                WriteLogClass.WriteToLog(1, $"Trying to connect using alt method ....", 3);
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
                WriteLogClass.WriteToLog(1, "FTP Alt Connection successful ....", 3);
            }
            catch (Exception ex)
            {
                WriteLogClass.WriteToLog(1, $"Exception at FTP connection: {ex.Message}", 3);
            }
            
            return FtpConnect;
        }
    }
}
