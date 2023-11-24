using FluentFTP;
using HandleErrorFiles;
using WriteLog;
using WriteNamesToLog;

namespace FolderCleaner
{
    internal class FolderCleanerClass
    {
        public static bool GetFolders(string downloadfolderPath, IEnumerable<string> jsonFileNames, int? customerID, string clientEmail)
        {
            if (Directory.Exists(downloadfolderPath))
            {
                WriteLogClass.WriteToLog(1, "Cleaning download folder ....", 1);
                return DeleteFolders(downloadfolderPath, jsonFileNames, customerID, clientEmail) ? true : false;
            }
            /*DirectoryInfo filePath = Directory.GetParent(Directory.GetParent(Path.GetDirectoryName(folderPath)!)!.FullName)!;

            if (Directory.Exists(filePath!.FullName))
            {
                WriteLogClass.WriteToLog(1, "Cleaning download folder ....", 1);

                DirectoryInfo dirPath = new(filePath!.FullName);
                IEnumerable<DirectoryInfo> folderList = dirPath.EnumerateDirectories("*.*", SearchOption.TopDirectoryOnly);

                if (DeleteFolders(folderList, jsonFileNames))
                {
                    return true;
                }
            }
            else
            {
                WriteLogClass.WriteToLog(1, "Folder path does not exsits ....", 1);
                return false;
            }*/
            return false;
        }

        private static bool DeleteFolders(string downloadedFolderPath, IEnumerable<string> jsonFileList, int? customerId, string clientEmail)
        {
            /*int fileLoopCount = 0;
            int folderLoopCount = 0;
            int folderCount = folderList.Count();*/            

            CheckMissedFiles(downloadedFolderPath, jsonFileList, customerId, clientEmail);

            /*foreach (DirectoryInfo folder in folderList)
            {
                if (Directory.Exists(folder.FullName))
                {
                    IEnumerable<FileInfo> fileNames = folder.EnumerateFiles("*.*", SearchOption.AllDirectories);
                    int fileCount = fileNames.Count();

                    if (fileNames.Any())
                    {
                        try
                        {
                            foreach (FileInfo fileName in fileNames)
                            {
                                fileName.Delete();
                                fileLoopCount++;
                            }
                            if (fileLoopCount == fileCount)
                            {
                                folder.Delete(true);
                            }
                        }
                        catch (IOException ex)
                        {
                            WriteLogClass.WriteToLog(0, $"Exception at folder and file delete: {ex.Message}", 0);
                        }
                    }
                    else if (!fileNames.Any())
                    {
                        try
                        {
                            string folderPath = Directory.GetParent(folder.FullName)!.ToString();
                            Directory.Delete(folderPath, true);                            
                        }
                        catch (Exception ex)
                        {
                            WriteLogClass.WriteToLog(0, $"Exception at folder delete: {ex.Message}", 0);
                        }
                    }
                    folderLoopCount++;
                }
            }

            if (folderLoopCount == folderCount)
            {
                WriteLogClass.WriteToLog(1, $"Removed {folderCount} folder from download folder ....", 1);
                return true;
            }
            else
            {
                WriteLogClass.WriteToLog(1, $"Folder not removed. It's empty ....", 1);
            }*/

            return false;
        }

        public static async Task<bool> GetFtpPathAsync(AsyncFtpClient ftpConnect, IEnumerable<string> ftpFileList, string[] localFileList)
        {
            int loopCount = 0;
            
            foreach (string localFilePath in localFileList)
            {
                string localFileName = Path.GetFileNameWithoutExtension(localFilePath);

                foreach (string ftpFilePath in ftpFileList)
                {
                    string ftpFileName = Path.GetFileNameWithoutExtension(ftpFilePath);

                    if (localFileName.Equals(ftpFileName, StringComparison.OrdinalIgnoreCase))
                    {
                        loopCount++;
                        await DeleteFtpFiles(ftpConnect, string.Concat(ftpFilePath));
                    }
                }
            }

            if (loopCount == localFileList.Length)
            {
                WriteLogClass.WriteToLog(1, $"Deleted {ftpFileList.Count()} from the FTP server ....", 3);
                return true;
            }
            return false;
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

        private static bool CheckMissedFiles(string downloadedFolderPath, IEnumerable<string> jsonFileList, int? customerId, string clienEmail)
        {
            IEnumerable<string> downloadedFileList = Directory.EnumerateFiles(downloadedFolderPath, "*.*");

            IEnumerable<string> jsonFileNames = jsonFileList.Select(jsonFilePath => Path.GetFileName(jsonFilePath));
            IEnumerable<string> downloadedFileNames = downloadedFileList.Select(downloadedFilePath => Path.GetFileName(downloadedFilePath));
            IEnumerable<string> unmatchedFileNames = jsonFileNames.Except(downloadedFileNames).Concat(downloadedFileNames.Except(jsonFileNames));

            if (unmatchedFileNames.Any())
            {
                WriteLogClass.WriteToLog(1, $"Missed {unmatchedFileNames.Count()} files ....", 1);
                bool fileMoveResult = HandleErrorFilesClass.MoveFilesToErrorFolder(downloadedFolderPath, unmatchedFileNames, customerId, clienEmail);

                WriteLogClass.WriteToLog(1, fileMoveResult ? $"Moved files {WriteNamesToLogClass.WriteMissedFilenames(unmatchedFileNames)}"
                                                           : "Failed to move files to error folder.", 1);
                return fileMoveResult;
            }
            return false;
        }
    }
}
