using System.Net;
using DEA.Next.Graph.GraphHelperClasses;
using DEA.Next.HelperClasses.ConfigFileFunctions;
using DEA.Next.HelperClasses.FolderFunctions;
using DEA.Next.HelperClasses.OtherFunctions;
using FluentFTP;
using GetMailFolderIds;
using GraphMoveEmailsrClass;
using HandleErrorFiles;
using Microsoft.Graph;
using Renci.SshNet;
using WriteLog;
using Directory = System.IO.Directory;

namespace DEA.Next.FileOperations.TpsServerResponseFunctions;

/// <summary>
///     Handles the operations after a failed TPS server response.
/// </summary>
internal class TpsServerOnFaile
{
    /// <summary>
    ///     Handles the normal project upload.
    /// </summary>
    /// <param name="deliveryType"></param>
    /// <param name="fullFilePath"></param>
    /// <param name="customerId"></param>
    /// <param name="clientOrgNo"></param>
    /// <param name="ftpConnect"></param>
    /// <param name="sftpConnect"></param>
    /// <param name="ftpFileList"></param>
    /// <param name="localFileList"></param>
    /// <param name="serverStatusCode"></param>
    /// <param name="serverResponseContent"></param>
    /// <returns></returns>
    public static async Task<int> ServerOnFailProjectsAsync(string deliveryType,
        string fullFilePath,
        Guid customerId,
        string clientOrgNo,
        AsyncFtpClient? ftpConnect,
        SftpClient? sftpConnect,
        string[] ftpFileList,
        string[] localFileList,
        HttpStatusCode serverStatusCode,
        string? serverResponseContent)
    {
        try
        {
            var deleteResult = 2;

            var customerDetailsDetails = await UserConfigRetriever.RetrieveFtpConfigById(customerId);
            var ftpDetails = customerDetailsDetails.FtpDetails;
            if (ftpDetails is null)
            {
                WriteLogClass.WriteToLog(0, "FTP details not found ....", 0);
                return -1;
            }

            WriteLogClass.WriteToLog(0,
                $"Server status code: {serverStatusCode}, Server Response Error: {serverResponseContent}",
                0);

            // Moves the files to the error folder. Assumes the files was not uploaded.
            if (ftpDetails.FtpMoveToSubFolder == false
                && !await HandleErrorFilesClass.MoveFilesToErrorFolder(fullFilePath,
                    ftpFileList,
                    customerId,
                    clientOrgNo))
            {
                WriteLogClass.WriteToLog(0, "Moving files to FTP sub folder failed ....", 3);
                return -1;
            }

            switch (deliveryType)
            {
                // This will run if it's not FTP.
                case MagicWords.Email when await FolderCleanerClass.GetFolders(fullFilePath,
                    ftpFileList,
                    null,
                    clientOrgNo,
                    MagicWords.Email):
                    return 0;

                // Delete the files from FTP server.
                case MagicWords.Ftp when ftpDetails is { FtpMoveToSubFolder: false, FtpRemoveFiles: true }
                                         && !await FolderCleanerClass.StartFtpFileDelete(ftpConnect,
                                             sftpConnect,
                                             ftpFileList,
                                             localFileList):
                    WriteLogClass.WriteToLog(0,
                        "Deleting files from FTP server failed ....",
                        1);

                    return 0;

                // Deleting the files from local.
                case MagicWords.Ftp when ftpDetails.FtpMoveToSubFolder
                                         && !FolderCleanerClass.DeleteFiles(
                                             Path.GetDirectoryName(fullFilePath) ?? string.Empty,
                                             ftpFileList):
                    WriteLogClass.WriteToLog(0, "Deleting files failed ....", 1);
                    return 0;

                // Checking the folder is empty or not.
                case MagicWords.Ftp:
                {
                    var fileList = Directory.EnumerateFiles(Path.GetDirectoryName(fullFilePath)
                                                            ?? string.Empty, "*", SearchOption.AllDirectories);
                    // Return if the folder is not empty.
                    if (fileList.Any()) return 0;

                    // Deleting the empty folders.
                    if (!FolderCleanerClass.DeleteEmptyFolders(Path.GetDirectoryName(fullFilePath) ?? string.Empty))
                    {
                        WriteLogClass.WriteToLog(0, "Deleting empty folders failed ....", 1);
                        return 0;
                    }

                    break;
                }
            }

            return ftpDetails.FtpMoveToSubFolder ? 6 : deleteResult;
        }
        catch (Exception ex)
        {
            WriteLogClass.WriteToLog(0, $"Exception at ServerOnFailProjectsAsync: {ex.Message}", 0);
            return -1;
        }
    }

