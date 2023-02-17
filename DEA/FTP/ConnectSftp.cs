using Renci.SshNet;
using WriteLog;

namespace ConnectSftp
{
    internal class ConnectSftpClass
    {
        public static SftpClient ConnectSftp(string HostName, string HostIp, string UserName, string UserPassword)
        {
            // SFTP implimention will be holted until it's needed.
            ConnectionInfo ConnInfo = new ConnectionInfo(HostName, UserName, new PasswordAuthenticationMethod(UserName, UserPassword));

            SftpClient SftpConnect = new SftpClient(ConnInfo);

            try
            {
                SftpConnect.Connect();
                WriteLogClass.WriteToLog(3, "SFTP Connection successful ....", 3);
            }
            catch
            {
                WriteLogClass.WriteToLog(2, "Trying to connect using alt method ....", 3);
                SftpConnect = ConnectSftpAlt(HostIp, UserName, UserPassword);
            }

            return SftpConnect;
        }

        private static SftpClient ConnectSftpAlt(string _HostIp, string _UserName, string _UserPassword)
        {
            ConnectionInfo ConnInfoAlt = new ConnectionInfo(_HostIp, _UserName, new PasswordAuthenticationMethod(_UserName, _UserPassword));

            SftpClient SftpConnectAlt = new SftpClient(ConnInfoAlt);

            try
            {
                SftpConnectAlt.Connect();
                WriteLogClass.WriteToLog(3, "SFTP Alt Connection successful....", 3);
            }
            catch (Exception ex)
            {
                WriteLogClass.WriteToLog(2, $"Exception at SFTP connection: {ex.Message}", 3);
            }

            return SftpConnectAlt;
        }
    }
}
