﻿using FolderFunctions;
using UserConfigSetterClass;
using WriteLog;

namespace HandleErrorFiles
{
    internal class HandleErrorFilesClass
    {
        // Error Folder path.
        public static readonly string ErrorFolderPath = FolderFunctionsClass.CheckFolders("error");

        /// <summary>
        /// This is a bit different from MoveAllFilesToErrorFolder. This function moves each file to the error folder.
        /// </summary>
        /// <param name="sourcePath">Source folder path.</param>
        /// <param name="fileNames">source file name list.</param>
        /// <param name="customerId">Customer ID.</param>
        /// <param name="clientEmail">If user uses email to deliver files then need this to make the folder name.</param>
        /// <returns>True or false.</returns>
        public static async Task<bool> MoveFilesToErrorFolder(string downloadFolderPath,
                                                  IEnumerable<string> fileNames,
                                                  int? customerId,
                                                  string clientEmail)
        {
            try
            {
                // Read the user config file.
                UserConfigSetterClass.UserConfigSetter.CustomerDetailsObject jsonDetails = await UserConfigSetterClass.UserConfigSetter.ReadUserDotConfigAsync<UserConfigSetterClass.UserConfigSetter.CustomerDetailsObject>();
                UserConfigSetterClass.UserConfigSetter.Customerdetail clientDetails = jsonDetails.CustomerDetails.FirstOrDefault(cid => cid.Id == customerId);

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
