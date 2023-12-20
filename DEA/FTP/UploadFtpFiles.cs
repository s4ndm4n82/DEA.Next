using FileFunctions;
using FluentFTP;

namespace UploadFtpFilesClass
{
    internal class UploadFtpFiles
    {
        /// <summary>
        /// File download function. This will be parsing the files to the FTP client and downloading them the the local folder.
        /// </summary>
        /// <param name="ftpConnect">FTP connection token.</param>
        /// <param name="currentBatch">Files downloaded from the server.</param>
        /// <param name="ftpHoldFolder">Local download folder.</param>
        /// <param name="clientId">Client ID.</param>
        /// <param name="downloadResult"></param>
        /// <returns></returns>
        public static async Task<int> FilesUploadFuntcion(AsyncFtpClient ftpConnect,
                                                          string[] currentBatch,
                                                          string ftpHoldFolder,
                                                          string fileName,
                                                          int clientId)
        {
            string[] matchingFileName = currentBatch.Where(f => Path.GetFileNameWithoutExtension(f)
                                                           .Equals(Path.GetFileNameWithoutExtension(fileName), StringComparison.OrdinalIgnoreCase))
                                                           .ToArray();
            /*IEnumerable<string> unmatchedFileList = FolderCleanerClass.CheckMissedFiles(ftpHoldFolder, currentBatch);
            if (unmatchedFileList.Any())
            {
                WriteLogClass.WriteToLog(1, $"Ftp file count: {currentBatch.Count()}, Local file count: {unmatchedFileList.Count()} files doesn't match", 3);
                return 3;
            }*/
            string[] localFiles = Directory.GetFiles(ftpHoldFolder, "*.*", SearchOption.TopDirectoryOnly);
            return await FileFunctionsClass.SendToWebService(ftpConnect, ftpHoldFolder, clientId, matchingFileName, localFiles, null!);
        }
    }
}
