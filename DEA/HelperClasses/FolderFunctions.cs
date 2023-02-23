using WriteLog;

namespace FolderFunctionsClasses
{
    internal class FolderFunctions
    {
        /// <summary>
        /// Check if the folder paths and folders exsits. If not they are created this is initiated on program launch.
        /// </summary>
        /// <param name="folderSwitch"></param>
        /// <returns>Folder paths have use the correct download folder name.</returns>
        public static string CheckFolders(string folderSwitch)
        {
            string? pthRootFolder = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string downloadFolderName = "Download";
            string attachmentFolderName = "Attachments";
            string ftpFileFolderName = "FTPFiles";
            string logFolderName = "Logs";
            string pathDownloadFolder = Path.Combine(pthRootFolder!, downloadFolderName); // Main file download folder.
            string pathLogFolder = Path.Combine(pthRootFolder!, logFolderName); // Log folder
            string attachmentPath = Path.Combine(pathDownloadFolder!, attachmentFolderName); // Email attachment download folder.
            string ftpPath = Path.Combine(pathDownloadFolder, ftpFileFolderName); // FTP file download folder.

            // Check if download folder exists. If not creates the fodler.
            if (!System.IO.Directory.Exists(pathDownloadFolder))
            {
                try
                {
                    System.IO.Directory.CreateDirectory(pathDownloadFolder);
                }
                catch (Exception ex)
                {
                    WriteLogClass.WriteToLog(1, $"Exception at download folder creation: {ex.Message}", 1);
                }
            }

            if (!System.IO.Directory.Exists(attachmentPath))
            {
                try
                {
                    System.IO.Directory.CreateDirectory(attachmentPath);
                }
                catch (Exception ex)
                {
                    WriteLogClass.WriteToLog(1, $"Exception at attachmnet folder creation: {ex.Message}", 1);
                }
            }

            if (System.IO.Directory.Exists(ftpPath))
            {
                try
                {
                    System.IO.Directory.CreateDirectory(ftpPath);
                }
                catch (Exception ex)
                {
                    WriteLogClass.WriteToLog(1, $"Exception at FTP folder creation: {ex.Message}", 1);
                }
            }

            if (!System.IO.Directory.Exists(pathLogFolder))
            {
                try
                {
                    System.IO.Directory.CreateDirectory(pathLogFolder);
                }
                catch (Exception ex)
                {
                    WriteLogClass.WriteToLog(1, $"Exception at download folder creation: {ex.Message}", 1);
                }
            }

            // Get current execution path.
            string folderPath;
            if (folderSwitch.ToLower() == "download")
            {
                folderPath = pathDownloadFolder;
            }
            else if (folderSwitch.ToLower() == "log")
            {
                folderPath = pathLogFolder;
            }
            else if (folderSwitch.ToLower() == "email")
            {
                folderPath = attachmentPath;
            }
            else if (folderSwitch.ToLower() == "ftp")
            {
                folderPath = ftpPath;
            }
            else
            {
                folderPath = string.Empty;
            }

            return folderPath;
        }
    }
}
