using Renci.SshNet;
using WriteLog;

namespace ConnectSftp
{
    internal class ConnectSftpClass
    {
        /// <summary>
        /// Creates the sftp connection using Renci.sshnet.
        /// </summary>
        /// <param name="hostName"></param>
        /// <param name="userName"></param>
        /// <param name="userPassword"></param>
        /// <param name="ftpPort"></param>
        /// <returns>Connections string</returns>
        public static async Task<SftpClient> ConnectSftp(string hostName,
                                                         string userName,
                                                         string userPassword,
                                                         int ftpPort)
        {
            // Creating the SFTP connection string.
            ConnectionInfo connInfo = new(hostName, ftpPort, userName, new PasswordAuthenticationMethod(userName, userPassword));

            SftpClient sftpConnect = new(connInfo);

            try
            {
                // Getting the cancellation token.
                CancellationToken cancellationToken = new();
                
                // Connecting to the SFTP server.
                await sftpConnect.ConnectAsync(cancellationToken);
                
                WriteLogClass.WriteToLog(1, "SFTP Connection successful ....", 3);

                return sftpConnect;
            }
            catch
            {
                WriteLogClass.WriteToLog(1, "Trying to connect using alt method ....", 3);
                return null;
            }
        }
    }
}
