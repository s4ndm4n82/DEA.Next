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
            /*var HostName = "localhost";
            var HostIp = "127.0.0.1";
            var UserName = "testuser";
            var UserPassword = "jackkills";*/

            var FtpConnect = new AsyncFtpClient();
                FtpConnect.Host = HostName;
                FtpConnect.Credentials = new NetworkCredential(UserName, UserPassword);
            try
            {
                await FtpConnect.Connect(CloseToken);
                WriteLogClass.WriteToLog(3, "FTP Connection successful ....");
            }
            catch
            {
                WriteLogClass.WriteToLog(3, $"Trying to connect using alt method ....");
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
                WriteLogClass.WriteToLog(3, "FTP Alt Connection successful ....");
            }
            catch (Exception ex)
            {
                WriteLogClass.WriteToLog(2, $"Exception at FTP connection: {ex.Message}");
            }
            
            return FtpConnect;
        }
    }
}
