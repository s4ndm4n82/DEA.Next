using ConnectFtp;
using ConnectFtps;
using ConnectSftp;
using DEA.Next.Entities;
using DEA.Next.HelperClasses.ConfigFileFunctions;
using DEA.Next.HelperClasses.OtherFunctions;
using DownloadFtpFilesClass;
using FluentFTP;
using FolderFunctions;
using FtpLoopDownloadClass;
using ProcessStatusMessageSetter;
using Renci.SshNet;
using WriteLog;

namespace DEA.Next.FTP.FtpFileRelatedFunctions;

internal class FtpFunctionsClass
{
    /// <summary>
    ///     This function is created to get the ftp files and this starts the ftp file download process.
    /// </summary>
    /// <param name="customerId"></param>
    /// <returns></returns>
    public static async Task<int> GetFtpFiles(Guid customerId)
    {
        // Starts the file download process if the client details are not empty.
        if (customerId != Guid.Empty) return await InitiateFtpDownload(customerId);
        return 0;
    }

    /// <summary>
    ///     Start the FTP file download process.
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

        var customerDetails = await UserConfigRetriever.RetrieveFtpConfigById(clientId);
        var ftpDetails = customerDetails.FtpDetails;

        if (ftpDetails == null)
        {
            WriteLogClass.WriteToLog(0, $"FTP details not found for client: {clientId} ....", 3);
            return downloadResult;
        }

        var downloadFolder = Path.Combine(FolderFunctionsClass.CheckFolders(MagicWords.Ftp)
            , ftpDetails.FtpMainFolder.Trim('/').Replace('/', '\\'));

        // If the user FTP config type is FTP.
        if (string.Equals(ftpDetails.FtpType, MagicWords.Ftp, StringComparison.OrdinalIgnoreCase))
            ftpConnectToken = await ConnectFtpClass.ConnectFtp(ftpDetails.FtpProfile,
                ftpDetails.FtpHost,
                ftpDetails.FtpUser,
                ftpDetails.FtpPassword,
                ftpDetails.FtpPort);

        // If the user FTP config type is FTPS.
        if (string.Equals(ftpDetails.FtpType, MagicWords.Ftps, StringComparison.OrdinalIgnoreCase))
            ftpConnectToken = await ConnectFtpsClass.ConnectFtps(ftpDetails.FtpProfile,
                ftpDetails.FtpHost,
                ftpDetails.FtpUser,
                ftpDetails.FtpPassword,
                ftpDetails.FtpPort);

        // the user FTP config type is SFTP
        if (string.Equals(ftpDetails.FtpType, MagicWords.Sftp, StringComparison.OrdinalIgnoreCase))
            sftpConnectToken = await ConnectSftpClass.ConnectSftp(ftpDetails.FtpHost,
                ftpDetails.FtpUser,
                ftpDetails.FtpPassword,
                ftpDetails.FtpPort);

        // Starts the file download from FTP or FTPS server.
        if (ftpConnectToken != null)
            downloadResult = await InitiateFtpDownload(ftpConnectToken, downloadFolder, clientId);

        // Starts the file download from SFTP server.
        if (sftpConnectToken != null)
            downloadResult = await InitiateSftpDownload(sftpConnectToken, downloadFolder, clientId);

        // If the connection token equals null then returns early terminating the execution.
        if (ftpConnectToken == null && ftpDetails.FtpType is MagicWords.Ftp or MagicWords.Ftps)
            WriteLogClass.WriteToLog(1, "Connection to FTP server failed ....", 3);

        if (sftpConnectToken == null && ftpDetails.FtpType == MagicWords.Sftp)
            WriteLogClass.WriteToLog(1, "Connection to SFTP server failed ....", 3);

