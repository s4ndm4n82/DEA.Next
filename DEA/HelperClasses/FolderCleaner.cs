using FluentFTP;
using WriteLog;

namespace FolderCleaner
{
    internal class FolderCleanerClass
    {
        public static bool GetFolders(string folderPath, string type)
        {
            DirectoryInfo filePath = Directory.GetParent(Path.GetDirectoryName(folderPath)!)!;

            if (type.ToLower() == "email")
            {
                filePath = Directory.GetParent(folderPath)!;
            }

            Console.WriteLine(filePath);
            Thread.Sleep(1000000);

            if (Directory.Exists(filePath!.FullName))
            {
                WriteLogClass.WriteToLog(3, "Cleaning download folder ....", 1);

                string[] folderList = Directory.GetDirectories(filePath.FullName, "*.*", SearchOption.AllDirectories);

                if (DeleteFolders(folderList))
                {
                    return true;
                }
            }
            else
            {
                WriteLogClass.WriteToLog(3, "Folder path does not exsits ....", 1);
            }
            return false;
        }

        private static bool DeleteFolders(string[] folderList)
        {
            int loopCount = 0;
            
            foreach (string folder in folderList)
            {
                loopCount++;

                if (Directory.Exists(folder))
                {
                    if (Directory.GetFiles(folder, "*.*", SearchOption.AllDirectories).Length > 0)
                    {
                        try
                        {   
                            string folderPath = Directory.GetParent(folder)!.ToString();
                            
                            Directory.Delete(folderPath, true);
                           
                        }
                        catch (IOException ex)
                        {
                            WriteLogClass.WriteToLog(2, $"Exception at folder delete: {ex.Message}", 1);
                        }
                    }
                }
            }

            if (loopCount == folderList.Length)
            {
                WriteLogClass.WriteToLog(3, $"Removed {folderList.Length} folder from download folder ....", 1);
                return true;
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
                WriteLogClass.WriteToLog(3, $"Deleted {localFileList.Length} from the FTP server ....", 3);
                return true;
            }
            return false;
        }

        public static async Task<bool> DeleteFtpFiles(AsyncFtpClient ftpConnect, string ftpFileName)
        {
            try
            {
                await ftpConnect.DeleteFile(ftpFileName);
                return true;
            }
            catch (Exception ex)
            {
                WriteLogClass.WriteToLog(2, $"Exception at FTP file deletetion: {ex.Message}, file name {ftpFileName}.", 3);
                return false;
            }

        }
    }
}
