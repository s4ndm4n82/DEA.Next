using UserConfigSetterClass;
using WriteLog;
using WriteNamesToLog;
using Directory = System.IO.Directory;

namespace DEA.Next.FileOperations.TpsFileFunctions
{
    internal class SendToWebServiceHelpertFunctions
    {
        public static string SetCustomerOrg(int ftpFolderLoop,
                                            int sendEmail,
                                            string customerOrg,
                                            string ftpFolderName,
                                            string recipientEmail)
        {
            string clientOrg = customerOrg;

            if (ftpFolderLoop == 1)
            {
                clientOrg = ftpFolderName;
            }

            if (sendEmail == 1)
            {
                clientOrg = recipientEmail;
            }

            return clientOrg;
        }

        public static string[] MakeDownloadedFileList(UserConfigSetter.Customerdetail clientDetails,
                                                      string localFilePath,
                                                      string clientOrg,
                                                      string[] ftpFileList)
        {
            // Loading the accepted extension list.
            var acceptedExtentions = clientDetails
                                              .DocumentDetails
                                              .DocumentExtensions
                                              .Select(e => e.ToLower())
                                              .ToList();
            // Creating the list of file in the local download folder.
            var localFileNameList = Directory.EnumerateFiles(localFilePath, "*.*", SearchOption.TopDirectoryOnly)
                .Where(f => acceptedExtentions.Contains(Path.GetExtension(f).ToLower()))
                .Where(f => ftpFileList.Any(g => Path.GetFileNameWithoutExtension(f)
                    .Equals(Path.GetFileNameWithoutExtension(g), StringComparison.OrdinalIgnoreCase)))
                .ToArray();
            
            return clientDetails.RenameFile == 1 ? RenameFileList(localFilePath, clientOrg, localFileNameList) : localFileNameList;
        }

        public static string[] MakeLocalFileList(string localFilePath, string acceptedExtensions)
        {
            var localPath = Path.GetDirectoryName(localFilePath);
            
            // Creating the list of file in the local download folder.
            if (localPath != null)
                return Directory.EnumerateFiles(localPath, "*.*", SearchOption.TopDirectoryOnly)
                    .Where(f => acceptedExtensions.Contains(Path.GetExtension(f).ToLower()))
                    .ToArray();
            
            WriteLogClass.WriteToLog(0, "Local file path is empty ....", 1);
            return Array.Empty<string>();
        }
        
        private static string[] RenameFileList(string localFilePath, string clientOrg, string[] localFileList)
        {
            List<string> renamedFileList = new();

            foreach (var localFile in localFileList)
            {
                var newFileName = Path.Combine(localFilePath, clientOrg + "_" + Path.GetFileName(localFile));

                if (File.Exists(newFileName)) continue;
                
                try
                {
                    File.Move(localFile, newFileName);
                    renamedFileList.Add(newFileName);
                }
                catch (IOException ex)
                {
                    WriteLogClass.WriteToLog(0, $"IO Exception at RenameFileList: {ex.Message}", 0);
                }
            }

            WriteLogClass.WriteToLog(1, $"Renamed files: {WriteNamesToLogClass.GetFileNames(renamedFileList.ToArray())}", 1);

            return renamedFileList.ToArray();
        }
    }
}