    /// <summary>
    ///     Handles the data file upload.
    /// </summary>
    /// <param name="ftpConnect"></param>
    /// <param name="sftpConnect"></param>
    /// <param name="customerId"></param>
    /// <param name="downloadFilePath"></param>
    /// <param name="serverResponseContent"></param>
    /// <param name="ftpFileList"></param>
    /// <param name="localFileList"></param>
    /// <param name="statusCode"></param>
    /// <returns></returns>
    public static async Task<int> ServerOnFailDataFileAsync(AsyncFtpClient? ftpConnect,
        SftpClient? sftpConnect,
        Guid customerId,
        string downloadFilePath,
        string serverResponseContent,
        string[] ftpFileList,
        string[] localFileList,
        HttpStatusCode statusCode)
    {
        try
        {
            // User configs.
            var customerDetails = await UserConfigRetriever.RetrieveFtpConfigById(customerId);
            var ftpDetails = customerDetails.FtpDetails;
            if (ftpDetails is null)
            {
                WriteLogClass.WriteToLog(0, "FTP details not found ....", 0);
                return -1;
            }

            WriteLogClass.WriteToLog(0,
                $"Server status code: {statusCode}, Server Response Error: {serverResponseContent}",
                0);

            // Move the file to error folder. Assuming file was not uploaded.
            if (!await HandleErrorFilesClass.MoveFilesToErrorFolder(downloadFilePath,
                    ftpFileList,
                    customerId,
                    string.Empty))
            {
                WriteLogClass.WriteToLog(0,
                    "Moving files failed ....",
                    3);
                return -1;
            }

            // Remove the files from FTP server.
            if (ftpDetails.FtpRemoveFiles
                && !await FolderCleanerClass.StartFtpFileDelete(ftpConnect,
                    sftpConnect,
                    ftpFileList,
                    localFileList))
            {
                WriteLogClass.WriteToLog(0,
                    "Deleting files from FTP server failed ....",
                    3);
                return 0;
            }

            // Remove the files from the local folder.
            if (FolderCleanerClass.DeleteEmptyFolders(downloadFilePath)) return 2; // Default return

            WriteLogClass.WriteToLog(0,
                "Deleting empty folders failed ....",
                3);
            return 0;
        }
        catch (Exception ex)
        {
            WriteLogClass.WriteToLog(0,
                $"Exception at ServerOnFailDataFileAsync: {ex.Message}",
                0);
            return -1;
        }
    }

    /// <summary>
    ///     Handles the on fail for body text upload.
    /// </summary>
    public static async Task<bool> ServerOnFailBodyTextAsync(IMailFolderRequestBuilder requestBuilder,
        List<AttachmentFile> attachments,
        string messageId,
        string messageSubject,
        string? serverResponseContent,
        HttpStatusCode serverStatusCode)
    {
        var errorFolderId = await GetMailFolderIdsClass.GetErrorFolderId(requestBuilder);

        try
        {
            if (!await GraphMoveEmailsFolder.MoveEmailsToAnotherFolder(requestBuilder,
                    messageId,
                    errorFolderId))
            {
                WriteLogClass.WriteToLog(0,
                    "Moving email to error folder failed ....",
                    2);
                return false;
            }

            if (attachments.Count == 0)
            {
                WriteLogClass.WriteToLog(1,
                    $"Body text sent to system. No attachments found in email {messageSubject} ....",
                    2);
                return false;
            }

            if (!await FolderCleanerBodyText.DeleteDownloadedAttachments(attachments))
            {
                WriteLogClass.WriteToLog(0,
                    "Deleting attachments from email unsuccessful ....",
                    2);
                return false;
            }

            WriteLogClass.WriteToLog(0,
                $"Sending to server failed. Email moved to error folder." +
                $"\nCode:{serverStatusCode}\nStatus:{serverResponseContent}",
                2);
            return false;
        }
        catch (Exception ex)
        {
            WriteLogClass.WriteToLog(0,
                $"Exception at ServerOnFailBodyTextAsync: {ex.Message}",
                0);
            return false;
        }
    }
}