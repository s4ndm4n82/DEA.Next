using DEA.Next.HelperClasses.OtherFunctions;
using FluentFTP;
using FolderCleaner;
using WriteLog;
using WriteNamesToLog;

namespace DEA.Next.FileOperations.TpsServerReponseFunctions
{
    internal class TpsServerOnSuccess
    {
        public static async Task<int> ServerOnSuccessProjectAsync(string projectId,
                                                                  string queue,
                                                                  int fileCount,
                                                                  string deliveryType,
                                                                  string fullFilePath,
                                                                  string downloadFolderPath,
                                                                  string[] jsonFileList,
                                                                  int customerId,
                                                                  string clientOrgNo,
                                                                  AsyncFtpClient ftpConnect,
                                                                  string[] ftpFileList,
                                                                  string[] localFileList)
        {
            try
            {
                WriteLogClass.WriteToLog(1, $"Uploaded {fileCount} file to project {projectId} using queue {queue} ....", 4);
                WriteLogClass.WriteToLog(1, $"Uploaded filenames: {WriteNamesToLogClass.GetFileNames(jsonFileList)}", 4);

                // This will run if it's not FTP.
                if (deliveryType == MagicWords.email)
                {
                    if (!await FolderCleanerClass.GetFolders(fullFilePath, jsonFileList, null, clientOrgNo, MagicWords.email))
                    {
                        return -1;
                    }
                }
                else if (deliveryType == MagicWords.ftp)
                {
                    if (!await FolderCleanerClass.StartFtpFileDelete(ftpConnect, ftpFileList, localFileList))
                    {
                        return -1;
                    }

                    // Deletes the file from local hold folder when sending is successful.
                    if (!await FolderCleanerClass.GetFolders(downloadFolderPath, jsonFileList, customerId, null, MagicWords.ftp))
                    {
                        return -1;
                    }
                }
                return 1;
            }
            catch (Exception ex)
            {
                WriteLogClass.WriteToLog(0, $"Exception at ServerOnSuccess: {ex.Message}", 0);
                return -1;
            }
        }

        public static async Task<int> ServerOnSuccessDataFileAsync(AsyncFtpClient ftpConnect,
                                                                   int customerId,
                                                                   string fileName,
                                                                   string downloadFolderPath,
                                                                   string[] ftpFileList,
                                                                   string[] localFileList)
        {
            try
            {
                WriteLogClass.WriteToLog(1, $"uploaded data file: {fileName}", 1);

                string[] jsonFileList = new string[] { fileName };

                if (!await FolderCleanerClass.StartFtpFileDelete(ftpConnect, ftpFileList, localFileList))
                {
                    return -1;
                }

                if (!await FolderCleanerClass.GetFolders(downloadFolderPath, jsonFileList, customerId, null, MagicWords.ftp))
                {
                    return -1;
                }
                return 1;
            }
            catch (Exception ex)
            {
                WriteLogClass.WriteToLog(0, $"Exception at ServerOnSuccessDataFileAsync: {ex.Message}", 0);
                return -1;
            }
        }
    }
}
