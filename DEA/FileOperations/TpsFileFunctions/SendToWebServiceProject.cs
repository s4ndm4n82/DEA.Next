using DEA.Next.FileOperations.TpsJsonStringCreatorFunctions;
using DEA.Next.HelperClasses.ConfigFileFunctions;
using FluentFTP;
using Renci.SshNet;
using WriteLog;

namespace DEA.Next.FileOperations.TpsFileFunctions;

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

            var clientOrg = string.Empty;
            var clientDetails = await UserConfigRetriever.RetrieveUserConfigById(customerId);
            var ftpDetails = clientDetails.FtpDetails;
            var emailDetails = clientDetails.EmailDetails;

            if (localFileList == null)
            {
                WriteLogClass.WriteToLog(0, "No local file list found ....", 0);
                return -1;
            }

            if (ftpDetails == null && emailDetails == null)
            {
                WriteLogClass.WriteToLog(0, "No email or ftp details found ....", 0);
                return -1;
            }

            // Get the correct org number depending on what type of download method is used.
            if (ftpDetails != null)
                clientOrg = SendToWebServiceHelpertFunctions.SetCustomerOrg(ftpDetails.FtpFolderLoop,
                    false,
                    false,
                    clientDetails.FieldOneValue,
                    ftpFolderName,
                    recipientEmail,
                    emailSubject);

            if (emailDetails != null)
                clientOrg = SendToWebServiceHelpertFunctions.SetCustomerOrg(false,
                    emailDetails.SendEmail,
                    emailDetails.SendSubject,
                    clientDetails.FieldOneValue,
                    ftpFolderName,
                    recipientEmail,
                    emailSubject);

            // Creates the file list of the downloaded files.
            var downloadedFiles = await SendToWebServiceHelpertFunctions.MakeDownloadedFileList(customerId,
                filePath,
                clientOrg,
                ftpFileList);

            if (downloadedFiles.Length != 0)
                return await MakeJsonRequestProjectsFunction.MakeJsonRequestProjects(ftpConnect,
                    sftpConnect,
                    customerId,
                    clientOrg,
                    downloadedFiles,
                    ftpFileList,
                    localFileList);

            WriteLogClass.WriteToLog(1, "No matching files in the download list ....", 1);
            return -1;
        }
        catch (Exception ex)
        {
            WriteLogClass.WriteToLog(0, $"Exception at SendToWebService: {ex.Message}", 0);
            return -1;
        }
    }
}