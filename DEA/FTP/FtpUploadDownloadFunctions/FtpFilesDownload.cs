using DEA.Next.FTP.FtpUploadDownloadFunctions;
using DEA.Next.Graph.GraphClientRelatedFunctions;
using DEA.Next.HelperClasses.ConfigFileFunctions;
using FluentFTP;
using Renci.SshNet;
using Renci.SshNet.Async;
using Renci.SshNet.Sftp;
using WriteLog;
using WriteNamesToLog;

namespace DownloadFtpFilesClass;

internal class FtpFilesDownload
{
    /// <summary>
    ///     Download the files from the FTP server in batches cofigured in the appsettings.json file.
    /// </summary>
    /// <param name="ftpConnect">FTP connection token.</param>
    /// <param name="sftpConnect"></param>
    /// <param name="ftpPath">FTP folder path</param>
    /// <param name="downloadFolderPath">Local download folder path.</param>
    /// <param name="ftpFolderName"></param>
    /// <param name="clientId">ID of the client take from the config file.</param>
    /// <returns></returns>
    public static async Task<int> DownloadFtpFilesFunction(AsyncFtpClient? ftpConnect,
        SftpClient? sftpConnect,
        string ftpPath,
        string downloadFolderPath,
        string ftpFolderName,
        Guid clientId)
    {
        // Return value.
        var result = -1;

        try
        {
            // Reads the ClientConfigs.
            var documentDetails = await UserConfigRetriever.RetrieveDocumentConfigById(clientId);
            var clientDetails = await UserConfigRetriever.RetrieveUserConfigById(clientId);

            // Allowed file extensions
            var allowedFileExtensions = documentDetails.Select(e => e.Extension.ToLower()).ToList();

            // Download folder path.
            var downloadFolder = Path.Combine(downloadFolderPath, GraphHelper.FolderNameRnd(10));

            List<FtpFileInfo> downloadResult = [];

            // Gets the FTP file list.
            if (ftpConnect != null)
                downloadResult = await CreateFtpFileList(ftpConnect, ftpPath, downloadFolder, allowedFileExtensions);

            // Gets the SFTP file list.
            if (sftpConnect != null)
                downloadResult = await CreateSftpFileList(sftpConnect, ftpPath, downloadFolder, allowedFileExtensions);

            if (downloadResult.Count == 0)
            {
                WriteLogClass.WriteToLog(1, "The downloadResult list is empty ....", 3);
                return 4;
            }

            WriteLogClass.WriteToLog(1,
                $"Downloaded file names: {WriteNamesToLogClass
                    .GetFileNames(downloadResult
                        .Select(f => f.FileName.ToString()).ToArray())}",
                1);

            // Starts the file download process.
            var batchSize = clientDetails.MaxBatchSize;
            var batchCurrentIndex = 0;

            while (batchCurrentIndex < downloadResult.Count) // Loop until all the files are downloaded.
            {
                // Gets the current batch of files.
                var currentBatch = downloadResult
                    .Skip(batchCurrentIndex)
                    .Take(batchSize).ToList();

                result = await FtpFilesUpload.FilesUploadFunction(ftpConnect,
                    sftpConnect,
                    currentBatch.Select(r => r.RemoteFilePath.ToString()).ToArray(),
                    downloadFolder,
                    currentBatch.Select(s => s.FileName.ToString()).ToArray(),
                    ftpFolderName,
                    clientId);

                if (result is 3 or 4) return result;

                // Increment the batch index
                batchCurrentIndex += batchSize;
            }

            return result;
        }
        catch (Exception ex)
        {
            WriteLogClass.WriteToLog(0, $"Exception at file download: {ex.Message}", 0);
            return result;
        }
    }

    private static async Task<List<FtpFileInfo>> CreateFtpFileList(AsyncFtpClient ftpConnect,
        string ftpPath,
        string downloadFolder,
        List<string> allowedFileExtensions)
    {
        try
        {
            // Gets the FTP file list.
            var ftpFileNameList = await ftpConnect.GetListing(ftpPath);

            // Filter the FTP file list. And add it to the filesToDownload list.
            var filesToDownload = ftpFileNameList
                .Where(f => f.Type == FtpObjectType.File &&
                            allowedFileExtensions.Any(ext =>
                                f.FullName.EndsWith(ext, StringComparison.OrdinalIgnoreCase)))
                .Select(f => f.FullName);

            // Initiate the download file list variable.
            var downloadResult = await ftpConnect.DownloadFiles(downloadFolder,
                filesToDownload,
                FtpLocalExists.Resume,
                FtpVerify.Retry);

            // Creating the new list to return.
            List<FtpFileInfo> ftpFileList = [];

            ftpFileList.AddRange(downloadResult
                .Select(fileInfo =>
                    new FtpFileInfo { RemoteFilePath = fileInfo.RemotePath, FileName = fileInfo.Name }));

            return ftpFileList;
        }
        catch (Exception ex)
        {
            WriteLogClass.WriteToLog(0, $"Exception at CreateFtpFileList: {ex.Message}", 0);
            throw new InvalidOperationException("Ftp or Sftp is not connected");
        }
    }

    private static async Task<List<FtpFileInfo>> CreateSftpFileList(SftpClient sftpConnect,
        string sftpPath,
        string downloaFolder,
        List<string> allowedFileExtensions)
    {
        try
        {
            // Creat the download folder if it's not there.
            if (!Directory.Exists(downloaFolder)) Directory.CreateDirectory(downloaFolder);

            // Creating the file stream.
            FileStream fileStream = null;

            // Get the SFTP file list.
            IEnumerable<ISftpFile> sftpFileNameList = sftpConnect.ListDirectory(sftpPath)
                .Where(f => f.IsRegularFile && allowedFileExtensions
                    .Any(ext => f.Name.EndsWith(ext, StringComparison.OrdinalIgnoreCase)));

            List<FtpFileInfo> ftpFileList = new();

            // Initiate the file download.
            foreach (var sftpFile in sftpFileNameList)
            {
                // Creating the local file path.
                var localFilePath = Path.Combine(downloaFolder, sftpFile.Name);

                // Creating or opening the file stream for each file.
                fileStream = File.Create(localFilePath);

                // Downloading the file.
                await sftpConnect.DownloadAsync(sftpFile.FullName, fileStream);

                // Closing the file stream.
                fileStream.Close();

                // Creating the new list to return.
                FtpFileInfo ftpFileInfo = new()
                {
                    RemoteFilePath = sftpFile.FullName,
                    FileName = sftpFile.Name
                };

                ftpFileList.Add(ftpFileInfo);
            }

            return ftpFileList;
        }
        catch (Exception ex)
        {
            WriteLogClass.WriteToLog(0, $"Exception at CreateSftpFileList: {ex.Message}", 0);
            return null;
        }
    }

    public class FtpFileInfo
    {
        public string FileName { get; init; } = string.Empty;
        public string RemoteFilePath { get; init; } = string.Empty;
    }
}