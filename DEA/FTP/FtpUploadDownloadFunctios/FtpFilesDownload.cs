using AppConfigReader;
using DEA.Next.HelperClasses.ConfigFileFunctions;
using DEA.Next.HelperClasses.FileFunctions;
using FluentFTP;
using GraphHelper;
using Renci.SshNet;
using Renci.SshNet.Async;
using Renci.SshNet.Sftp;
using UploadFtpFilesClass;
using UserConfigSetterClass;
using WriteLog;
using WriteNamesToLog;

namespace DownloadFtpFilesClass
{
    internal class FtpFilesDownload
    {
        public class FtpFileInfo
        {
            public string FileName { get; set; } = string.Empty;
            public string RemoteFilePath { get; set; } = string.Empty;
        }
        /// <summary>
        /// Download the files from the FTP server in batches cofigured in the appsettings.json file.
        /// </summary>
        /// <param name="ftpConnect">FTP connection token.</param>
        /// <param name="ftpPath">FTP folder path</param>
        /// <param name="downloadFolderPath">Local download folder path.</param>
        /// <param name="clientID">ID of the client take from the config file.</param>
        /// <returns></returns>
        public static async Task<int> DownloadFtpFilesFunction(AsyncFtpClient? ftpConnect,
                                                               SftpClient? sftpConnect,
                                                               string ftpPath,
                                                               string downloadFolderPath,
                                                               string ftpFolderName,
                                                               int clientID)
        {
            try
            {
                // Reads the CustomerConfig.json file.
                var jsonData = await UserConfigRetriever.RetrieveUserConfigById(clientID);
                
                // Allowed file extensions
                var allowedFileExtensions = jsonData.DocumentDetails.DocumentExtensions;

                // Download folder path.
                var downloadFolder = Path.Combine(downloadFolderPath, GraphHelperClass.FolderNameRnd(10));

                List<FtpFileInfo> downloadFilesList = new();

                // Gets the FTP file list.
                if (ftpConnect != null)
                {
                    downloadFilesList = await CreateFtpFileList(ftpConnect, ftpPath, downloadFolder, allowedFileExtensions);
                }

                // Gets the SFTP file list.
                if (sftpConnect != null)
                {
                    downloadFilesList = await CreateSftpFileList(sftpConnect, ftpPath, downloadFolder, allowedFileExtensions);
                }

                switch (downloadFilesList.Count)
                {
                    case 0:
                        WriteLogClass.WriteToLog(1, "The downloadResult list is empty ....", 3);
                        return 4;
                    case > 0 when jsonData.ReadContentSettings.ReadTheContent:
                        return await ReadFileContent.StartReadingFileContent(downloadFolder, downloadFilesList, clientID);
                    default:
                        WriteLogClass.WriteToLog(1, $"Downloaded file names: {WriteNamesToLogClass.GetFileNames(downloadFilesList.Select(f => f.FileName.ToString()).ToArray())}", 1);

                        // Starts the file download process.
                        return await UploadFiles(ftpConnect,
                            sftpConnect,
                            downloadFilesList,
                            jsonData.MaxBatchSize,
                            downloadFolder,
                            ftpFolderName,
                            clientID);
                }
            }
            catch (Exception ex)
            {
                WriteLogClass.WriteToLog(0, $"Exception at file download: {ex.Message}", 0);
                return -1;
            }
        }

