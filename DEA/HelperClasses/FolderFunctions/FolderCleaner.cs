using DEA.Next.HelperClasses.OtherFunctions;
using FluentFTP;
using HandleErrorFiles;
using Renci.SshNet;
using WriteLog;
using WriteNamesToLog;

namespace DEA.Next.HelperClasses.FolderFunctions;

internal class FolderCleanerClass
{
    /// <summary>
    ///     This function calls the folder cleaning function below.
    /// </summary>
    /// <param name="downloadFilePath"></param>
    /// <param name="jsonFileNames"></param>
    /// <param name="customerId"></param>
    /// <param name="clientEmail"></param>
    /// <param name="deliveryType"></param>
    /// <returns></returns>
    public static async Task<bool> GetFolders(string downloadFilePath,
        string[] jsonFileNames,
        Guid? customerId,
        string? clientEmail,
        string deliveryType)
    {
        var result = false;
        var localDownloadFilePath = downloadFilePath;

        if (deliveryType == MagicWords.Email) localDownloadFilePath = Path.GetDirectoryName(downloadFilePath);

        if (!Directory.Exists(Path.GetDirectoryName(localDownloadFilePath))) return result;
        WriteLogClass.WriteToLog(1, "Cleaning download folder ....", 1);
        if (localDownloadFilePath != null)
            result = await FolderCleaningProcess(localDownloadFilePath, jsonFileNames, customerId, clientEmail,
                deliveryType);
        return result;
    }

    /// <summary>
    ///     This function starts the cleaning of local download folder. It checks for missed files and moves them to the error
    ///     folder.
    ///     After that it deletes the empty folders.
    /// </summary>
    /// <param name="downloadedFolderPath"></param>
    /// <param name="jsonFileList"></param>
    /// <param name="customerId"></param>
    /// <param name="clientEmail"></param>
    /// <param name="deliverType"></param>
    /// <returns></returns>
    private static async Task<bool> FolderCleaningProcess(string downloadedFolderPath,
        string[] jsonFileList,
        Guid? customerId,
        string clientEmail,
        string deliverType)
    {
        try
        {
            var fileMoveResult = true; // Store the result of move files to error folder.

            if (deliverType == MagicWords.Email)
                if (!AttachmentFileDelete(downloadedFolderPath, jsonFileList))
                    return false;

            // File delete files will be written to the log. And return false.
            if (!DeleteFiles(downloadedFolderPath, jsonFileList))
            {
                WriteLogClass.WriteToLog(0, "Deleting files failed ....", 1);
                return false;
            }

            var nameList = CheckMissedFiles(downloadedFolderPath, jsonFileList).ToList();

            if (nameList.Count != 0) // If there are any unmatched files.
            {
                // Calls the MoveFilesToErrorFolder method to start moving the missed files.
                fileMoveResult = await HandleErrorFilesClass.MoveFilesToErrorFolder(downloadedFolderPath,
                    nameList,
                    customerId,
                    clientEmail);

                // Writes the result to the log.
                WriteLogClass.WriteToLog(1, fileMoveResult
                    ? $"Moved files {WriteNamesToLogClass.WriteMissedFilenames(nameList)}"
                    : "Moving files was unsuccessful ...", 1);
            }

            // Checking if the folder is not empty.
            var fileList = Directory.EnumerateFiles(downloadedFolderPath, "*.*").ToList();

            // Return false if the folder is not empty.
            if (fileList.Count != 0) return false;

            // Folder delete files will be written to the log. And return false.
            if (DeleteEmptyFolders(downloadedFolderPath)) return fileMoveResult;

            WriteLogClass.WriteToLog(0, "Deleting empty folders failed ....", 1);
            return false;
        }
        catch (Exception ex)
        {
            WriteLogClass.WriteToLog(0, $"Exception at folder cleaning process: {ex.Message}", 0);
            return false;
        }
    }

