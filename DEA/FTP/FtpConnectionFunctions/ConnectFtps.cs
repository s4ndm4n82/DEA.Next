using System.Net;
using FluentFTP;
using WriteLog;
using static DEA.Next.FTP.FtpConnectionClasses.FtpProfilesSelector;
using static DEA.Next.FTP.FtpConnectionClasses.FtpProfileChecker;

namespace ConnectFtps;

internal class ConnectFtpsClass
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
    public static async Task<AsyncFtpClient> ConnectFtps(string ftpProfile,
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
        CancellationToken cancelToken = new();

        // Get the FTP profile.
        FtpConfigurations ftpConfiguration = await GetFtpProfiles(ftpProfile);

        // Create the FTP connection.
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
            // Connect to the FTP server.
            await ftpsConnect.Connect(cancelToken);
            WriteLogClass.WriteToLog(1, "FTPS Connection successful ....", 3);
        }
        catch (Exception ex)
        {
            WriteLogClass.WriteToLog(0, $"Exception at connect FTPS: {ex.Message}", 0);
        }
        // Return the FTP connection.
        return ftpsConnect;
    }
}