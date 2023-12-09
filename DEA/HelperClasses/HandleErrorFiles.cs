using FolderFunctions;
using UserConfigReader;
using WriteLog;

namespace HandleErrorFiles
{
    internal class HandleErrorFilesClass
    {
        // Error Folder path.
        public static readonly string ErrorFolderPath = FolderFunctionsClass.CheckFolders("error");

        /// <summary>
        /// This function is designed to move the entire file set to the error folder.
        /// </summary>
        /// <param name="sourcePath">Source folder path.</param>
        /// <param name="clientID">Client ID.</param>
        /// <param name="recivedEmail">If client use email to send files then this will be used.</param>
        /// <returns>True or false.</returns>
        public static bool MoveAllFilesToErrorFolder(string sourcePath, int clientID, string recivedEmail)
        {
            // Read the user config file.
            UserConfigReaderClass.CustomerDetailsObject jsonData = UserConfigReaderClass.ReadUserDotConfig<UserConfigReaderClass.CustomerDetailsObject>();
            UserConfigReaderClass.Customerdetail clientDetails = jsonData.CustomerDetails.FirstOrDefault(cid => cid.Id == clientID);
            // Client ID.
            string clientNo = clientID.ToString();
            // Source folder path.
            string sourceFolderPath = Path.GetDirectoryName(sourcePath);
            // Source folder name.
            string sourceFolder = sourceFolderPath.Split(Path.DirectorySeparatorChar).Last();
            // Destination folder name.
            string destinationFolderName = clientDetails.FileDeliveryMethod.ToLower() == "email" ? string.Concat("ID_", clientNo, " ", "Email_", recivedEmail)
                                                                                        : string.Concat("ID_", clientNo, " ", "Org_", clientDetails.ClientOrgNo);
            // Destination folder path.
            string destinationFolderPath = Path.Combine(ErrorFolderPath, destinationFolderName, sourceFolder);

            // Source file list.
            DirectoryInfo dirInfo = new(sourceFolderPath);
            IEnumerable<FileInfo> sourceFileNameList = dirInfo.EnumerateFiles("*.*", SearchOption.TopDirectoryOnly);

            if (!Directory.Exists(destinationFolderPath))
            {
                Directory.CreateDirectory(destinationFolderPath);
            }
            // Move files.
            if (FileMover(destinationFolderPath, sourceFileNameList, Path.Combine(destinationFolderName, sourceFolder)))
            {
                // Delete source folder.
                if (DeleteSrcFolder(sourceFolderPath))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Moves the files to the error folder.
        /// </summary>
        /// <param name="dstPath">Destination path.</param>
        /// <param name="srcFileList">Source file list.</param>
        /// <returns>True or false.</returns>
        private static bool FileMover(string dstPath, IEnumerable<FileInfo> srcFileList, string destinationFolder)
        {
            try
            {
                int fileCount = srcFileList.Count();

                WriteLogClass.WriteToLog(1, $"Moving files to {destinationFolder} ....", 1);

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
                WriteLogClass.WriteToLog(1, $"Moved {fileCount} file/s to \\{destinationFolder} ....", 1);
                return true;
            }
            catch (Exception ex)
            {
                WriteLogClass.WriteToLog(0, $"Exception at file mover: {ex.Message}", 0);
                return false;
            }
        }

        /// <summary>
        /// Deletes the source folder.
        /// </summary>
        /// <param name="srcFolderPath">Source folder path</param>
        /// <returns>True or false.</returns>
        private static bool DeleteSrcFolder(string srcFolderPath)
        {
            try
            {
                IEnumerable<string> srcFileCount = Directory.EnumerateFiles(srcFolderPath, "*.*", SearchOption.TopDirectoryOnly);

                if (!srcFileCount.Any())
                {
                    // Delete the download folder.
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

        /// <summary>
        /// This is a bit different from MoveAllFilesToErrorFolder. This function moves each file to the error folder.
        /// </summary>
        /// <param name="sourcePath">Source folder path.</param>
        /// <param name="fileNames">source file name list.</param>
        /// <param name="customerId">Customer ID.</param>
        /// <param name="clientEmail">If user uses email to deliver files then need this to make the folder name.</param>
        /// <returns>True or false.</returns>
        public static bool MoveFilesToErrorFolder(string downloadFolderPath,
                                                  IEnumerable<string> fileNames,
                                                  int? customerId,
                                                  string clientEmail)
        {
            try
            {
                // Read the user config file.
                UserConfigReaderClass.CustomerDetailsObject jsonDetails = UserConfigReaderClass.ReadUserDotConfig<UserConfigReaderClass.CustomerDetailsObject>();
                UserConfigReaderClass.Customerdetail clientDetails = jsonDetails.CustomerDetails.FirstOrDefault(cid => cid.Id == customerId);

                // Source folder path.
                string sourcePath = Path.GetDirectoryName(downloadFolderPath);
                // Source folder name.
                string sourcFolderName = sourcePath.Split(Path.DirectorySeparatorChar).Last();
                // Destination folder name.
                string destinationFolderName = clientDetails.FileDeliveryMethod.ToLower() == "email" ? string.Concat("ID_", customerId.ToString(), " ", "Email_", clientEmail)
                                                                                            : string.Concat("ID_", customerId.ToString(), " ", "Org_", clientDetails.ClientOrgNo);
                // Destination folder path.
                string destinationFolderPath = Path.Combine(ErrorFolderPath, destinationFolderName, sourcFolderName);
                // Create destination folder.
                //Directory.CreateDirectory(!Directory.Exists(destinationFolderPath) ? destinationFolderPath : null);
                if (!Directory.Exists(destinationFolderPath))
                {
                    Directory.CreateDirectory(destinationFolderPath);
                }
                // Move files.
                return MoveEachFile(sourcePath, destinationFolderPath, fileNames, clientDetails.FileDeliveryMethod.ToLower());
            }
            catch (Exception ex)
            {
                WriteLogClass.WriteToLog(0, $"Exception at single file mover: {ex.Message}", 0);
                return false;
            }
        }

        /// <summary>
        /// Moves each file to the error folder.
        /// </summary>
        /// <param name="srcPath">Source path.</param>
        /// <param name="dstPath">Destination path.</param>
        /// <param name="fileNames">List of the file names.</param>
        /// <returns></returns>
        private static bool MoveEachFile(string srcPath, string dstPath, IEnumerable<string> fileNames, string deliveryType)
        {
            try
            {
                WriteLogClass.WriteToLog(1, $"Moving files to {dstPath} ....", 1);
                foreach (string fileName in fileNames)
                {
                    string cleanFileName = fileName;

                    if (deliveryType == DeliveryType.ftp)
                    {
                        cleanFileName = Path.GetFileName(fileName);
                    }

                    string srcFile = Path.Combine(srcPath, cleanFileName);
                    string dstFile = Path.Combine(dstPath, cleanFileName);

                    if (File.Exists(dstFile))
                    {
                        WriteLogClass.WriteToLog(1, "File already exists ....", 1);
                        continue;
                    }
                    else
                    {
                        File.Move(srcFile, dstFile);
                    }
                    WriteLogClass.WriteToLog(1, $"Moved {cleanFileName} file/s to \\{dstPath} ....", 1);
                }                
                return true;
            }
            catch (Exception ex)
            {
                WriteLogClass.WriteToLog(0, $"Exception at single file mover: {ex.Message}", 0);
                return false;
            }
        }

        private static class DeliveryType
        {
            public const string email = "email";
            public const string ftp = "ftp";
        }
    }
}