    /// <summary>
    ///     Start the FTP file delete process.
    /// </summary>
    /// <param name="ftpConnect">Ftp connection token.</param>
    /// <param name="sftpConnect"></param>
    /// <param name="ftpFileList">File list from the FTP server.</param>
    /// <param name="localFileList">File list from the local download folder.</param>
    /// <returns>The result of remove process or false.</returns>
    public static async Task<bool> StartFtpFileDelete(AsyncFtpClient? ftpConnect,
        SftpClient? sftpConnect,
        string[] ftpFileList,
        string[] localFileList)
    {
        try
        {
            // Get the FTP path from the first FTP file.
            var ftpPath = GetFtpPath(ftpFileList.FirstOrDefault() ?? string.Empty);
            // Local file name list.
            var localFileNames = localFileList.Select(Path.GetFileName);
            // FTP file name list.
            var ftpFileNames = ftpFileList.Select(Path.GetFileName);
            // Matching file names.
            var matchingFileNames = localFileNames.Intersect(ftpFileNames).ToList();
            // Result of the foreach loop.
            var result = false;

            // If there are no matching files, return.
            if (matchingFileNames.Count == 0) return result;

            if (ftpConnect != null)
                // Delete FTP files.
                foreach (var matchingFileName in matchingFileNames)
                {
                    result = await DeleteFtpFiles(ftpConnect, string.Concat(ftpPath, matchingFileName));
                    if (!result) break;
                }
            else if (sftpConnect != null)
                // Delete SFTP files.
                foreach (var matchingFileName in matchingFileNames)
                {
                    result = await DeleteSftpFiles(sftpConnect, string.Concat(ftpPath, matchingFileName));
                    if (!result) break;
                }

            return result;
        }
        catch (Exception ex)
        {
            WriteLogClass.WriteToLog(0, $"Exception at Start FTP file delete: {ex.Message}", 0);
            return false;
        }
    }

    /// <summary>
    ///     Check for any missing files from the upload process.
    /// </summary>
    /// <param name="localFolderPath">Local download process path</param>
    /// <param name="remoteFileList">FTP files list.</param>
    /// <returns>Returns the unmatched file names list.</returns>
    private static List<string> CheckMissedFiles(string localFolderPath,
        IEnumerable<string> remoteFileList)
    {
        try
        {
            // Makes the downloaded files list from the folder path.
            var downloadedFileList = Directory.EnumerateFiles(localFolderPath, "*.*");
            // Creates file name only list from the json file list.
            var jsonFileNames = remoteFileList.Select(jsonFilePath => Path.GetFileName(jsonFilePath));
            // Creates file name only list from the downloaded file list.
            var downloadedFileNames =
                downloadedFileList.Select(downloadedFilePath => Path.GetFileName(downloadedFilePath));
            // Gets the unmatched file names. From matching the above two lists.
            //IEnumerable<string> unmatchedFileNames = jsonFileNames.Except(downloadedFileNames).Concat(downloadedFileNames.Except(jsonFileNames));
            var matchedFileNames = downloadedFileNames.Intersect(jsonFileNames).ToList();

            WriteLogClass.WriteToLog(1, matchedFileNames.Count == 0
                ? "No missed files found ...."
                : $"Found {matchedFileNames.Count} missed files ....", 1);
            return matchedFileNames;
        }
        catch (Exception ex)
        {
            WriteLogClass.WriteToLog(0, $"Exception at check missed files: {ex.Message}", 0);
            return [];
        }
    }

    /// <summary>
    ///     Deletes files from the FTP server.
    /// </summary>
    /// <param name="ftpConnect">FTP connection token.</param>
    /// <param name="fileToDelete"></param>
    /// <returns>Return true or false.</returns>
    private static async Task<bool> DeleteFtpFiles(AsyncFtpClient ftpConnect,
        string fileToDelete)
    {
        try
        {
            // Delete the FTP file.
            await ftpConnect.DeleteFile(fileToDelete);
            return true;
        }
        catch (Exception ex)
        {
            WriteLogClass.WriteToLog(0, $"Exception at FTP file deletetion: {ex.Message}, file name {fileToDelete}.",
                0);
            return false;
        }
    }

    private static async Task<bool> DeleteSftpFiles(SftpClient sftpConnect,
        string fileToDelete)
    {
        try
        {
            // Delete the SFTP file.
            await sftpConnect.DeleteFileAsync(fileToDelete, CancellationToken.None);
            return true;
        }
        catch (Exception ex)
        {
            WriteLogClass.WriteToLog(0, $"Exception at SFTP file deletetion: {ex.Message}, file name {fileToDelete}.",
                0);
            return false;
        }
    }

    /// <summary>
    ///     Get the FTP path from the first FTP file.
    /// </summary>
    /// <param name="ftpFilePath">FTP file path with the file name.</param>
    /// <returns>Directory path</returns>
    private static string GetFtpPath(string ftpFilePath)
    {
        var lastSlashIndex = ftpFilePath.Replace("Downloaded:  ", "").LastIndexOf('/');
        var directoryPath = string.Concat(ftpFilePath[..lastSlashIndex], '/');

        return directoryPath;
    }

