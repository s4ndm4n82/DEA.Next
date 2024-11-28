using AppConfigReader;
using FluentFTP;
using GraphHelper;
using Renci.SshNet;
using Renci.SshNet.Async;
using Renci.SshNet.Sftp;
using UploadFtpFilesClass;
using UserConfigRetriverClass;
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
            // Return value.
            int result = -1;

            try
            {
                // Reads the ClientConfig.json file.
                UserConfigSetter.Customerdetail jsonData = await UserConfigRetriver.RetriveUserConfigById(clientID);
                
                // Reads the appsettings.json file.
                var appJsonData = AppConfigReaderClass.ReadAppDotConfig();
                
                // Allowed file extentions
                List<string> allowedFileExtensions = jsonData.DocumentDetails.DocumentExtensions;

                // Download folder path.
                string downloaFolder = Path.Combine(downloadFolderPath, GraphHelperClass.FolderNameRnd(10));

                List<FtpFileInfo> downloadResult = new();
                // Gets the FTP file list.
                if (ftpConnect != null)
                {
                    downloadResult = await CreateFtpFileList(ftpConnect, ftpPath, downloaFolder, allowedFileExtensions);
                }

                // Gets the SFTP file list.
                if (sftpConnect != null)
                {
                    downloadResult = await CreateSftpFileList(sftpConnect, ftpPath, downloaFolder, allowedFileExtensions);
                }

                if (downloadResult.Count == 0)
                {
                    WriteLogClass.WriteToLog(1, "The downloadResult list is empty ....", 3);
                    return 4;
                }

                WriteLogClass.WriteToLog(1, $"Downloaded file names: {WriteNamesToLogClass.GetFileNames(downloadResult.Select(f => f.FileName.ToString()).ToArray())}", 1);

                // Starts the file download process.
                int batchSize = jsonData.MaxBatchSize;
                int batchCurrentIndex = 0;

                while (batchCurrentIndex < downloadResult.Count) // Loop until all the files are downloaded.
                {
                    // Gets the current batch of files.
                    IEnumerable<FtpFileInfo> currentBatch = downloadResult
                                                            .Skip(batchCurrentIndex)
                                                            .Take(batchSize);

                    result = await FtpFilesUpload.FilesUploadFuntcion(ftpConnect,
                                                                      sftpConnect,
                                                                      currentBatch.Select(r => r.RemoteFilePath.ToString()).ToArray(),
                                                                      downloaFolder,
                                                                      currentBatch.Select(s => s.FileName.ToString()).ToArray(),
                                                                      ftpFolderName,
                                                                      clientID);

                    if (result == 3 || result == 4)
                    {
                        return result;
                    }

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
                FileStream fileStream = null;

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
