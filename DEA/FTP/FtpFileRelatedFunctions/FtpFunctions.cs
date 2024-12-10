using WriteLog;
using ConnectFtp;
using ConnectFtps;
using FluentFTP;
using FolderFunctions;
using ProcessStatusMessageSetter;
using DownloadFtpFilesClass;
using FtpLoopDownloadClass;
using DEA.Next.HelperClasses.OtherFunctions;
using UserConfigRetriverClass;
using Renci.SshNet;
using ConnectSftp;
using static UserConfigSetterClass.UserConfigSetter;

namespace FtpFunctions;

internal class FtpFunctionsClass
{
    /// <summary>
    /// This function is created to get the ftp files and this starts the ftp file download process.
    /// </summary>
    /// <param name="customerId"></param>
    /// <returns></returns>
    public static async Task<int> GetFtpFiles(Guid customerId)
    {
        // Starts the file download process if the client details are not empty.
        if (customerId != default)
        {
            return await InitiateFtpDownload(customerId);
        }
        return 0;
    }

    /// <summary>
    /// Start the FTP file download process.
    /// </summary>
    /// <param name="clientId"></param>
    /// <returns>Return 1 or 0.</returns>
    private static async Task<int> InitiateFtpDownload(Guid clientId)
    {
        // Return result.
        var downloadResult = 0;
        // FTP connection token
        AsyncFtpClient? ftpConnectToken = null;
        // SFTP connection token
        SftpClient? sftpConnectToken = null;

        var ftpDetails = await UserConfigRetriver.RetriveFtpConfigById(clientId);

        string downloadFolder = Path.Combine(FolderFunctionsClass.CheckFolders(MagicWords.Ftp)
            , ftpDetails.FtpMainFolder.Trim('/').Replace('/', '\\'));

        // If the user FTP config type is FTP.
        if (string.Equals(ftpDetails.FtpType, MagicWords.Ftp, StringComparison.OrdinalIgnoreCase))
        {
            ftpConnectToken = await ConnectFtpClass.ConnectFtp(ftpDetails.FtpProfile,
                ftpDetails.FtpHostName,
                ftpDetails.FtpUser,
                ftpDetails.FtpPassword,
                ftpDetails.FtpPort);
        }

        // If the user FTP config type is FTPS.
        if (string.Equals(ftpDetails.FtpType, MagicWords.Ftps, StringComparison.OrdinalIgnoreCase))
        {
            ftpConnectToken = await ConnectFtpsClass.ConnectFtps(ftpDetails.FtpProfile,
                ftpDetails.FtpHostName,
                ftpDetails.FtpUser,
                ftpDetails.FtpPassword,
                ftpDetails.FtpPort);
        }

        // the user FTP config type is SFTP
        if (string.Equals(ftpDetails.FtpType, MagicWords.Sftp, StringComparison.OrdinalIgnoreCase))
        {
            sftpConnectToken = await ConnectSftpClass.ConnectSftp(ftpDetails.FtpHostName,
                ftpDetails.FtpUser,
                ftpDetails.FtpPassword,
                ftpDetails.FtpPort);
        }

        // Starts the file download from FTP or FTPS server.
        if (ftpConnectToken != null)
        {
            downloadResult = await InitiateFtpDownload(ftpConnectToken, downloadFolder, clientId);
        }

        // Starts the file download from SFTP server.
        if (sftpConnectToken != null)
        {
            downloadResult = await InitiateSftpDownload(sftpConnectToken, downloadFolder, clientId);
        }

        // If the connection token equals null then returns early terminating the execution.
        if (ftpConnectToken == null && (ftpDetails.FtpType == MagicWords.Ftp || ftpDetails.FtpType == MagicWords.Ftps))
        {
            WriteLogClass.WriteToLog(1, "Connection to FTP server failed ....", 3);
        }

        if (sftpConnectToken == null && ftpDetails.FtpType == MagicWords.Sftp)
        {
            WriteLogClass.WriteToLog(1, "Connection to SFTP server failed ....", 3);
        }

        return downloadResult;
    }

