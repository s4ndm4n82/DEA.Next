using AppConfigReader;
using FluentFTP;
using GraphHelper;
using UploadFtpFilesClass;
using WriteLog;

namespace DownloadFtpFilesClass
{
    internal class DownloadFtpFiles
    {
        /// <summary>
        /// Download the files from the FTP server in batches cofigured in the appsettings.json file.
        /// </summary>
        /// <param name="ftpConnect">FTP connection token.</param>
        /// <param name="ftpPath">FTP folder path</param>
        /// <param name="downloadFolderPath">Local download folder path.</param>
        /// <param name="clientID">ID of the client take from the config file.</param>
        /// <returns></returns>
        public static async Task<int> DownloadFtpFilesFunction(AsyncFtpClient ftpConnect, string ftpPath, string downloadFolderPath, int clientID)
        {
            // Return value.
            int result = -1;

            try
            {
                // Reads the appsettings.json file.
                AppConfigReaderClass.AppSettingsRoot jsonData = AppConfigReaderClass.ReadAppDotConfig();
                // Gets the FTP file list.
                IEnumerable<FtpListItem> ftpFileNameList = await ftpConnect.GetListing(ftpPath);
                // Gets the files to download.
                IEnumerable<string> filesToDownload = ftpFileNameList
                                                      .Where(f => f.Type == FtpObjectType.File)
                                                      .Select(f => f.FullName)
                                                      .ToArray();

                // If there are no files to download then returns early terminating the execution.
                if (!filesToDownload.Any())
                {
                    return 4;
                }
                // Download folder path.
                string downloaFolder = Path.Combine(downloadFolderPath, GraphHelperClass.FolderNameRnd(10));

                // Starts the file download process.
                List<FtpResult> downloadResult = await ftpConnect.DownloadFiles(downloaFolder, filesToDownload, FtpLocalExists.Resume, FtpVerify.Retry);

                // Starts the file download process.
                int batchSize = jsonData.ProgramSettings.MaxBatchSize;
                int totalFtpFiles = filesToDownload.Count();
                int batchCurrentIndex = 0;

                while (batchCurrentIndex < totalFtpFiles) // Loop until all the files are downloaded.
                {
                    // Gets the current batch of files.
                    IEnumerable<FtpResult> currentBatch = downloadResult.Skip(batchCurrentIndex).Take(batchSize);

                    foreach (FtpResult ftpFile in currentBatch)
                    {
                        result = await UploadFtpFiles.FilesUploadFuntcion(ftpConnect, currentBatch.Select(r => r.RemotePath.ToString()).ToArray(), downloaFolder, ftpFile.Name, clientID);
                    }

                    if (result == 3 || result == 4)
                    {
                        return result;
                    }

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
    }
}
