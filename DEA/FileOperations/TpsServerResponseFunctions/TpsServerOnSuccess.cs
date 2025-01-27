using DEA.Next.FTP.FtpFileRelatedFunctions;
using DEA.Next.HelperClasses.ConfigFileFunctions;
using DEA.Next.HelperClasses.FolderFunctions;
using DEA.Next.HelperClasses.OtherFunctions;
using FluentFTP;
using GraphMoveEmailsToExportClass;
using Microsoft.Graph;
using Renci.SshNet;
using WriteLog;
using WriteNamesToLog;

namespace DEA.Next.FileOperations.TpsServerResponseFunctions;

/// <summary>
///     Handles the operations after a successful TPS server response.
/// </summary>
internal class TpsServerOnSuccess
{
    /// <summary>
    ///     Normal project upload.
    /// </summary>
    /// <param name="projectId"></param>
    /// <param name="queue"></param>
    /// <param name="fileCount"></param>
    /// <param name="deliveryType"></param>
    /// <param name="fullFilePath"></param>
    /// <param name="downloadFolderPath"></param>
    /// <param name="jsonFileList"></param>
    /// <param name="customerId"></param>
    /// <param name="clientOrgNo"></param>
    /// <param name="ftpConnect"></param>
    /// <param name="sftpConnect"></param>
    /// <param name="ftpFileList"></param>
    /// <param name="localFileList"></param>
    /// <returns></returns>
    public static async Task<int> ServerOnSuccessProjectAsync(string projectId,
        int queue,
        int fileCount,
        string deliveryType,
        string fullFilePath,
        string downloadFolderPath,
        string[] jsonFileList,
        Guid customerId,
        string clientOrgNo,
        AsyncFtpClient? ftpConnect,
        SftpClient? sftpConnect,
        string[] ftpFileList,
        string[] localFileList)
    {
        try
        {
            var customerDetails = await UserConfigRetriever.RetrieveFtpConfigById(customerId);
            var ftpDetails = customerDetails.FtpDetails;

            // Writes to log.
            WriteLogClass.WriteToLog(1,
                $"Uploaded {fileCount} file to project {projectId} using queue {queue} ....",
                4);

            WriteLogClass.WriteToLog(1,
                $"Uploaded filenames: {WriteNamesToLogClass.GetFileNames(jsonFileList)}",
                4);

            switch (deliveryType)
            {
                // This will run if it's not FTP.
                case MagicWords.Email when !await FolderCleanerClass.GetFolders(fullFilePath,
                    jsonFileList,
                    null,
                    clientOrgNo,
                    MagicWords.Email): return 1;

                // Removes the files from FTP server. If the files not needed to be moved to another FTP sub folder.
                case MagicWords.Ftp when ftpDetails is { FtpMoveToSubFolder: false, FtpRemoveFiles: true }
                                         && !await FolderCleanerClass.StartFtpFileDelete(ftpConnect,
                                             sftpConnect,
                                             ftpFileList,
                                             localFileList):
                    WriteLogClass.WriteToLog(0,
                        "Deleting files from FTP failed ....",
                        1);
                    return -1;

                // Moving files to another FTP sub folder.
                case MagicWords.Ftp when ftpDetails != null
                                         && ftpDetails.FtpMoveToSubFolder
                                         && !await FtpFunctionsClass.MoveFtpFiles(ftpConnect,
                                             sftpConnect,
                                             customerId,
                                             ftpFileList.ToList()):
                    WriteLogClass.WriteToLog(0,
                        "Moving files to FTP sub folder failed ....",
                        1);
                    await FolderCleanerClass.StartFtpFileDelete(ftpConnect,
                        sftpConnect,
                        ftpFileList,
                        localFileList);
                    return -1;

                // Deletes the file from local hold folder when sending is successful.
                case MagicWords.Ftp when !await FolderCleanerClass.GetFolders(downloadFolderPath,
                    jsonFileList,
                    customerId,
                    null,
                    MagicWords.Ftp): return -1;

                default:
                    return 1;
            }
        }
        catch (Exception ex)
        {
            WriteLogClass.WriteToLog(0,
                $"Exception at ServerOnSuccess: {ex.Message}",
                0);
            return -1;
        }
    }

    /// <summary>
    ///     Data file upload.
    /// </summary>
    /// <param name="ftpConnect"></param>
    /// <param name="sftpConnect"></param>
    /// <param name="customerId"></param>
    /// <param name="fileName"></param>
    /// <param name="downloadFolderPath"></param>
    /// <param name="ftpFileList"></param>
    /// <param name="localFileList"></param>
    /// <returns></returns>
    public static async Task<int> ServerOnSuccessDataFileAsync(AsyncFtpClient? ftpConnect,
        SftpClient? sftpConnect,
        Guid customerId,
        string fileName,
        string downloadFolderPath,
        string[] ftpFileList,
        string[] localFileList)
    {
        try
        {
            // User config details.
            var customerDetails = await UserConfigRetriever.RetrieveFtpConfigById(customerId);
            var ftpDetails = customerDetails.FtpDetails;

            if (ftpDetails is null)
            {
                WriteLogClass.WriteToLog(0, "FTP details not found ....", 1);
                return -1;
            }

            WriteLogClass.WriteToLog(1, $"Uploaded data file: {fileName}", 1);

            // Converts the filename to an array. Needed by the FolderCleanerClass.
            var jsonFileList = new[] { fileName };

            // Remove the files from FTP server.
            if (ftpDetails.FtpRemoveFiles
                && !await FolderCleanerClass.StartFtpFileDelete(ftpConnect,
                    sftpConnect,
                    ftpFileList,
                    localFileList))
                return -1;

            // Remove the files from the local folder.
            if (!await FolderCleanerClass.GetFolders(downloadFolderPath,
                    jsonFileList,
                    customerId,
                    null,
                    MagicWords.Ftp))
                return -1;
            return 1;
        }
        catch (Exception ex)
        {
            WriteLogClass.WriteToLog(0,
                $"Exception at ServerOnSuccessDataFileAsync: {ex.Message}",
                0);
            return -1;
        }
    }

    /// <summary>
    ///     Sending body text to TPS server success.
    /// </summary>
    public static async Task<int> ServerOnSuccessBodyTextAsync(IMailFolderRequestBuilder requestBuilder,
        string messageId,
        string messageSubject)
    {
        try
        {
            if (!await GraphMoveEmailsToExport.MoveEmailsToExport(requestBuilder,
                    messageId,
                    messageSubject))
            {
                WriteLogClass.WriteToLog(0,
                    $"Moving email {messageSubject} to export unsuccessful ....",
                    2);
                return 2;
            }

            WriteLogClass.WriteToLog(1,
                $"Body text sent to system. Moved email {messageSubject} to export successfully ....",
                2);
            return 1;
        }
        catch (Exception ex)
        {
            WriteLogClass.WriteToLog(0,
                $"Exception at ServerOnSuccessBodyTextAsync: {ex.Message}",
                0);
            return 2;
        }
    }
}