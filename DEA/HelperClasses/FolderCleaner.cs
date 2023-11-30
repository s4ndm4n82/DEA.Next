using FluentFTP;
using HandleErrorFiles;
using System.Linq;
using WriteLog;
using WriteNamesToLog;

namespace FolderCleaner
{
    internal class FolderCleanerClass
    {
        /// <summary>
        /// This function calls the folder cleaning function below.
        /// </summary>
        /// <param name="downloadfolderPath"></param>
        /// <param name="jsonFileNames"></param>
        /// <param name="customerID"></param>
        /// <param name="clientEmail"></param>
        /// <returns></returns>
        public static bool GetFolders(string downloadfolderPath, IEnumerable<string> jsonFileNames, int? customerID, string clientEmail)
        {
            bool result = false;

            if (Directory.Exists(downloadfolderPath))
            {
                WriteLogClass.WriteToLog(1, "Cleaning download folder ....", 1);
                result = FolderCleaningProcess(downloadfolderPath, jsonFileNames, customerID, clientEmail);
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
        private static bool FolderCleaningProcess(string downloadedFolderPath, IEnumerable<string> jsonFileList, int? customerId, string clientEmail)
        {
            IEnumerable<string> nameList = CheckMissedFiles(downloadedFolderPath, jsonFileList);

            if (nameList.Any()) // If there are any unmatched files.
            {
                // Calls the MoveFilesToErrorFolder method to start moving the missed files.
                bool fileMoveResult = HandleErrorFilesClass.MoveFilesToErrorFolder(downloadedFolderPath, nameList, customerId, clientEmail);
                // Writes the result to the log.
                WriteLogClass.WriteToLog(1, fileMoveResult ? $"Moved files {WriteNamesToLogClass.WriteMissedFilenames(nameList)}"
                                                           : "Moving files was unsuccessful ...", 1);
                // Returns the result.
                return fileMoveResult;
            }

            if (!DeleteFiles(downloadedFolderPath))
            {
                WriteLogClass.WriteToLog(0, "Delete files failed ....", 0);
                return false;
            }

            if (!DeleteEmptyFolders(downloadedFolderPath))
            {
                WriteLogClass.WriteToLog(0, "Delete empty folders failed ....", 0);
                return false;
            }

            return true;
        }

        public static async Task<bool> GetFtpPathAsync(AsyncFtpClient ftpConnect, IEnumerable<string> ftpFileList, string[] localFileList)
        {
            // Local file name list.
            IEnumerable<string> localFileNames = localFileList.Select(localFilePath => Path.GetFileName(localFilePath));
            // FTP file name list.
            IEnumerable<string> ftpFileNames = ftpFileList.Select(ftpFilePath => Path.GetFileName(ftpFilePath));
            // Matching file names.
            IEnumerable<string> matchingFileNames = localFileNames.Intersect(ftpFileNames);
            // Get the FTP path from the first FTP file.
            string ftpPath = GetFtpPath(ftpFileList.FirstOrDefault());
            // Result.
            bool result = false;

            // If there are no matching files, return.
            if (!matchingFileNames.Any())
            {
                return result;
            }

            // Delete FTP files.
            foreach (string matchingFileName in matchingFileNames)
            {
                result = await DeleteFtpFiles(ftpConnect, string.Concat(ftpPath, matchingFileName));
            }
            return result;
        }

        public static IEnumerable<string> CheckMissedFiles(string localFolderPath, IEnumerable<string> remoteFileList)
        {
            // Makes the downloaded files list from the folder path.
            IEnumerable<string> downloadedFileList = Directory.EnumerateFiles(localFolderPath, "*.*");

            // Creates file name only list from the json file list.
            IEnumerable<string> jsonFileNames = remoteFileList.Select(jsonFilePath => Path.GetFileName(jsonFilePath));
            // Creates file name only list from the downloaded file list.
            IEnumerable<string> downloadedFileNames = downloadedFileList.Select(downloadedFilePath => Path.GetFileName(downloadedFilePath));
            // Gets the unmatched file names. From matching the above two lists.
            IEnumerable<string> unmatchedFileNames = jsonFileNames.Except(downloadedFileNames).Concat(downloadedFileNames.Except(jsonFileNames));

            WriteLogClass.WriteToLog(1, !unmatchedFileNames.Any() ? "No missed files found ...."
                                                                  : $"Found {unmatchedFileNames.Count()} missed files ....", 1);
            return unmatchedFileNames;
        }

        private static async Task<bool> DeleteFtpFiles(AsyncFtpClient ftpConnect, string ftpFileName)
        {
            try
            {
                await ftpConnect.DeleteFile(ftpFileName);
                return true;
            }
            catch (Exception ex)
            {
                WriteLogClass.WriteToLog(0, $"Exception at FTP file deletetion: {ex.Message}, file name {ftpFileName}.", 0);
                return false;
            }

        }

        private static string GetFtpPath(string ftpFilePath)
        {
            int lastSlashIndex = ftpFilePath.LastIndexOf('/');            
            string directoryPath = string.Concat(ftpFilePath.Substring(0, lastSlashIndex), '/');

            return directoryPath;
        }

        private static bool DeleteFiles(string downloadFolderPath)
        {
            if (!Directory.Exists(downloadFolderPath))
            {
                return false;
            }

            IEnumerable<string> downloadedFileNames = Directory.EnumerateFiles(downloadFolderPath, "*.*");
            bool result = true;

            foreach (string fileName in downloadedFileNames)
            {
                try
                {
                    File.Delete(fileName);
                }
                catch (Exception ex)
                {
                    WriteLogClass.WriteToLog(0, $"Exception at file delete: {ex.Message}", 0);
                    result = false;
                }
            }

            return result;
        }

        private static bool DeleteEmptyFolders(string downloadFolderPath)
        {
            string fullPath = Path.GetFullPath(downloadFolderPath);
            string basePath = Path.GetDirectoryName(fullPath);
            
            IEnumerable<string> directoryList = Directory.EnumerateDirectories(basePath, "*", SearchOption.AllDirectories);

            IEnumerable<string> emptyFolderList = directoryList.Where(dirPath => !Directory.EnumerateFileSystemEntries(dirPath).Any());
            bool result = true;

            foreach (string emptyFolder in emptyFolderList)
            {
                try
                {
                    Directory.Delete(emptyFolder);
                }
                catch (Exception ex)
                {
                    WriteLogClass.WriteToLog(0, $"Exception at folder delete: {ex.Message}", 0);
                    result = false;
                }
            }
                return result;
        }
    }
}
