using DEA.Next.HelperClasses.OtherFunctions;
using FluentFTP;
using FolderCleaner;
using GetMailFolderIds;
using GraphMoveEmailsrClass;
using HandleErrorFiles;
using Microsoft.Graph;
using System.Net;
using UserConfigRetriverClass;
using UserConfigSetterClass;
using WriteLog;
using Directory = System.IO.Directory;

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
                int deleteResult = 2;

                UserConfigSetter.Ftpdetails ftpDetails = await UserConfigRetriver.RetriveFtpConfigById(customerId);

                WriteLogClass.WriteToLog(0, $"Server status code: {serverStatusCode}, Server Response Error: {serverResponseContent}", 0);

                // Moves the files to the error folder. Assumes the files was not uploaded.
                if (ftpDetails.FtpMoveToSubFolder == false && !await HandleErrorFilesClass.MoveFilesToErrorFolder(fullFilePath,
                                                                                                                  ftpFileList,
                                                                                                                  customerId,
                                                                                                                  clientOrgNo))
                {
                    WriteLogClass.WriteToLog(0, "Moving files failed ....", 3);
                    return -1;
                }

                // This will run if it's not FTP.
                if (deliveryType == MagicWords.email && await FolderCleanerClass.GetFolders(fullFilePath,
                                                                                            ftpFileList,
                                                                                            null,
                                                                                            clientOrgNo,
                                                                                            MagicWords.email))
                {
                    return 0;
                }

                if (deliveryType == MagicWords.ftp)
                {
                    // Delete the files from FTP server.
                    if (ftpDetails.FtpMoveToSubFolder == false && !await FolderCleanerClass.StartFtpFileDelete(ftpConnect,
                                                                                                               null,
                                                                                                               ftpFileList,
                                                                                                               localFileList))
                    {
                        WriteLogClass.WriteToLog(0, "Deleting files from FTP server failed ....", 1);
                        return 0;
                    }

                    // Deleting the files from local.
                    if (ftpDetails.FtpMoveToSubFolder == true && !FolderCleanerClass.DeleteFiles(Path.GetDirectoryName(fullFilePath), ftpFileList))
                    {
                        WriteLogClass.WriteToLog(0, "Deleting files failed ....", 1);
                        return 0;
                    }

                    // Cehcking the folder is empty or not.
                    IEnumerable<string> fileList = Directory.EnumerateFiles(Path.GetDirectoryName(fullFilePath), "*", SearchOption.AllDirectories);
                    // Return if the folder is not empty.
                    if (fileList.Any())
                    {
                        return 0;
                    }

                    // Deleting the empty folders.
                    if (!FolderCleanerClass.DeleteEmptyFolders(Path.GetDirectoryName(fullFilePath)))
                    {
                        WriteLogClass.WriteToLog(0, "Deleting empty folders failed ....", 1);
                        return 0;
                    }
                }

                return ftpDetails.FtpMoveToSubFolder == true ? deleteResult = 6 : deleteResult;
            }
            catch (Exception ex)
            {
                WriteLogClass.WriteToLog(0, $"Exception at ServerOnFailProjectsAsync: {ex.Message}", 0);
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
                    WriteLogClass.WriteToLog(0, "Moving files failed ....", 3);
                    return -1;
                }

                // Remove the files from FTP server.
                if (!await FolderCleanerClass.StartFtpFileDelete(ftpConnect,
                                                                 null,
                                                                 ftpFileList,
                                                                 localFileList))
                {
                    WriteLogClass.WriteToLog(0, "Deleting files from FTP server failed ....", 3);
                    return 0;
                }

                // Remove the files from the local folder.
                if (!FolderCleanerClass.DeleteEmptyFolders(downloadFilePath))
                {
                    WriteLogClass.WriteToLog(0, "Deleting empty folders failed ....", 3);
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

        /// <summary>
        /// Handles the on fail for body text upload.
        /// </summary>        
        public static async Task<int> ServerOnFailBodyTextAsync(IMailFolderRequestBuilder requestBuilder,
                                                                string messageId,
                                                                string serverResponseContent,
                                                                HttpStatusCode serverStatusCode)
        {
            string errorFolderId = await GetMailFolderIdsClass.GetErrorFolderId(requestBuilder);

            try
            {
                if (!await GraphMoveEmailsFolder.MoveEmailsToAnotherFolder(requestBuilder,
                                                                           messageId,
                                                                           errorFolderId))
                {
                    WriteLogClass.WriteToLog(0, "Moving email to error folder failed ....", 2);
                    return 2;
                }

                WriteLogClass.WriteToLog(0, $"Sending to server failed. Email moved to error folder.\nCode:{serverStatusCode}\nStatus:{serverResponseContent}", 2);
                return 2;
            }
            catch (Exception ex)
            {
                WriteLogClass.WriteToLog(0, $"Exception at ServerOnFailBodyTextAsync: {ex.Message}", 0);
                return 2;
            }
        }
    }
}