    private static async Task<int> InitiateFtpDownload(AsyncFtpClient? ftpConnectToken,
        string downloadFolder, int clientId)
    {
        // Return result.
        int downloadResult = 0;
        // Get the FTP details from the config file.
        Ftpdetails ftpDetails = await UserConfigRetriver.RetriveFtpConfigById(clientId);

        try
        {
            using (ftpConnectToken)
            {
                WriteLogClass.WriteToLog(1, $"Starting file download from {ftpDetails.FtpMainFolder} ....", 3);

                if (ftpDetails.FtpFolderLoop == 1)
                {
                    downloadResult = await FtpLoopDownload.StartFtpLoopDownload(ftpConnectToken,
                        null,
                        ftpDetails.FtpMainFolder,
                        downloadFolder,
                        clientId);
                }

                if (ftpDetails.FtpFolderLoop == 0)
                {
                    downloadResult = await FtpFilesDownload.DownloadFtpFilesFunction(ftpConnectToken,
                        null,
                        ftpDetails.FtpMainFolder,
                        downloadFolder,
                        string.Empty,
                        clientId);
                }

                WriteLogClass.WriteToLog(ProcessStatusMessageSetterClass.SetMessageTypeOther(downloadResult),
                    $"{ProcessStatusMessageSetterClass.SetProcessStatusOther(downloadResult, MagicWords.Ftp)}\n", 3);
                return downloadResult;
            }
        }
        catch (Exception ex)
        {
            WriteLogClass.WriteToLog(0, $"Exception at FTP file download: {ex.Message}", 0);
            return downloadResult;
        }
    }

    private static async Task<int> InitiateSftpDownload(SftpClient? sftpConnectToken,
        string downloadFolder,
        int clientId)
    {
        // Return result.
        int downloadResult = 0;
        // Get the FTP details from the config file.
        Ftpdetails ftpDetails = await UserConfigRetriver.RetriveFtpConfigById(clientId);
        try
        {
            using (sftpConnectToken)
            {
                WriteLogClass.WriteToLog(1, $"Starting file download from {ftpDetails.FtpMainFolder} ....", 3);

                if (ftpDetails.FtpFolderLoop == 1)
                {
                    downloadResult = await FtpLoopDownload.StartFtpLoopDownload(null,
                        sftpConnectToken,
                        ftpDetails.FtpMainFolder,
                        downloadFolder,
                        clientId);
                }

                if (ftpDetails.FtpFolderLoop == 0)
                {
                    downloadResult = await FtpFilesDownload.DownloadFtpFilesFunction(null,
                        sftpConnectToken,
                        ftpDetails.FtpMainFolder,
                        downloadFolder,
                        string.Empty,
                        clientId);
                }

                WriteLogClass.WriteToLog(ProcessStatusMessageSetterClass.SetMessageTypeOther(downloadResult),
                    $"{ProcessStatusMessageSetterClass.SetProcessStatusOther(downloadResult, MagicWords.Ftp)}\n", 3);
                return downloadResult;
            }
        }
        catch (Exception ex)
        {
            WriteLogClass.WriteToLog(0, $"Exception at SFTP file download: {ex.Message}", 0);
            return downloadResult;
        }
    }

    /// <summary>
    /// Move the ftp files to another FTP sub folder if the setting is true.
    /// </summary>
    /// <param name="ftpConnect"></param>
    /// <param name="clientId"></param>
    /// <param name="ftpFilesList"></param>
    /// <returns></returns>
    public static async Task<bool> MoveFtpFiles(AsyncFtpClient ftpConnect,
        SftpClient sftpConnect,
        int clientId,
        IEnumerable<string> ftpFilesList)
    {
        Ftpdetails ftpDetails = await UserConfigRetriver.RetriveFtpConfigById(clientId);

        try
        {
            if (!ftpFilesList.Any())
            {
                WriteLogClass.WriteToLog(0, $"Ftp files list is empty ....", 3);
                return false;
            }

            if (ftpConnect != null)
            {
                return await MoveFtpFiles(ftpConnect, ftpDetails, ftpFilesList);
            }

            if (sftpConnect != null)
            {
                return await MoveSftpFiles(sftpConnect, ftpDetails, ftpFilesList);
            }

            WriteLogClass.WriteToLog(0, $"Moving FTP files failes ....", 3);
            return false;
        }
        catch (Exception ex)
        {
            WriteLogClass.WriteToLog(0, $"Exception at FTP file move: {ex.Message}", 0);
            return false;
        }
    }

