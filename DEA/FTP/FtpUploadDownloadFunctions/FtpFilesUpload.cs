using DEA.Next.FileOperations.TpsFileFunctions;
using DEA.Next.HelperClasses.ConfigFileFunctions;
using FluentFTP;
using Renci.SshNet;

namespace DEA.Next.FTP.FtpUploadDownloadFunctions;

internal class FtpFilesUpload
{
    /// <summary>
    ///     File download function. This will be parsing the files to the FTP client and downloading them the the local folder.
    /// </summary>
    /// <param name="ftpConnect">FTP connection token.</param>
    /// <param name="sftpConnect"></param>
    /// <param name="currentBatch">Files downloaded from the server.</param>
    /// <param name="ftpHoldFolder">Local download folder.</param>
    /// <param name="ftpFolderName"></param>
    /// <param name="clientId">Client ID.</param>
    /// <param name="fileNames"></param>
    /// <returns></returns>
    public static async Task<int> FilesUploadFunction(AsyncFtpClient? ftpConnect,
        SftpClient? sftpConnect,
        string[] currentBatch,
        string ftpHoldFolder,
        string[] fileNames,
        string ftpFolderName,
        Guid clientId)
    {
        var customerDetail = await UserConfigRetriever.RetrieveUserConfigById(clientId);

        // Get the matching file names.
        var matchingFileNames = currentBatch
            .Where(batchFile => fileNames
                .Any(fileName => Path.GetFileNameWithoutExtension(batchFile)
                    .Equals(
                        Path.GetFileNameWithoutExtension(fileName),
                        StringComparison.OrdinalIgnoreCase)))
            .ToArray();

        // Get the local files.
        var localFiles = Directory.GetFiles(ftpHoldFolder, "*.*", SearchOption.TopDirectoryOnly);

        // If the project ID is not empty, then send the files to the web service using normal upload.
        if (!string.IsNullOrWhiteSpace(customerDetail.ProjectId))
            return await SendToWebServiceProject.SendToWebServiceProjectAsync(ftpConnect,
                sftpConnect,
                clientId,
                matchingFileNames,
                localFiles,
                ftpHoldFolder,
                ftpFolderName,
                null!,
                string.Empty);

        // If the project ID is empty then it's a data file upload. Then this upload process will be used.
        return await SendToWebServiceDataFile.SendToWebServiceDataFileAsync(ftpConnect,
            sftpConnect,
            clientId,
            ftpHoldFolder,
            matchingFileNames,
            localFiles);
    }
}