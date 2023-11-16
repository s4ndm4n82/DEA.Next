using FolderFunctions;
using UserConfigReader;
using WriteLog;

namespace HandleErrorFiles
{
    internal class HandleErrorFilesClass
    {
        public static bool MoveFilesToErrorFolder(string sourcePath, int clientID, string recivedEmail)
        {
            UserConfigReaderClass.CustomerDetailsObject jsonData = UserConfigReaderClass.ReadUserDotConfig<UserConfigReaderClass.CustomerDetailsObject>();
            UserConfigReaderClass.Customerdetail clientDetails = jsonData.CustomerDetails.FirstOrDefault(cid => cid.Id == clientID);

            string clientNo = clientID.ToString();
            string errorFolderPath = FolderFunctionsClass.CheckFolders("error");
            string sourceFolderPath = Path.GetDirectoryName(sourcePath)!;
            string sourceFolder = sourceFolderPath.Split(Path.DirectorySeparatorChar).Last();
            string folderName = clientDetails.FileDeliveryMethod.ToLower() == "email" ? string.Concat("ID_", clientNo, " ", "Email_", recivedEmail) : string.Concat("ID_", clientNo, " ", "Org_", clientDetails.ClientOrgNo);
            string destinationFolderPath = Path.Combine(errorFolderPath, folderName, sourceFolder);

            DirectoryInfo dirInfo = new(sourceFolderPath);
            IEnumerable<FileInfo> sourceFileNameList = dirInfo.EnumerateFiles("*.*", SearchOption.TopDirectoryOnly);
            int fileCount = sourceFileNameList.Count();

            if (!Directory.Exists(destinationFolderPath))
            {
                Directory.CreateDirectory(destinationFolderPath);
            }

            if (FileMover(destinationFolderPath, sourceFileNameList))
            {
                if (DeleteSrcFolder(sourceFolderPath))
                {
                    string movedFolder = Path.Combine(folderName, sourceFolder);
                    WriteLogClass.WriteToLog(1, $"Moved {fileCount} file/s to \\{movedFolder} ....", 1);
                    return true;
                }
            }
            return false;
        }

        private static bool FileMover(string dstPath, IEnumerable<FileInfo> srcFileList)
        {
            try
            {
                foreach (FileInfo srcFile in srcFileList)
                {   
                    string dstFile = Path.Combine(dstPath, srcFile.Name);

                    if (File.Exists(dstFile))
                    {
                        WriteLogClass.WriteToLog(1, "File already exists ....", 1);
                        continue;
                    }
                    else
                    {
                        File.Move(srcFile.FullName, dstFile);
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
                    string dirPath = Path.GetDirectoryName(srcFolderPath)!;
                    Directory.Delete(dirPath, true);
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
