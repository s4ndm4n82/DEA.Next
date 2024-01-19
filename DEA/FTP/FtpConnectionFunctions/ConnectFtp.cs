using DEA.Next.FTP.FtpConnectionClasses;
using FluentFTP;
using System.Net;
using WriteLog;
using static DEA.Next.FTP.FtpConnectionClasses.FtpProfilesSelector;

namespace ConnectFtp
{
    internal class ConnectFtpClass
    {
        public static async Task<AsyncFtpClient> ConnectFtp(string ftpProfile,
                                                            string hostName,
                                                            string userName,
                                                            string userPassword,
                                                            int ftpPort)
        {
            CancellationToken closeToken = new();

            FtpProfilesSelector.FtpConfigurations ftpConfiguration = await FtpProfilesSelector.GetFtpProfiles(ftpProfile);

            AsyncFtpClient ftpConnect = new()
            {                
                Host = hostName,
                Port = ftpPort,
                Credentials = new NetworkCredential(userName, userPassword),                
                Config = new FtpConfig()
                {
                    DataConnectionType = ftpConfiguration.DataConnectionType,
                    EncryptionMode = ftpConfiguration.EncryptionMode
                }
            };

            try
            {
                await ftpConnect.Connect(closeToken);
                WriteLogClass.WriteToLog(1, "FTP Connection successful ....", 3);
            }
            catch (Exception ex)
            {
                WriteLogClass.WriteToLog(1, $"Exception at FTP connecting to FTP: {ex.Message}", 3);
            }

            return ftpConnect;
        }

        private static async Task<bool> CheckProfileExistsAsync(string ftpProfile)
        {
            HashSet<string> validProfiles = new()
            {
                FtpProfileList.ProfilePxe,
                FtpProfileList.ProfileEpe,
                FtpProfileList.ProfileFsv
            };

            if (!validProfiles.Contains(ftpProfile))
            {
                return false;
            }

            return true;
        }
    }
}
