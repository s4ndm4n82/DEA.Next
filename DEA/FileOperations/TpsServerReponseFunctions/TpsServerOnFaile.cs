using DEA.Next.HelperClasses.OtherFunctions;
using FluentFTP;
using FolderCleaner;
using HandleErrorFiles;
using System.Net;
using WriteLog;

namespace DEA.Next.FileOperations.TpsServerReponseFunctions
{
    /// <summary>
    /// Handles the operations after a failed TPS server response.
    /// </summary>
    internal class TpsServerOnFaile
    {
        /// <summary>
        /// Hadles the normal project upload.
        /// </summary>
        /// <param name="deliveryType"></param>
        /// <param name="fullFilePath"></param>
        /// <param name="customerId"></param>
        /// <param name="clientOrgNo"></param>
        /// <param name="ftpConnect"></param>
        /// <param name="ftpFileList"></param>
        /// <param name="localFileList"></param>
        /// <param name="serverStatusCode"></param>
        /// <param name="serverResponseContent"></param>
        /// <returns></returns>
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

                // Moves the files to the error folder. Assumes the files was not uploaded.
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

                    if (!FolderCleanerClass.DeleteEmptyFolders(Path.GetDirectoryName(fullFilePath)))
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

        /// <summary>
        /// Handles the data file upload.
        /// </summary>
        /// <param name="ftpConnect"></param>
        /// <param name="customerId"></param>
        /// <param name="downloadFilePath"></param>
        /// <param name="serverresponseContent"></param>
        /// <param name="ftpFileList"></param>
        /// <param name="localFileList"></param>
        /// <param name="statusCode"></param>
        /// <returns></returns>
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

                // Move the file to error folder. Assuming file was not uploaded.
                if (!await HandleErrorFilesClass.MoveFilesToErrorFolder(downloadFilePath, ftpFileList, customerId, string.Empty))
                {
                    WriteLogClass.WriteToLog(0, "Moving files failed ....", 0);
                    return -1;
                }

                // Remove the files from FTP server.
                if (!await FolderCleanerClass.StartFtpFileDelete(ftpConnect, ftpFileList, localFileList))
                {
                    WriteLogClass.WriteToLog(0, "Deleting files from FTP server failed ....", 0);
                    return 0;
                }

                // Remove the files from the local folder.
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