        /// <summary>
        /// Uploads files to the TPS system in batches.
        /// </summary>
        /// <param name="ftpConnect">FTP connection token.</param>
        /// <param name="sftpConnect">SFTP connection token.</param>
        /// <param name="downloadFilesList">List of files to upload.</param>
        /// <param name="batchSize">Number of files to upload in each batch.</param>
        /// <param name="downloadFolder">Local download folder path.</param>
        /// <param name="ftpFolderName">Name of the FTP folder.</param>
        /// <param name="clientID">ID of the client for configuration.</param>
        /// <returns>Result code indicating the status of the upload process.</returns>
        private static async Task<int> UploadFiles(AsyncFtpClient ftpConnect,
                                                   SftpClient sftpConnect,
                                                   List<FtpFileInfo> downloadFilesList,
                                                   int batchSize,
                                                   string downloadFolder,
                                                   string ftpFolderName,
                                                   int clientID)
        {
            var result = -1;

            try
            {
                var appJsonData = AppConfigReaderClass.ReadAppDotConfig();
                var delayTime = appJsonData.ProgramSettings.UploadDelayTime;
                
                for (var batchCurrentIndex = 0; batchCurrentIndex < downloadFilesList.Count; batchCurrentIndex += batchSize)
                {
                    // Get the current batch of files to upload.
                    var currentBatch = downloadFilesList
                        .Skip(batchCurrentIndex)
                        .Take(batchSize);

                    // Extract remote file paths and file names from the current batch.
                    var remoteFilePaths = currentBatch.Select(r => r.RemoteFilePath.ToString()).ToArray();
                    var fileNames = currentBatch.Select(s => s.FileName.ToString()).ToArray();

                    // Upload the current batch of files.
                    result = await FtpFilesUpload.FilesUploadFuntcion(ftpConnect,
                        sftpConnect,
                        remoteFilePaths,
                        downloadFolder,
                        fileNames,
                        ftpFolderName,
                        clientID);

                    if (result == 3 || result == 4)
                    {
                        return result;
                    }
                    
                    // Wait for the specified time before uploading the next batch of files
                    await Task.Delay(delayTime);
                }

                return result;
            }
            catch (Exception ex)
            {
                // Log any exceptions that occur during file upload.
                WriteLogClass.WriteToLog(0, $"Exception at file upload: {ex.Message}", 0);
                return result;
            }
        }

        private static async Task<List<FtpFileInfo>> CreateFtpFileList(AsyncFtpClient ftpConnect,
                                                                       string ftpPath,
                                                                       string downloaFolder,
                                                                       List<string> allowedFileExtensions)
        {
            try
            {
                // Gets the FTP file list.
                IEnumerable<FtpListItem> ftpFileNameList = await ftpConnect.GetListing(ftpPath);

                // Filter the FTP file list. And add it to the filesToDownload list.
                IEnumerable<string> filesToDownload = ftpFileNameList
                                                         .Where(f => f.Type == FtpObjectType.File && allowedFileExtensions.Any(ext => f.FullName.EndsWith(ext, StringComparison.OrdinalIgnoreCase)))
                                                         .Select(f => f.FullName);

                // Initiate the download file list variable.
                List<FtpResult> downloadResult = await ftpConnect.DownloadFiles(downloaFolder,
                                                                                filesToDownload,
                                                                                FtpLocalExists.Resume,
                                                                                FtpVerify.Retry);
                // Creating the new list to return.
                List<FtpFileInfo> ftpFileList = new();

                foreach (FtpResult fileInfo in downloadResult)
                {
                    FtpFileInfo ftpFileInfo = new()
                    {
                        RemoteFilePath = fileInfo.RemotePath,
                        FileName = fileInfo.Name
                    };

                    ftpFileList.Add(ftpFileInfo);
                }

                return ftpFileList;
            }
            catch (Exception ex)
            {
                WriteLogClass.WriteToLog(0, $"Exception at CreateFtpFileList: {ex.Message}", 0);
                return null;
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
                if (!Directory.Exists(downloaFolder))
                {
                    Directory.CreateDirectory(downloaFolder);
                }

                // Creating the file stream.
                FileStream fileStream;

                // Get the SFTP file list.
                IEnumerable<ISftpFile> sftpFileNameList = sftpConnect.ListDirectory(sftpPath)
                                                                     .Where(f => f.IsRegularFile && allowedFileExtensions
                                                                     .Any(ext => f.Name.EndsWith(ext, StringComparison.OrdinalIgnoreCase)));

                List<FtpFileInfo> ftpFileList = new();

                // Initiate the file download.
                foreach (ISftpFile sftpFile in sftpFileNameList)
                {
                    // Creating the local file path.
                    string localFilePath = Path.Combine(downloaFolder, sftpFile.Name);

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
    }
}