        return downloadResult;
    }

    private static async Task<int> InitiateFtpDownload(AsyncFtpClient? ftpConnectToken,
        string downloadFolder,
        Guid clientId)
    {
        // Return result.
        var downloadResult = 0;

        // Get the FTP details from the config file.
        var customerDetails = await UserConfigRetriever.RetrieveFtpConfigById(clientId);
        var ftpDetails = customerDetails.FtpDetails;

        if (ftpDetails == null)
        {
            WriteLogClass.WriteToLog(0, $"FTP details not found for client: {clientId} ....", 3);
            return downloadResult;
        }

        try
        {
            await using (ftpConnectToken)
            {
                WriteLogClass.WriteToLog(1, $"Starting file download from {ftpDetails.FtpMainFolder} ....", 3);

                // downloadResult = ftpDetails.FtpFolderLoop switch
                // {
                //     true => await FtpLoopDownload.StartFtpLoopDownload(ftpConnectToken, null, ftpDetails.FtpMainFolder,
                //         downloadFolder, clientId),
                //     false => await FtpFilesDownload.DownloadFtpFilesFunction(ftpConnectToken, null,
                //         ftpDetails.FtpMainFolder, downloadFolder, string.Empty, clientId)
                // };
                //
                // WriteLogClass.WriteToLog(ProcessStatusMessageSetterClass.SetMessageTypeOther(downloadResult),
                //     $"{ProcessStatusMessageSetterClass.SetProcessStatusOther(downloadResult, MagicWords.Ftp)}\n", 3);

                if (ftpDetails.FtpFolderLoop)
                    downloadResult = await FtpLoopDownload.StartFtpLoopDownload(ftpConnectToken, null,
                        ftpDetails.FtpMainFolder,
                        downloadFolder, clientId);
                else
                    downloadResult = await FtpFilesDownload.DownloadFtpFilesFunction(ftpConnectToken, null,
                        ftpDetails.FtpMainFolder, downloadFolder, string.Empty, clientId);

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
        Guid clientId)
    {
        // Return result.
        var downloadResult = 0;

        // Get the FTP details from the config file.
        var customerDetails = await UserConfigRetriever.RetrieveFtpConfigById(clientId);
        var ftpDetails = customerDetails.FtpDetails;

        if (ftpDetails == null)
        {
            WriteLogClass.WriteToLog(0, $"FTP details not found for client: {clientId} ....", 3);
            return downloadResult;
        }

        try
        {
            using (sftpConnectToken)
            {
                WriteLogClass.WriteToLog(1, $"Starting file download from {ftpDetails.FtpMainFolder} ....", 3);

                downloadResult = ftpDetails.FtpFolderLoop switch
                {
                    true => await FtpLoopDownload.StartFtpLoopDownload(null, sftpConnectToken, ftpDetails.FtpMainFolder,
                        downloadFolder, clientId),
                    false => await FtpFilesDownload.DownloadFtpFilesFunction(null, sftpConnectToken,
                        ftpDetails.FtpMainFolder, downloadFolder, string.Empty, clientId)
                };

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
    ///     Move the ftp files to another FTP sub folder if the setting is true.
    /// </summary>
    /// <param name="ftpConnect"></param>
    /// <param name="sftpConnect"></param>
    /// <param name="clientId"></param>
    /// <param name="ftpFilesList"></param>
    /// <returns></returns>
    public static async Task<bool> MoveFtpFiles(AsyncFtpClient? ftpConnect,
        SftpClient? sftpConnect,
        Guid clientId,
        List<string> ftpFilesList)
    {
        var customerDetails = await UserConfigRetriever.RetrieveFtpConfigById(clientId);
        var ftpDetails = customerDetails.FtpDetails;

        if (ftpDetails == null)
        {
            WriteLogClass.WriteToLog(0, $"FTP details not found for client: {clientId} ....", 3);
            return false;
        }

        try
        {
            if (ftpFilesList.Count == 0)
            {
                WriteLogClass.WriteToLog(0, "Ftp files list is empty ....", 3);
                return false;
            }

            switch (ftpDetails.FtpType)
            {
                case MagicWords.Ftp:
                    return await MoveFtpFiles(ftpConnect, ftpDetails, ftpFilesList);
                case MagicWords.Sftp:
                    return await MoveSftpFiles(sftpConnect, ftpDetails, ftpFilesList);
                default:
                    WriteLogClass.WriteToLog(0, "Moving FTP files files ....", 3);
                    return false;
            }
        }
        catch (Exception ex)
        {
            WriteLogClass.WriteToLog(0, $"Exception at FTP file move: {ex.Message}", 0);
            return false;
        }
    }

    /// <summary>
    ///     Move the ftp files to another FTP sub folder if the setting is true.
    /// </summary>
    /// <param name="ftpConnect"></param>
    /// <param name="ftpDetails"></param>
    /// <param name="ftpFilesList"></param>
    /// <returns></returns>
    private static async Task<bool> MoveFtpFiles(AsyncFtpClient? ftpConnect,
        FtpDetails ftpDetails,
        List<string> ftpFilesList)
    {
        if (ftpConnect == null)
        {
            WriteLogClass.WriteToLog(0, "Ftp connection is null ....", 3);
            return false;
        }

        var loopCount = 0;

        foreach (var ftpFile in ftpFilesList)
        {
            if (!await ftpConnect.FileExists(ftpFile))
            {
                WriteLogClass.WriteToLog(0, $"Source file does not exist: {ftpFile} ....", 3);
                continue;
            }

            var ftpFileName = Path.GetFileName(ftpFile);
            var ftpDestinationPath = ftpDetails.FtpSubFolder;

            if (ftpDetails is { FtpFolderLoop: true, FtpMoveToSubFolder: true })
            {
                var loopFolderName = Path.GetFileName(Path.GetDirectoryName(ftpFile));
                ftpDestinationPath = string.Concat(ftpDetails.FtpSubFolder, "/", loopFolderName);
            }

            try
            {
                WriteLogClass.WriteToLog(1, $"Moving file: {ftpFileName} to {ftpDestinationPath} ....", 3);

                if (!await ftpConnect.DirectoryExists(ftpDestinationPath))
                    await ftpConnect.CreateDirectory(ftpDestinationPath);

                await ftpConnect.MoveFile(ftpFile, string.Concat(ftpDestinationPath, "/", ftpFileName));

                WriteLogClass.WriteToLog(1, $"File moved: {ftpFileName} to {ftpDestinationPath} ....", 3);
                loopCount++;
            }
            catch (Exception ex)
            {
                WriteLogClass.WriteToLog(0,
                    $"Failed to move file: {ftpFileName} to {ftpDestinationPath}. Exception: {ex.Message}", 3);
            }
        }

        if (loopCount == ftpFilesList.Count) return true;

        WriteLogClass.WriteToLog(0, $"Moving files to {ftpDetails.FtpSubFolder} unsuccessfully ....", 3);
        return false;
    }

    /// <summary>
    ///     Move the sftp files to another SFTP sub folder if the setting is true.
    /// </summary>
    /// <param name="sftpConnect"></param>
    /// <param name="ftpDetails"></param>
    /// <param name="sftpFilesList"></param>
    /// <returns></returns>
    private static async Task<bool> MoveSftpFiles(SftpClient? sftpConnect,
        FtpDetails ftpDetails,
        List<string> sftpFilesList)
    {
        if (sftpConnect == null)
        {
            WriteLogClass.WriteToLog(0, "SFTP connection is null ....", 3);
            return false;
        }

        var loopCount = 0;

        foreach (var sftpFile in sftpFilesList)
        {
            if (!await sftpConnect.ExistsAsync(sftpFile))
            {
                WriteLogClass.WriteToLog(0, $"Source file does not exist: {sftpFile} ....", 3);
                continue;
            }

            var sftpFileName = Path.GetFileName(sftpFile);
            var sftpDestinationPath = ftpDetails.FtpSubFolder;

            if (ftpDetails is { FtpFolderLoop: true, FtpMoveToSubFolder: true })
            {
                var loopFolderName = Path.GetFileName(Path.GetDirectoryName(sftpFile));
                sftpDestinationPath = string.Concat(ftpDetails.FtpSubFolder, "/", loopFolderName);
            }

            try
            {
                WriteLogClass.WriteToLog(1, $"Moving file: {sftpFileName} to {sftpDestinationPath} ....", 3);


                if (!await sftpConnect.ExistsAsync(sftpDestinationPath))
                    await sftpConnect.CreateDirectoryAsync(sftpDestinationPath);

                await sftpConnect.RenameFileAsync(sftpFile, string.Concat(sftpDestinationPath, "/", sftpFileName),
                    CancellationToken.None);

                if (await sftpConnect.ExistsAsync(sftpFile))
                    await sftpConnect.DeleteFileAsync(sftpFile, CancellationToken.None);

                WriteLogClass.WriteToLog(1, $"File moved: {sftpFileName} to {sftpDestinationPath} ....", 3);
                loopCount++;
            }
            catch (Exception ex)
            {
                WriteLogClass.WriteToLog(0,
                    $"Failed to move file: {sftpFileName} to {sftpDestinationPath}. Exception: {ex.Message}", 3);
            }
        }

        if (loopCount == sftpFilesList.Count()) return true;

        WriteLogClass.WriteToLog(0, $"Moving files to {ftpDetails.FtpSubFolder} unsuccessfull ....", 3);
        return false;
    }
}