using DEA.Next.HelperClasses.OtherFunctions;
using FluentFTP;
using HandleErrorFiles;
using Renci.SshNet;
using WriteLog;
using WriteNamesToLog;

namespace FolderCleaner
{
    internal class FolderCleanerClass
    {
        /// <summary>
        /// This function calls the folder cleaning function below.
        /// </summary>
        /// <param name="localDownloadFilePath"></param>
        /// <param name="jsonFileNames"></param>
        /// <param name="customerID"></param>
        /// <param name="clientEmail"></param>
        /// <returns></returns>
        public static async Task<bool> GetFolders(string downloadFilePath,
                                                  string[] jsonFileNames,
                                                  int? customerID,
                                                  string clientEmail,
                                                  string deliveryType)
        {
            bool result = false;
            string localDownloadFilePath = downloadFilePath;

            if (deliveryType == MagicWords.email)
            {
                localDownloadFilePath = Path.GetDirectoryName(downloadFilePath);
            }

            if (Directory.Exists(Path.GetDirectoryName(localDownloadFilePath)))
            {
                WriteLogClass.WriteToLog(1, "Cleaning download folder ....", 1);
                result = await FolderCleaningProcess(localDownloadFilePath, jsonFileNames, customerID, clientEmail, deliveryType);
            }
            return result;
        }

        /// <summary>
        /// This function starts the cleaning of local download folder. It checks for missed files and moves them to the error folder.
        /// After that it deletes the empty folders.
        /// </summary>
        /// <param name="downloadedFolderPath"></param>
        /// <param name="jsonFileList"></param>
        /// <param name="customerId"></param>
        /// <param name="clientEmail"></param>
        /// <returns></returns>
        private static async Task<bool> FolderCleaningProcess(string downloadedFolderPath,
                                                              string[] jsonFileList,
                                                              int? customerId,
                                                              string clientEmail,
                                                              string deliverType)
        {
            try
            {
                bool fileMoveResult = true; // Store the result of move files to error folder.

                if (deliverType == MagicWords.email)
                {
                    if (!AttachmentFileDelete(downloadedFolderPath, jsonFileList))
                    {
                        return false;
                    }
                }

                // File delete files will be written to the log. And return false.
                if (!DeleteFiles(downloadedFolderPath, jsonFileList))
                {
                    WriteLogClass.WriteToLog(0, "Deleting files failed ....", 1);
                    return false;
                }

                IEnumerable<string> nameList = CheckMissedFiles(downloadedFolderPath, jsonFileList);
                if (nameList.Any()) // If there are any unmatched files.
                {
                    // Calls the MoveFilesToErrorFolder method to start moving the missed files.
                    fileMoveResult = await HandleErrorFilesClass.MoveFilesToErrorFolder(downloadedFolderPath,
                                                                                  nameList,
                                                                                  customerId,
                                                                                  clientEmail);
                    // Writes the result to the log.
                    WriteLogClass.WriteToLog(1, fileMoveResult ? $"Moved files {WriteNamesToLogClass.WriteMissedFilenames(nameList)}"
                                                               : "Moving files was unsuccessful ...", 1);
                }

                // Checking if the folder is not empty.
                IEnumerable<string> fileList = Directory.EnumerateFiles(downloadedFolderPath, "*.*");
                // Return false if the folder is not empty.
                if (fileList.Any())
                {
                    return false;
                }

                // Folder delete failes will be written to the log. And return false.
                if (!DeleteEmptyFolders(downloadedFolderPath))
                {
                    WriteLogClass.WriteToLog(0, "Deletting empty folders failed ....", 1);
                    return false;
                }

                return fileMoveResult;
            }
            catch (Exception ex)
            {
                WriteLogClass.WriteToLog(0, $"Exception at folder cleaning process: {ex.Message}", 0);
                return false;
            }
        }

