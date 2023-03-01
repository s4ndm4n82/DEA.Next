using FolderFunctions;
using WriteLog;

namespace HandleErrorFiles
{
    internal class HandleErrorFilesClass
    {
        public static bool MoveFilesToErrorFolder(string sourceFolderPath, int clientID)
        {
            string clientNo = clientID.ToString();
            string errorFolderPath = FolderFunctionsClass.CheckFolders("error");
            string sourceFolder = sourceFolderPath.Split(Path.DirectorySeparatorChar).Last();
            string destinationFolderPath = Path.Combine(errorFolderPath, clientNo, sourceFolder);
            IEnumerable<string> sourceFileNameList = Directory.EnumerateFiles(sourceFolderPath, "*.*", SearchOption.TopDirectoryOnly);

            if (!Directory.Exists(destinationFolderPath))
            {
                Directory.CreateDirectory(destinationFolderPath);
            }

            if (FileMover(destinationFolderPath, sourceFileNameList))
            {
                if (DeleteSrcFolder(sourceFolderPath))
                {
                    WriteLogClass.WriteToLog(1, $"Moved {sourceFileNameList.Count()} to {destinationFolderPath} ....", 1);
                    return true;
                }
            }
            return false;
        }

        private static bool FileMover(string dstPath, IEnumerable<string> srcFileList)
        {
            try
            {
                foreach (string srcFile in srcFileList)
                {
                    string fileName = Path.GetFileName(srcFile);
                    string dstFile = Path.Combine(dstPath, fileName);

                    if (File.Exists(dstFile))
                    {
                        WriteLogClass.WriteToLog(1, "File already exists ....", 1);
                        continue;
                    }
                    else
                    {
                        File.Move(srcFile, dstFile);
                    }                    
                }
                return true;
            }
            catch (Exception ex)
            {
                WriteLogClass.WriteToLog(0, $"Exception at file mover: {ex.Message}", 0);
                return false;
            }
        }

        private static bool DeleteSrcFolder(string srcFolderPath)
        {
            try
            {
                IEnumerable<string> srcFileCount = Directory.EnumerateFiles(srcFolderPath, "*.*", SearchOption.TopDirectoryOnly);

                if (!srcFileCount.Any())
                {
                    Directory.Delete(srcFolderPath, true);
                }
                return true;
            }
            catch (Exception ex)
            {
                WriteLogClass.WriteToLog(0, $"Exception at folder delete: {ex.Message}", 0);
                return false;
            }
        }
    }
}
