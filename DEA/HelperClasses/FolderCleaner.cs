using FluentFTP;
using WriteLog;

namespace FolderCleaner
{
    internal class FolderCleanerClass
    {
        public static bool GetFolders(string folderPath, string type)
        {
            DirectoryInfo filePath;

            if (type.ToLower() == "email")
            {
                filePath = Directory.GetParent(Path.GetDirectoryName(folderPath)!)!;
            }
            else
            {
                filePath = Directory.GetParent(Directory.GetParent(Path.GetDirectoryName(folderPath)!)!.FullName)!;
            }

            if (Directory.Exists(filePath!.FullName))
            {
                WriteLogClass.WriteToLog(1, "Cleaning download folder ....", 1);
                //string[] folderList = Directory.GetDirectories(filePath.FullName, "*.*", SearchOption.TopDirectoryOnly);

                DirectoryInfo dirPath = new(filePath!.FullName);
                IEnumerable<DirectoryInfo> folderList = dirPath.EnumerateDirectories("*.*", SearchOption.TopDirectoryOnly);

                if (DeleteFolders(folderList))
                {
                    return true;
                }
            }
            else
            {
                WriteLogClass.WriteToLog(1, "Folder path does not exsits ....", 1);
                return false;
            }
            return false;
        }

        private static bool DeleteFolders(IEnumerable<DirectoryInfo> folderList)
        {
            int fileLoopCount = 0;
            
            foreach (DirectoryInfo folder in folderList)
            {
                if (Directory.Exists(folder.FullName))
                {
                    IEnumerable<FileInfo> fileNames = folder.EnumerateFiles("*.*", SearchOption.AllDirectories);

                    if (fileNames.Any())
                    {
                        try
                        {
                            // TODO 2 :Check this code.
                            foreach (FileInfo fileName in fileNames)
                            {
                                fileName.Delete();
                                fileLoopCount++;
                            }

                            if (fileLoopCount == fileNames.Count())
                            {
                                folder.Delete();
                            }
                            /*string folderPath = Directory.GetParent(folder.FullName)!.ToString();
                            Directory.Delete(folderPath, true);*/
                            
                        }
                        catch (IOException ex)
                        {
                            WriteLogClass.WriteToLog(0, $"Exception at folder delete: {ex.Message}", 0);
                        }
                    }
                }
            }

            if (fileLoopCount == folderList.Count())
            {
                WriteLogClass.WriteToLog(1, $"Removed {folderList.Count()} folder from download folder ....", 1);
                return true;
            }
            else
            {
                WriteLogClass.WriteToLog(1, $"Folder not removed. It's empty ....", 1);
            }

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
        
    }
}
