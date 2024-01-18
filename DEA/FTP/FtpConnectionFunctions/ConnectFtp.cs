using FluentFTP;
using System.Net;
using WriteLog;

namespace ConnectFtp
{
    internal class ConnectFtpClass
    {
        public static async Task<AsyncFtpClient> ConnectFtp(string hostName, string hostIp, string userName, string userPassword)
        {
            CancellationToken closeToken = new();

            AsyncFtpClient ftpConnect = new()
            {
                Host = hostName,
                Credentials = new NetworkCredential(userName, userPassword),
            };

            try
            {
                await ftpConnect.Connect(closeToken);
                WriteLogClass.WriteToLog(1, "FTP Connection successful ....", 3);
            }
            catch
            {
                WriteLogClass.WriteToLog(1, $"Trying to connect using alt method ....", 3);
                await ConnectFtpAlt(hostIp, userName, userPassword);
            }

            return ftpConnect;
        }

        private static async Task<AsyncFtpClient> ConnectFtpAlt(string _hostIp, string _userName, string _userPassword)
        {
            CancellationToken closeToken = new();

            AsyncFtpClient ftpConnect = new()
            {
                Host = _hostIp,
                Credentials = new NetworkCredential(_userName, _userPassword)
            };

            try
            {
                await ftpConnect.Connect(closeToken);
                WriteLogClass.WriteToLog(1, "FTP Alt Connection successful ....", 3);
            }
            catch (Exception ex)
            {
                WriteLogClass.WriteToLog(0, $"Exception at FTP connection: {ex.Message}", 0);
                ftpConnect = null;
            }

            return ftpConnect;
        }
    }
}
