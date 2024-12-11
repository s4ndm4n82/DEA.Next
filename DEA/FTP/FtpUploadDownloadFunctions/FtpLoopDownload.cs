using DownloadFtpFilesClass;
using FluentFTP;
using Renci.SshNet;
using Renci.SshNet.Sftp;
using WriteLog;

namespace FtpLoopDownloadClass;

internal class FtpLoopDownload
{
    /// <summary>
    /// Starts the FTP loop download process.
    /// </summary>
    /// <param name="ftpConnectToken">FTP login link token.</param>
    /// <param name="sftpConnectToken"></param>
    /// <param name="ftpFolderPath">The path to customers FTP folder in the config file.</param>
    /// <param name="downloadFolderPath">Local download folder path.</param>
    /// <param name="clientId">Clients ID retrieved from the config file.</param>
    /// <returns>Returns the result as an integer.</returns>
    public static async Task<int> StartFtpLoopDownload(AsyncFtpClient? ftpConnectToken,
        SftpClient? sftpConnectToken,
        string ftpFolderPath,
        string downloadFolderPath,
        Guid clientId)
    {
        try
        {
            // Starts the file download process.
            return await DownloadFolderInLoop(ftpConnectToken,
                sftpConnectToken,
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
    /// <param name="sftpConnectToken"></param>
    /// <param name="ftpFolderPath">The path to customers FTP folder in the config file.</param>
    /// <param name="downloadFolderPath">Local download folder path.</param>
    /// <param name="clientId">Clients ID retrieved from the config file.</param>
    /// <returns>Returns the result as an integer.</returns>
    public static async Task<int> DownloadFolderInLoop(AsyncFtpClient? ftpConnectToken,
        SftpClient? sftpConnectToken,
        string ftpFolderPath,
        string downloadFolderPath,
        Guid clientId)
    {
        var result = -1;
        try
        {
            // Gets the FTP folder list.
            var ftpFolderPathNotEmptyList = await GetFtpFolderListAsync(ftpConnectToken,
                sftpConnectToken,
                ftpFolderPath);
            // If there are no folders with files in them then returns early terminating the execution.
            if (ftpFolderPathNotEmptyList.Count == 0)
            {
                result = 4;
            }

            // Downloads the files in the folders that have files in them.
            foreach (var ftpFolderPathNotEmpty in ftpFolderPathNotEmptyList)
            {
                var downloadFolder = Path.Combine(downloadFolderPath, Path.GetFileName(ftpFolderPathNotEmpty));
                var ftpFolderName = Path.GetFileName(ftpFolderPathNotEmpty);

                result = await FtpFilesDownload.DownloadFtpFilesFunction(ftpConnectToken,
                    sftpConnectToken,
                    ftpFolderPathNotEmpty,
                    downloadFolder,
                    ftpFolderName,
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

    /// <summary>
    /// Gets the FTP folder list.
    /// </summary>
    /// <param name="ftpConnectToken">FTP connection token.</param>
    /// <param name="sftpConnectToken"></param>
    /// <param name="ftpFolderPath">The path to customers FTP folder in the config file.</param>
    /// <returns>Returns the FTP folder list.</returns>
    private static async Task<List<string>> GetFtpFolderListAsync(AsyncFtpClient? ftpConnectToken,
        SftpClient? sftpConnectToken,
        string ftpFolderPath)
    {
        try
        {
            // Gets the FTP/SFTP folder list.
            if (ftpConnectToken != null)
            {
                return await MakeFtpFolderList(ftpConnectToken, ftpFolderPath);
            }

            if (sftpConnectToken != null)
            {
                return await MakeSftpFolderList(sftpConnectToken, ftpFolderPath);
            }

            WriteLogClass.WriteToLog(0, $"Ftp or Sftp is not connected ....", 1);
            throw new InvalidOperationException("Ftp or Sftp is not connected");
        }
        catch (Exception ex)
        {
            WriteLogClass.WriteToLog(0, $"Exception at GetFtpFolderList: {ex.Message}", 0);
            throw new InvalidOperationException("Ftp or Sftp is not connected");
        }
    }

    private static async Task<List<string>> MakeFtpFolderList(AsyncFtpClient? ftpConnectToken,
        string ftpFolderPath)
    {
        try
        {
            if (ftpConnectToken == null)
            {
                WriteLogClass.WriteToLog(0, $"Ftp is not connected ....", 1);
                throw new InvalidOperationException("Ftp or Sftp is not connected");
            }

            // Initiate the FTP folder list variable.
            IEnumerable<FtpListItem> ftpFolderList = await ftpConnectToken.GetListing(ftpFolderPath);

            // Check if Ftp folder list is null.
            if (ftpFolderList == null)
            {
                WriteLogClass.WriteToLog(0, $"Ftp folder list is null ....", 1);
                throw new InvalidOperationException("Ftp or Sftp is not connected");
            }

            // Gets the folders that have files in them.
            List<string> ftpFolderPathNotEmptyList = new();

            foreach (var folder in ftpFolderList.Where(fl => fl.Type == FtpObjectType.Directory))
            {
                var folderContents = await ftpConnectToken.GetListing(folder.FullName);

                if (folderContents.Any(item => item.Type == FtpObjectType.File))
                {
                    ftpFolderPathNotEmptyList.Add(folder.FullName);
                }
            }

            return ftpFolderPathNotEmptyList;
        }
        catch (Exception ex)
        {
            WriteLogClass.WriteToLog(0, $"Exception at GetFtpFolderList: {ex.Message}", 0);
            throw new InvalidOperationException("Ftp or Sftp is not connected");
        }
    }

    private static async Task<List<string>> MakeSftpFolderList(SftpClient? sftpConnectToken,
        string sftpFolderPath)
    {
        try
        {
            if (sftpConnectToken == null)
            {
                WriteLogClass.WriteToLog(0, $"Sftp is not connected ....", 1);
                throw new InvalidOperationException("Ftp or Sftp is not connected");
            }

            // Initiate the SFTP folder list variable.
            var sftpFolderList = sftpConnectToken.ListDirectory(sftpFolderPath).ToList();

            // Check if Sftp folder list is null.
            if (sftpFolderList.Count == 0)
            {
                WriteLogClass.WriteToLog(0, $"Sftp folder list is null ....", 1);
                throw new InvalidOperationException("Ftp or Sftp is not connected");
            }

            // Gets the folders that have files in them.
            List<string> sftpFolderPathNotEmptyList = [];
            sftpFolderPathNotEmptyList.AddRange(from folder in sftpFolderList
                .Where(fl => fl.IsDirectory)
                let folderContents = sftpConnectToken
                    .ListDirectory(folder.FullName)
                where folder.Name != "." && folder.Name != ".."
                where folderContents.Any(item => item.IsRegularFile)
                select folder.FullName);

            return sftpFolderPathNotEmptyList;
        }
        catch (Exception ex)
        {
            WriteLogClass.WriteToLog(0, $"Exception at GetSftpFolderList: {ex.Message}", 0);
            return null;
        }
    }
}