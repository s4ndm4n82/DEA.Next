using DownloadFtpFilesClass;
using FluentFTP;
using WriteLog;

namespace FtpLoopDownloadClass
{
    internal class FtpLoopDownload
    {
        /// <summary>
        /// Starts the FTP loop download process.
        /// </summary>
        /// <param name="ftpConnectToken">FTP login link token.</param>
        /// <param name="ftpFolderPath">The path to customers FTP folder in the config file.</param>
        /// <param name="downloadFolderPath">Local downnload folder path.</param>
        /// <param name="clientId">Clients ID retrived from the config file.</param>
        /// <returns>Returns the result as an integer.</returns>
        public static async Task<int> StartFtpLoopDownload(AsyncFtpClient ftpConnectToken,
                                                   string ftpFolderPath,
                                                   string downloadFolderPath,
                                                   int clientId)
        {
            try
            {
                // Starts the file download process.
                return await DownloadFolderInLoop(ftpConnectToken,
                                              ftpFolderPath,
                                              downloadFolderPath,
                                              clientId);
            }
            catch (Exception ex)
            {
                WriteLogClass.WriteToLog(0, $"Exception at StartFtpLoopDownload: {ex.Message}", 0);
                return -1;
            }
        }

        /// <summary>
        /// Start the download of files in a loop untill ite reaches end of the folder list.
        /// </summary>
        /// <param name="ftpConnectToken">FTP connection token.</param>
        /// <param name="ftpFolderPath">The path to customers FTP folder in the config file.</param>
        /// <param name="downloadFolderPath">Local downnload folder path.</param>
        /// <param name="clientId">Clients ID retrived from the config file.</param>
        /// <returns>Returns the result as an integer.</returns>
        public static async Task<int> DownloadFolderInLoop(AsyncFtpClient ftpConnectToken,
                                                  string ftpFolderPath,
                                                  string downloadFolderPath,
                                                  int clientId)
        {
            int result = -1;
            try
            {
                // Gets the FTP folder list.
                IEnumerable<FtpListItem> ftpFolderList = await ftpConnectToken.GetListing(ftpFolderPath);

                // Gets the folders that have files in them.
                List<string> ftpFoldersNotEmptyList = new();

                foreach (var folder in ftpFolderList.Where(fl => fl.Type == FtpObjectType.Directory))
                {
                    IEnumerable<FtpListItem> folderContents = await ftpConnectToken.GetListing(folder.FullName);

                    if (folderContents.Any(item => item.Type == FtpObjectType.File))
                    {
                        ftpFoldersNotEmptyList.Add(folder.FullName);
                    }
                }

                // If there are no folders with files in them then returns early terminating the execution.
                if (!ftpFoldersNotEmptyList.Any())
                {
                    result = 4;
                }

                // Downloads the files in the folders that have files in them.
                foreach (string ftpFolder in ftpFoldersNotEmptyList)
                {
                    string downloadFolder = Path.Combine(downloadFolderPath, Path.GetFileName(ftpFolder));
                    result = await FtpFilesDownload.DownloadFtpFilesFunction(ftpConnectToken,
                                                                             ftpFolder,
                                                                             downloadFolder,
                                                                             clientId);
                }
                
                return result;
            }
            catch (Exception ex)
            {
                WriteLogClass.WriteToLog(0, $"Exception at GetFtpFolderList: {ex.Message}", 0);
                return -1;
            }
        }
    }
}