    /// <summary>
    ///     Delete all files from the download folder.
    /// </summary>
    /// <param name="downloadFolderPath">Path of the local download folder.</param>
    /// <param name="jsonFileList"></param>
    /// <returns>Returns true or false.</returns>
    public static bool DeleteFiles(string downloadFolderPath,
        string[] jsonFileList)
    {
        // Returns if the loacal directory is missing without executing the remove code.
        if (!Directory.Exists(downloadFolderPath)) return false;

        // Makes the downloaded files list from the folder path.
        var downloadedFileNamesList = Directory.EnumerateFiles(downloadFolderPath, "*.*");
        // Creates file name only list from the downloaded file list.
        var downloadedFileNames = downloadedFileNamesList.Select(Path.GetFileName).ToList();
        // Checking if the jsonFileList contains file names or file names with path.
        var containsPath = jsonFileList.Any(filePath => filePath.Contains('/') || filePath.Contains('\\'));
        // Creates file name only list from the json file list.
        var jsonFileNames = containsPath
            ? jsonFileList.Select(jsonFilePath => Path.GetFileName(jsonFilePath))
            : jsonFileList;
        // Makes the matching file names list.
        //IEnumerable<string> matchingFileNames = jsonFileList.Intersect(jsonFileList); <-- remove this if not needed. Do this next release
        var matchingFileNames = jsonFileNames.Intersect(downloadedFileNames)
            .Where(fileName => downloadedFileNames.Contains(fileName));
        // Result of the foreach loop.
        var result = true;

        // Delete files.
        foreach (var fileName in matchingFileNames)
            try
            {
                if (fileName != null) File.Delete(Path.Combine(downloadFolderPath, fileName));
            }
            catch (Exception ex)
            {
                WriteLogClass.WriteToLog(0, $"Exception at file delete: {ex.Message}", 0);
                result = false;
            }

        return result;
    }

    /// <summary>
    ///     Delete all the empty folders from the download folder.
    /// </summary>
    /// <param name="downloadFolderPath"></param>
    /// <returns></returns>
    public static bool DeleteEmptyFolders(string downloadFolderPath)
    {
        try
        {
            var result = true;
            // Get the download folder name.
            var basePath = Path.GetDirectoryName(downloadFolderPath);
            if (basePath == null) return result;

            // List all the folder with in the local download folder.
            var directoryList = Directory.EnumerateDirectories(basePath, "*", SearchOption.AllDirectories);
            // List all the empty folders.
            var emptyFolderList = directoryList.Where(dirPath => !Directory.EnumerateFileSystemEntries(dirPath).Any())
                .ToList();

            if (emptyFolderList.Count == 0)
            {
                WriteLogClass.WriteToLog(1, "No empty folders found ....", 1);
                return false;
            }

            foreach (var emptyFolder in emptyFolderList)
                try
                {
                    // Check if the folder is not empty.
                    var fileList = Directory.EnumerateFiles(emptyFolder, "*.*");
                    if (fileList.Any())
                    {
                        WriteLogClass.WriteToLog(1, "The download folder is not empty ....", 1);
                        continue;
                    }

                    // Delete the folders if empty.
                    Directory.Delete(emptyFolder);
                    WriteLogClass.WriteToLog(1, $"Local download folder {Path.GetFileName(emptyFolder)} deleted ....",
                        1);
                }
                catch (Exception ex)
                {
                    WriteLogClass.WriteToLog(0, $"Exception at DeleteEmptyFolders foreach: {ex.Message}", 0);
                    result = false;
                }

            return result;
        }
        catch (Exception ex)
        {
            WriteLogClass.WriteToLog(0, $"Exception at DeleteEmptyFolders: {ex.Message}", 0);
            return false;
        }
    }

    private static bool AttachmentFileDelete(string downloadFolderPath,
        string[] jsonFileList)
    {
        var result = true;
        try
        {
            foreach (var jsonFile in jsonFileList)
            {
                var fullFileName = Path.Combine(downloadFolderPath, jsonFile);
                File.Delete(fullFileName);
            }
        }
        catch (Exception ex)
        {
            WriteLogClass.WriteToLog(0, $"Exception at attachment file delete: {ex.Message}", 0);
            result = false;
        }

        return result;
    }
}