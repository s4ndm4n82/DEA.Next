using UserConfigSetterClass;
using Directory = System.IO.Directory;

namespace DEA.Next.FileOperations.TpsFileFunctions
{
    internal class SendToWebServiceHelpertFunctions
    {
        public static string SetCustomerOrg(int ftpFolerLoop, int sendEmail, string customerOrg, string ftpFolderName, string recipientEmail)
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

        public static string[] MakeDownloadedFileList(UserConfigSetter.Customerdetail clientDetails, string localFilePath, string[] ftpFileList)
        {
            // Loading the accepted extension list.
            List<string> acceptedExtentions = clientDetails
                                              .DocumentDetails
                                              .DocumentExtensions
                                              .Select(e => e.ToLower())
                                              .ToList();

            // Creating the list of file in the local download folder.
            return Directory.GetFiles(localFilePath, "*.*", SearchOption.TopDirectoryOnly)
                                      .Where(f => acceptedExtentions.Contains(Path.GetExtension(f).ToLower()))
                                      .Where(f => ftpFileList.Any(g => Path.GetFileNameWithoutExtension(f)
                                      .Equals(Path.GetFileNameWithoutExtension(g), StringComparison.OrdinalIgnoreCase)))
                                      .ToArray();
        }
    }
}
