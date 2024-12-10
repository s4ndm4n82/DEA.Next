using DEA.Next.HelperClasses.ConfigFileFunctions;
using UserConfigSetterClass;
using WriteLog;
using WriteNamesToLog;
using Directory = System.IO.Directory;

namespace DEA.Next.FileOperations.TpsFileFunctions;

internal class SendToWebServiceHelpertFunctions
{
    public static string SetCustomerOrg(bool ftpFolderLoop,
        bool sendEmail,
        bool sendSubject,
        string customerOrg,
        string ftpFolderName,
        string recipientEmail,
        string emailSubject)
    {
        var clientOrg = customerOrg;

        if (ftpFolderLoop) clientOrg = ftpFolderName;

        if (sendEmail) clientOrg = recipientEmail;

        if (sendSubject) clientOrg = emailSubject;

        return clientOrg;
    }

    public static async Task<string[]> MakeDownloadedFileList(Guid customerId,
        string localFilePath,
        string clientOrg,
        string[] ftpFileList)
    {
        var documentDetails = await UserConfigRetriever.RetrieveDocumentConfigById(customerId);
        // Loading the accepted extension list.
        var acceptedExtensions = documentDetails.Select(d => d.Extension.ToLower()).ToArray(); 

        // Creating the list of file in the local download folder.
        var localFileNameList = Directory.EnumerateFiles(localFilePath, "*.*", SearchOption.TopDirectoryOnly)
            .Where(f => acceptedExtensions.Contains(Path.GetExtension(f).ToLower()))
            .Where(f => ftpFileList.Any(g => Path.GetFileNameWithoutExtension(f)
                .Equals(Path.GetFileNameWithoutExtension(g), StringComparison.OrdinalIgnoreCase)))
            .ToArray();

        return localFileNameList;
        // return clientDetails.RenameFile == 1 ? RenameFileList(localFilePath, clientOrg, localFileNameList) : localFileNameList;
    }

    private static string[] RenameFileList(string localFilePath, string clientOrg, string[] localFileList)
    {
        List<string> renamedFileList = [];

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