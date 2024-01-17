using DEA.Next.HelperClasses.OtherFunctions;
using FluentFTP;
using FolderCleaner;
using HandleErrorFiles;
using System.Net;
using WriteLog;

namespace DEA.Next.FileOperations.TpsServerReponseFunctions
{
    internal class TpsServerOnFaile
    {
        public static async Task<int> ServerOnFailProjectsAsync(string deliveryType,
                                                                string fullFilePath,
                                                                int customerId,
                                                                string clientOrgNo,
                                                                AsyncFtpClient ftpConnect,
                                                                string[] ftpFileList,
                                                                string[] localFileList,
                                                                HttpStatusCode serverStatusCode,
                                                                string serverResponseContent)
        {
            try
            {
                WriteLogClass.WriteToLog(0, $"Server status code: {serverStatusCode}, Server Response Error: {serverResponseContent}", 0);

                if (!await HandleErrorFilesClass.MoveFilesToErrorFolder(fullFilePath, ftpFileList, customerId, clientOrgNo))
                {
                    WriteLogClass.WriteToLog(0, "Moving files failed ....", 0);
                    return -1;
                }

                // This will run if it's not FTP.
                if (deliveryType == MagicWords.email)
                {
                    if (await FolderCleanerClass.GetFolders(fullFilePath, ftpFileList, null, clientOrgNo, MagicWords.email))
                    {
                        return 2;
                    }
                }
                else if (deliveryType == MagicWords.ftp)
                {
                    if (!await FolderCleanerClass.StartFtpFileDelete(ftpConnect, ftpFileList, localFileList))
                    {
                        WriteLogClass.WriteToLog(0, "Deleting files from FTP server failed ....", 0);
                        return 0;
                    }

                    if (!FolderCleanerClass.DeleteEmptyFolders(fullFilePath))
                    {
                        WriteLogClass.WriteToLog(0, "Deleting empty folders failed ....", 0);
                        return 0;
                    }
                }
                return 2; // Deafult return
            }
            catch (Exception ex)
            {
                WriteLogClass.WriteToLog(1, $"Exception at ServerOnFailProjectsAsync: {ex.Message}", 1);
                return -1;
            }
        }

        public static async Task<int> ServerOnFailDataFileAsync(AsyncFtpClient ftpConnect,
                                                                int customerId,
                                                                string downloadFilePath,
                                                                string serverresponseContent,
                                                                string[] ftpFileList,
                                                                string[] localFileList,
                                                                HttpStatusCode statusCode)
        {
            try
            {
                WriteLogClass.WriteToLog(0, $"Server status code: {statusCode}, Server Response Error: {serverresponseContent}", 0);

                if (!await HandleErrorFilesClass.MoveFilesToErrorFolder(downloadFilePath, ftpFileList, customerId, string.Empty))
                {
                    WriteLogClass.WriteToLog(0, "Moving files failed ....", 0);
                    return -1;
                }

                if (!await FolderCleanerClass.StartFtpFileDelete(ftpConnect, ftpFileList, localFileList))
                {
                    WriteLogClass.WriteToLog(0, "Deleting files from FTP server failed ....", 0);
                    return 0;
                }

                if (!FolderCleanerClass.DeleteEmptyFolders(downloadFilePath))
                {
                    WriteLogClass.WriteToLog(0, "Deleting empty folders failed ....", 0);
                    return 0;
                }
                return 2; // Default return
            }
            catch (Exception ex)
            {
                WriteLogClass.WriteToLog(0, $"Exception at ServerOnFailDataFileAsync: {ex.Message}", 0);
                return -1;
            }
        }
    }
}
