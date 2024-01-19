using System.Net;
using DEA.Next.FTP.FtpConnectionClasses;
using FluentFTP;
using WriteLog;

namespace ConnectFtps
{
    internal class ConnectFtpsClass
    {
        public static async Task<AsyncFtpClient> ConnectFtps(string ftpProfile,
                                                             string hostName,                                                             
                                                             string userName,
                                                             string userPassword,
                                                             int ftpPort)
        {
            CancellationToken cancelToken = new();

            FtpProfilesSelector.FtpConfigurations ftpConfiguration = await FtpProfilesSelector.GetFtpProfiles(ftpProfile);

            AsyncFtpClient ftpsConnect = new()
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
                await ftpsConnect.Connect(cancelToken);
                WriteLogClass.WriteToLog(1, "FTPS Connection successful ....", 3);
            }
            catch (Exception ex)
            {
                WriteLogClass.WriteToLog(0, $"Exception at connect FTPS: {ex.Message}", 0);
            }
            return ftpsConnect;
        }
    }
}
