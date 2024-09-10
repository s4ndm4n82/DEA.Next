using UserConfigSetterClass;
using WriteLog;
using WriteNamesToLog;
using Directory = System.IO.Directory;

namespace DEA.Next.FileOperations.TpsFileFunctions
{
    internal class SendToWebServiceHelpertFunctions
    {
        public static string SetCustomerOrg(int ftpFolerLoop,
                                            int sendEmail,
                                            string customerOrg,
                                            string ftpFolderName,
                                            string recipientEmail)
        {
            string clientOrg = customerOrg;

            if (ftpFolerLoop == 1)
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
            List<string> acceptedExtentions = clientDetails
                                              .DocumentDetails
                                              .DocumentExtensions
                                              .Select(e => e.ToLower())
                                              .ToList();

            // Creating the list of file in the local download folder.
            string[] localFileNameList = Directory.EnumerateFiles(localFilePath, "*.*", SearchOption.TopDirectoryOnly)
                                                                  .Where(f => acceptedExtentions.Contains(Path.GetExtension(f).ToLower()))
                                                                  .Where(f => ftpFileList.Any(g => Path.GetFileNameWithoutExtension(f)
                                                                  .Equals(Path.GetFileNameWithoutExtension(g), StringComparison.OrdinalIgnoreCase)))
                                                                  .ToArray();

            if (clientDetails.RenameFile == 1)
            {
                return RenameFileList(localFilePath, clientOrg, localFileNameList);               
            }

            return localFileNameList;            
        }

        private static string[] RenameFileList(string localFilePath, string clientOrg, string[] localFileList)
        {
            List<string> renamedFileList = new();

            foreach (string localFile in localFileList)
            {   
                string newFileName = Path.Combine(localFilePath, clientOrg + "_" + Path.GetFileName(localFile));

                if (!File.Exists(newFileName))
                {
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
            }

            WriteLogClass.WriteToLog(1, $"Renamed files: {WriteNamesToLogClass.GetFileNames(renamedFileList.ToArray())}", 1);

            return renamedFileList.ToArray();
        }
    }
}
