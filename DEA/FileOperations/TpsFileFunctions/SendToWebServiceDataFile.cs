using DEA.Next.FileOperations.TpsJsonStringCreatorFunctions;
using FluentFTP;
using Renci.SshNet;
using WriteLog;

namespace DEA.Next.FileOperations.TpsFileFunctions;

/// <summary>
/// This function will start the data file upload process.
/// </summary>
internal class SendToWebServiceDataFile
{
    public static async Task<int> SendToWebServiceDataFileAsync(AsyncFtpClient? ftpConnect,
        SftpClient? sftpConnect,
        Guid customerId,
        string localFolderPath,
        string[] ftpFileList,
        string[] localFileList)
    {
        try
        {
            WriteLogClass.WriteToLog(1, "Starting file upload process .... ", 4);

            // Creat the local file list.
            var downloadedFileList = await SendToWebServiceHelpertFunctions.MakeDownloadedFileList(customerId,
                localFolderPath,
                string.Empty,
                ftpFileList);

            if (!downloadedFileList.Any())
            {
                WriteLogClass.WriteToLog(1, "No matching files in the download list ....", 1); // No matching files in the download lis
                return -1;
            }

            return await MakeJsonRequestDataFileFunction.MakeJsonRequestDataFileAsync(ftpConnect,
                sftpConnect,
                customerId,
                localFolderPath,
                downloadedFileList,
                ftpFileList,
                localFileList);
        }
        catch (Exception ex)
        {
            WriteLogClass.WriteToLog(0, $"Exception at SendToWebServiceDataFile: {ex.Message}", 0);
            return -1;
        }
    }
}