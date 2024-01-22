using FluentFTP;
using System.Net;
using WriteLog;
using static DEA.Next.FTP.FtpConnectionClasses.FtpProfilesSelector;
using static DEA.Next.FTP.FtpConnectionClasses.FtpProfileChecker;

namespace ConnectFtp
{
    internal class ConnectFtpClass
    {
        /// <summary>
        /// Creates the ftp connection using FluentFTP.
        /// </summary>
        /// <param name="ftpProfile">FTP settings profile name</param>
        /// <param name="hostName">FTP address to the server</param>
        /// <param name="userName">FTP username</param>
        /// <param name="userPassword">FTP Password</param>
        /// <param name="ftpPort">FTP port</param>
        /// <returns>The conncetion token.</returns>
        public static async Task<AsyncFtpClient> ConnectFtp(string ftpProfile,
                                                            string hostName,
                                                            string userName,
                                                            string userPassword,
                                                            int ftpPort)
        {
            // Check is the profile exists.
            if (!await CheckProfileExistsAsync(ftpProfile))
            {
                return null;
            }

            // Cancellation token.
            CancellationToken closeToken = new();

            // Get the FTP profile.
            FtpConfigurations ftpConfiguration = await GetFtpProfiles(ftpProfile);

            // Create the FTP connection.
            AsyncFtpClient ftpConnect = new()
            {                
                Host = hostName,
                Port = ftpPort,
                Credentials = new NetworkCredential(userName, userPassword),                
                Config = new FtpConfig()
                {
                    DataConnectionType = ftpConfiguration.DataConnectionType,
                    EncryptionMode = ftpConfiguration.EncryptionMode,
                    ValidateAnyCertificate = ftpConfiguration.ValidateCertificate
                }
            };

            try
            {
                // Makes the FTP connection.
                await ftpConnect.Connect(closeToken);
                WriteLogClass.WriteToLog(1, "FTP Connection successful ....", 3);
            }
            catch (Exception ex)
            {
                WriteLogClass.WriteToLog(1, $"Exception at FTP connecting to FTP: {ex.Message}", 3);
            }

            // Return the FTP connection.
            return ftpConnect;
        }        
    }
}