        /// <summary>
        /// Start the FTP file delete process.
        /// </summary>
        /// <param name="ftpConnect">Ftp conntection token.</param>
        /// <param name="ftpFileList">File list from the FTP server.</param>
        /// <param name="localFileList">File list from the local download folder.</param>
        /// <returns>The result of remove process or false.</returns>
        public static async Task<bool> StartFtpFileDelete(AsyncFtpClient? ftpConnect,
                                                          SftpClient? sftpConnect,
                                                          IEnumerable<string> ftpFileList,
                                                          string[] localFileList)
        {
            try
            {
                // Get the FTP path from the first FTP file.
                string ftpPath = GetFtpPath(ftpFileList.FirstOrDefault());
                // Local file name list.
                IEnumerable<string> localFileNames = localFileList.Select(localFilePath => Path.GetFileName(localFilePath));
                // FTP file name list.
                IEnumerable<string> ftpFileNames = ftpFileList.Select(ftpFilePath => Path.GetFileName(ftpFilePath));
                // Matching file names.
                IEnumerable<string> matchingFileNames = localFileNames.Intersect(ftpFileNames);
                // Result of the foreach loop.
                bool result = false;

                // If there are no matching files, return.
                if (!matchingFileNames.Any())
                {
                    return result;
                }

                if (ftpConnect != null)
                {
                    // Delete FTP files.
                    foreach (string matchingFileName in matchingFileNames)
                    {
                        result = await DeleteFtpFiles(ftpConnect, string.Concat(ftpPath, matchingFileName));
                    }
                }

                if (sftpConnect != null)
                {
                    // Delete SFTP files.
                    foreach (string matchingFileName in matchingFileNames)
                    {
                        result = await DeleteSftpFiles(sftpConnect, string.Concat(ftpPath, matchingFileName));
                    }
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
        /// Check for any missing files from the upload process.
        /// </summary>
        /// <param name="localFolderPath">Local download process path</param>
        /// <param name="remoteFileList">FTP files list.</param>
        /// <returns>Returns the unmatched file names list.</returns>
        private static IEnumerable<string> CheckMissedFiles(string localFolderPath,
                                                            IEnumerable<string> remoteFileList)
        {
            try
            {
                // Makes the downloaded files list from the folder path.
                IEnumerable<string> downloadedFileList = Directory.EnumerateFiles(localFolderPath, "*.*");
                // Creates file name only list from the json file list.
                IEnumerable<string> jsonFileNames = remoteFileList.Select(jsonFilePath => Path.GetFileName(jsonFilePath));
                // Creates file name only list from the downloaded file list.
                IEnumerable<string> downloadedFileNames = downloadedFileList.Select(downloadedFilePath => Path.GetFileName(downloadedFilePath));
                // Gets the unmatched file names. From matching the above two lists.
                //IEnumerable<string> unmatchedFileNames = jsonFileNames.Except(downloadedFileNames).Concat(downloadedFileNames.Except(jsonFileNames));
                IEnumerable<string> matchedFileNames = downloadedFileNames.Intersect(jsonFileNames);

                WriteLogClass.WriteToLog(1, !matchedFileNames.Any() ? "No missed files found ...."
                                                                      : $"Found {matchedFileNames.Count()} missed files ....", 1);
                return matchedFileNames;
            }
            catch (Exception ex)
            {
                WriteLogClass.WriteToLog(0, $"Exception at check missed files: {ex.Message}", 0);
                return Enumerable.Empty<string>();
            }
        }

        /// <summary>
        /// Deletes files from the FTP server.
        /// </summary>
        /// <param name="ftpConnect">FTP connection token.</param>
        /// <param name="ftpFileName">FTP files list.</param>
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
                WriteLogClass.WriteToLog(0, $"Exception at FTP file deletetion: {ex.Message}, file name {fileToDelete}.", 0);
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
                WriteLogClass.WriteToLog(0, $"Exception at SFTP file deletetion: {ex.Message}, file name {fileToDelete}.", 0);
                return false;
            }
        }

        /// <summary>
        /// Get the FTP path from the first FTP file.
        /// </summary>
        /// <param name="ftpFilePath">FTP file path with the file name.</param>
        /// <returns>Directory path</returns>
        private static string GetFtpPath(string ftpFilePath)
        {
            int lastSlashIndex = ftpFilePath.Replace("Downloaded:  ", "").LastIndexOf('/');
            string directoryPath = string.Concat(ftpFilePath[..lastSlashIndex], '/');

            return directoryPath;
        }

        /// <summary>
        /// Delete all files from the download folder.
        /// </summary>
        /// <param name="downloadFolderPath">Path of the local download folder.</param>
        /// <returns>Returns true or false.</returns>
        public static bool DeleteFiles(string downloadFolderPath,
                                        string[] jsonFileList)
        {
            // Returns if the loacal directory is missing without executing the remove code.
            if (!Directory.Exists(downloadFolderPath))
            {
                return false;
            }

            // Makes the downloaded files list from the folder path.
            IEnumerable<string> downloadedFileNamesList = Directory.EnumerateFiles(downloadFolderPath, "*.*");
            // Creates file name only list from the downloaded file list.
            IEnumerable<string> downloadedFileNames = downloadedFileNamesList.Select(downloadedFilePath => Path.GetFileName(downloadedFilePath));
            // Checking if the jsonFileList contains file names or file names with path.
            bool containsPath = jsonFileList.Any(filePath => filePath.Contains('/') || filePath.Contains('\\'));
            // Creates file name only list from the json file list.
            IEnumerable<string> jsonFileNames = containsPath ? jsonFileList.Select(jsonFilePath => Path.GetFileName(jsonFilePath)) : jsonFileList;
            // Makes the matching file names list.
            //IEnumerable<string> matchingFileNames = jsonFileList.Intersect(jsonFileList); <-- remove this if not needed. Do this next release
            IEnumerable<string> matchingFileNames = jsonFileNames.Intersect(downloadedFileNames).Where(fileName => downloadedFileNames.Contains(fileName));
            // Result of the foreach loop.
            bool result = true;

            // Delete files.
            foreach (string fileName in matchingFileNames)
            {
                try
                {
                    File.Delete(Path.Combine(downloadFolderPath, fileName));
                }
                catch (Exception ex)
                {
                    WriteLogClass.WriteToLog(0, $"Exception at file delete: {ex.Message}", 0);
                    result = false;
                }
            }

            return result;
        }

        /// <summary>
        /// Delete all the empty folders from the download folder.
        /// </summary>
        /// <param name="downloadFolderPath"></param>
        /// <returns></returns>
        public static bool DeleteEmptyFolders(string downloadFolderPath)
        {
            try
            {
                bool result = true;
                // Get the download folder name.
                string basePath = Path.GetDirectoryName(downloadFolderPath);
                // List all the folder with in the local download folder.
                IEnumerable<string> directoryList = Directory.EnumerateDirectories(basePath, "*", SearchOption.AllDirectories);
                // List all the empty folders.
                IEnumerable<string> emptyFolderList = directoryList.Where(dirPath => !Directory.EnumerateFileSystemEntries(dirPath).Any());

                if (!emptyFolderList.Any())
                {
                    WriteLogClass.WriteToLog(1, "No empty folders found ....", 1);
                    return false;
                }

                foreach (string emptyFolder in emptyFolderList)
                {
                    try
                    {
                        // Check if the folder is not empty.
                        IEnumerable<string> fileList = Directory.EnumerateFiles(emptyFolder, "*.*");
                        if (fileList.Any())
                        {
                            WriteLogClass.WriteToLog(1, "The download folder is not empty ....", 1);
                            continue;
                        }

                        // Delete the folders if empty.
                        Directory.Delete(emptyFolder);
                        WriteLogClass.WriteToLog(1, $"Local download folder {Path.GetFileName(emptyFolder)} deleted ....", 1);
                    }
                    catch (Exception ex)
                    {
                        WriteLogClass.WriteToLog(0, $"Exception at DeleteEmptyFolders foreach: {ex.Message}", 0);
                        result = false;
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                WriteLogClass.WriteToLog(0, $"Exception at DeleteEmptyFolders: {ex.Message}", 0);
                return false;
            }
        }

        private static bool AttachmentFileDelete(string downloadFolerPath,
                                                 string[] jsonFileList)
        {
            bool result = true;
            try
            {
                foreach (string jsonFile in jsonFileList)
                {
                    string fullFileName = Path.Combine(downloadFolerPath, jsonFile);
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
}
