using DEA.Next.FileOperations.TpsFileFunctions;
using FileFunctions;
using FluentFTP;
using UserConfigRetriverClass;
using UserConfigSetterClass;

namespace UploadFtpFilesClass
{
    internal class FtpFilesUpload
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
                                                          string[] fileNames,
                                                          string ftpFolderName,
                                                          int clientId)
        {
            UserConfigSetter.Customerdetail customerdetail = await UserConfigRetriver.RetriveUserConfigById(clientId);

            string[] matchingFileNames = currentBatch
                                         .Where(batchFile => fileNames
                                         .Any(fileName => Path.GetFileNameWithoutExtension(batchFile)
                                         .Equals(
                                             Path.GetFileNameWithoutExtension(fileName),
                                             StringComparison.OrdinalIgnoreCase)))
                                         .ToArray();

            string[] localFiles = Directory.GetFiles(ftpHoldFolder, "*.*", SearchOption.TopDirectoryOnly);

            if (!string.IsNullOrWhiteSpace(customerdetail.ProjetID))
            {
                return await SendToWebServiceProject.SendToWebServiceProjectAsync(ftpConnect,
                                                                  ftpHoldFolder,
                                                                  clientId,
                                                                  matchingFileNames,
                                                                  localFiles,
                                                                  ftpFolderName,
                                                                  null!);
            }

            return await SendToWebServiceDataFile.SendToWebServiceDataFileAsync(ftpConnect,
                                                                    clientId,
                                                                    ftpHoldFolder,
                                                                    ftpFolderName,
                                                                    matchingFileNames,
                                                                    localFiles);
        }
    }
}
