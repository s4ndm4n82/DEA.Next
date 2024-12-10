using WriteLog;
using FluentFTP;
using DEA.Next.FileOperations.TpsJsonStringCreatorFunctions;
using DEA.Next.FileOperations.TpsFileFunctions;
using DEA.Next.HelperClasses.ConfigFileFunctions;
using Renci.SshNet;

namespace FileFunctions;

internal class SendToWebServiceProject
{
    public static async Task<int> SendToWebServiceProjectAsync(AsyncFtpClient? ftpConnect,
        SftpClient? sftpConnect,
        Guid customerId,
        string[] ftpFileList,
        string[]? localFileList,
        string filePath,
        string ftpFolderName,
        string recipientEmail,
        string emailSubject)
    {
        try
        {
            WriteLogClass.WriteToLog(1, "Starting file upload process .... ", 4);

            var clientDetails = await UserConfigRetriever.RetrieveUserConfigById(customerId);
            var ftpDetails = await UserConfigRetriever.RetrieveFtpConfigById(customerId);
            var emailDetails = await UserConfigRetriever.RetrieveEmailConfigById(customerId);

            // Get the correct org number depending on what type of download method is used.
            var clientOrg = SendToWebServiceHelpertFunctions.SetCustomerOrg(ftpDetails.FtpFolderLoop,
                clientDetails.SendEmail,
                clientDetails.SendSubject,
                clientDetails.FieldOneValue,
                ftpFolderName,
                recipientEmail,
                emailSubject);
            
            // Creates the file list of the downloaded files.
            var downloadedFiles = SendToWebServiceHelpertFunctions.MakeDownloadedFileList(customerId,
                filePath,
                clientOrg,
                ftpFileList);

            return await MakeJsonRequestProjectsFunction.MakeJsonRequestProjects(ftpConnect,
                sftpConnect,
                customerId,
                clientOrg,
                downloadedFiles,
                ftpFileList,
                localFileList);
        }
        catch (Exception ex)
        {
            WriteLogClass.WriteToLog(0, $"Exception at SendToWebService: {ex.Message}", 0);
            return -1;
        }
    }
}