    /// <summary>
    /// Move the ftp files to another FTP sub folder if the setting is true.
    /// </summary>
    /// <param name="ftpConnect"></param>
    /// <param name="ftpDetails"></param>
    /// <param name="ftpFilesList"></param>
    /// <returns></returns>
    private static async Task<bool> MoveFtpFiles(AsyncFtpClient ftpConnect,
        Ftpdetails ftpDetails,
        IEnumerable<string> ftpFilesList)
    {
        if (ftpConnect == null)
        {
            WriteLogClass.WriteToLog(0, $"Ftp connection is null ....", 3);
            return false;
        }

        int loopCount = 0;

        foreach (string ftpFile in ftpFilesList)
        {
            if (!await ftpConnect.FileExists(ftpFile))
            {
                WriteLogClass.WriteToLog(0, $"Source file does not exist: {ftpFile} ....", 3);
                continue;
            }

            string ftpFileName = Path.GetFileName(ftpFile);
            string ftpDestinationPath = ftpDetails.FtpSubFolder;

            if (ftpDetails.FtpFolderLoop == 1 && ftpDetails.FtpMoveToSubFolder)
            {
                string loopFolderName = Path.GetFileName(Path.GetDirectoryName(ftpFile));
                ftpDestinationPath = string.Concat(ftpDetails.FtpSubFolder, "/", loopFolderName);
            }

            try
            {
                WriteLogClass.WriteToLog(1, $"Moving file: {ftpFileName} to {ftpDestinationPath} ....", 3);

                if (!await ftpConnect.DirectoryExists(ftpDestinationPath))
                {
                    await ftpConnect.CreateDirectory(ftpDestinationPath);
                }

                await ftpConnect.MoveFile(ftpFile, string.Concat(ftpDestinationPath, "/", ftpFileName));

                WriteLogClass.WriteToLog(1, $"File moved: {ftpFileName} to {ftpDestinationPath} ....", 3);
                loopCount++;
            }
            catch (Exception ex)
            {
                WriteLogClass.WriteToLog(0, $"Failed to move file: {ftpFileName} to {ftpDestinationPath}. Exception: {ex.Message}", 3);
            }
        }

        if (loopCount == ftpFilesList.Count())
        {
            return true;
        }

        WriteLogClass.WriteToLog(0, $"Moving files to {ftpDetails.FtpSubFolder} unsuccessfull ....", 3);
        return false;
    }

    /// <summary>
    /// Move the sftp files to another SFTP sub folder if the setting is true.
    /// </summary>
    /// <param name="sftpConnect"></param>
    /// <param name="ftpDetails"></param>
    /// <param name="sftpFilesList"></param>
    /// <returns></returns>
    private static async Task<bool> MoveSftpFiles(SftpClient sftpConnect,
        Ftpdetails ftpDetails,
        IEnumerable<string> sftpFilesList)
    {
        if (sftpConnect == null)
        {
            WriteLogClass.WriteToLog(0, $"SFTP connection is null ....", 3);
            return false;
        }

        int loopCount = 0;

        foreach (string sftpFile in sftpFilesList)
        {
            if (!sftpConnect.Exists(sftpFile))
            {
                WriteLogClass.WriteToLog(0, $"Source file does not exist: {sftpFile} ....", 3);
                continue;
            }

            string sftpFileName = Path.GetFileName(sftpFile);
            string sftpDestinationPath = ftpDetails.FtpSubFolder;

            if (ftpDetails.FtpFolderLoop == 1 && ftpDetails.FtpMoveToSubFolder)
            {
                string loopFolderName = Path.GetFileName(Path.GetDirectoryName(sftpFile));
                sftpDestinationPath = string.Concat(ftpDetails.FtpSubFolder, "/", loopFolderName);
            }

            try
            {
                WriteLogClass.WriteToLog(1, $"Moving file: {sftpFileName} to {sftpDestinationPath} ....", 3);


                if (!sftpConnect.Exists(sftpDestinationPath))
                {
                    sftpConnect.CreateDirectory(sftpDestinationPath);
                }

                await sftpConnect.RenameFileAsync(sftpFile, string.Concat(sftpDestinationPath, "/", sftpFileName), CancellationToken.None);

                if (sftpConnect.Exists(sftpFile))
                {
                    await sftpConnect.DeleteFileAsync(sftpFile, CancellationToken.None);
                }

                WriteLogClass.WriteToLog(1, $"File moved: {sftpFileName} to {sftpDestinationPath} ....", 3);
                loopCount++;
            }
            catch (Exception ex)
            {
                WriteLogClass.WriteToLog(0, $"Failed to move file: {sftpFileName} to {sftpDestinationPath}. Exception: {ex.Message}", 3);
            }
        }

        if (loopCount == sftpFilesList.Count())
        {
            return true;
        }

        WriteLogClass.WriteToLog(0, $"Moving files to {ftpDetails.FtpSubFolder} unsuccessfull ....", 3);
        return false;
    }
}