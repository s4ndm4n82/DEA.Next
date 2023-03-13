using Renci.SshNet;
using WriteLog;

namespace ConnectSftp
{
    internal class ConnectSftpClass
    {
        public static SftpClient ConnectSftp(string hostName, string hostIp, string userName, string userPassword)
        {
            // SFTP implimention will be holted until it's needed.
            ConnectionInfo connInfo = new(hostName, userName, new PasswordAuthenticationMethod(userName, userPassword));

            SftpClient sftpConnect = new(connInfo);

            try
            {
                sftpConnect.Connect();
                WriteLogClass.WriteToLog(1, "SFTP Connection successful ....", 3);
            }
            catch
            {
                WriteLogClass.WriteToLog(1, "Trying to connect using alt method ....", 3);
                sftpConnect = ConnectSftpAlt(hostIp, userName, userPassword);
            }

            return sftpConnect;
        }

        private static SftpClient ConnectSftpAlt(string _hostIp, string _userName, string _userPassword)
        {
            ConnectionInfo connInfoAlt = new(_hostIp, _userName, new PasswordAuthenticationMethod(_userName, _userPassword));

            SftpClient sftpConnectAlt = new(connInfoAlt);

            try
            {
                sftpConnectAlt.Connect();
                WriteLogClass.WriteToLog(1, "SFTP Alt Connection successful....", 3);
            }
            catch (Exception ex)
            {
                WriteLogClass.WriteToLog(0, $"Exception at SFTP connection: {ex.Message}", 0);
            }

            return sftpConnectAlt;
        }
    }
}
