using FluentFTP;
using GraphHelper;
using UploadFtpFilesClass;
using UserConfigRetriverClass;
using UserConfigSetterClass;
using WriteLog;
using WriteNamesToLog;

namespace DownloadFtpFilesClass
{
    internal class FtpFilesDownload
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
                UserConfigSetter.Customerdetail jsonData = await UserConfigRetriver.RetriveUserConfigById(clientID);
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
                    WriteLogClass.WriteToLog(0, "The filesToDownload list is empty ....", 0);
                    return 4;
                }

                // Download folder path.
                string downloaFolder = Path.Combine(downloadFolderPath, GraphHelperClass.FolderNameRnd(10));

                // Starts the file download process.
                List<FtpResult> downloadResult = await ftpConnect.DownloadFiles(downloaFolder, filesToDownload, FtpLocalExists.Resume, FtpVerify.Retry);

                if (!downloadResult.Any())
                {
                    WriteLogClass.WriteToLog(0, "The downloadResult list is empty ....", 0);
                    return 4;
                }

                WriteLogClass.WriteToLog(1, $"Downloaded file names: {WriteNamesToLogClass.GetFileNames(downloadResult.Select(f => f.Name.ToString()).ToArray())}", 2);

                // Starts the file download process.
                int batchSize = jsonData.MaxBatchSize;
                int batchCurrentIndex = 0;

                while (batchCurrentIndex < downloadResult.Count) // Loop until all the files are downloaded.
                {
                    // Gets the current batch of files.
                    IEnumerable<FtpResult> currentBatch = downloadResult
                                                          .Skip(batchCurrentIndex)
                                                          .Take(batchSize);
                    
                    result = await FtpFilesUpload.FilesUploadFuntcion(ftpConnect, currentBatch.Select(r => r.RemotePath.ToString()).ToArray(), downloaFolder, currentBatch.Select(s => s.Name.ToString()).ToArray(), clientID);

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
