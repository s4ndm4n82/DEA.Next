﻿using DEA.Next.HelperClasses.OtherFunctions;
using FluentFTP;
using FolderCleaner;
using FtpFunctions;
using GraphMoveEmailsToExportClass;
using Microsoft.Graph;
using Renci.SshNet;
using System.Net;
using UserConfigRetriverClass;
using UserConfigSetterClass;
using WriteLog;
using WriteNamesToLog;

namespace DEA.Next.FileOperations.TpsServerReponseFunctions
{
    /// <summary>
    /// Handles the operations after a successful TPS server response.
    /// </summary>
    internal class TpsServerOnSuccess
    {
        /// <summary>
        /// Normal project upload.
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="queue"></param>
        /// <param name="fileCount"></param>
        /// <param name="deliveryType"></param>
        /// <param name="fullFilePath"></param>
        /// <param name="downloadFolderPath"></param>
        /// <param name="jsonFileList"></param>
        /// <param name="customerId"></param>
        /// <param name="clientOrgNo"></param>
        /// <param name="ftpConnect"></param>
        /// <param name="ftpFileList"></param>
        /// <param name="localFileList"></param>
        /// <returns></returns>
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
                                                                  SftpClient sftpConnect,
                                                                  string[] ftpFileList,
                                                                  string[] localFileList)
        {
            try
            {
                UserConfigSetter.Ftpdetails ftpDetails = await UserConfigRetriver.RetriveFtpConfigById(customerId);

                WriteLogClass.WriteToLog(1, $"Uploaded {fileCount} file to project {projectId} using queue {queue} ....", 4);
                WriteLogClass.WriteToLog(1, $"Uploaded filenames: {WriteNamesToLogClass.GetFileNames(jsonFileList)}", 4);

                // This will run if it's not FTP.
                if (deliveryType == MagicWords.email)
                {
                    if (!await FolderCleanerClass.GetFolders(fullFilePath,
                                                             jsonFileList,
                                                             null,
                                                             clientOrgNo,
                                                             MagicWords.email))
                    {
                        return -1;
                    }
                }

                if (deliveryType == MagicWords.ftp)
                {
                    // Removes the files from FTP server. If the files not needed to be moved to a FTP sub folder.
                    if (ftpDetails.FtpMoveToSubFolder == false && !await FolderCleanerClass.StartFtpFileDelete(ftpConnect,
                                                                                                               sftpConnect,
                                                                                                               ftpFileList,
                                                                                                               localFileList))
                    {
                        WriteLogClass.WriteToLog(0, "Deleting files from FTP failed ....", 1);
                        return -1;
                    }

                    // Moving files to another FTP sub folder.
                    if (ftpDetails.FtpMoveToSubFolder == true && !await FtpFunctionsClass.MoveFtpFiles(ftpConnect,
                                                                                                       sftpConnect,
                                                                                                       customerId,
                                                                                                       ftpFileList))
                    {
                        WriteLogClass.WriteToLog(0, "Moving files to FTP sub folder failed ....", 1);
                        return -1;
                    }

                    // Deletes the file from local hold folder when sending is successful.
                    if (!await FolderCleanerClass.GetFolders(downloadFolderPath,
                                                             jsonFileList,
                                                             customerId,
                                                             null,
                                                             MagicWords.ftp))
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

        /// <summary>
        /// Data file upload.
        /// </summary>
        /// <param name="ftpConnect"></param>
        /// <param name="customerId"></param>
        /// <param name="fileName"></param>
        /// <param name="downloadFolderPath"></param>
        /// <param name="ftpFileList"></param>
        /// <param name="localFileList"></param>
        /// <returns></returns>
        public static async Task<int> ServerOnSuccessDataFileAsync(AsyncFtpClient ftpConnect,
                                                                   SftpClient sftpConnect,
                                                                   int customerId,
                                                                   string fileName,
                                                                   string downloadFolderPath,
                                                                   string[] ftpFileList,
                                                                   string[] localFileList)
        {
            try
            {
                WriteLogClass.WriteToLog(1, $"Uploaded data file: {fileName}", 1);

                // Converts the filename to an array. Needed by the FolderCleanerClass.
                string[] jsonFileList = new string[] { fileName };

                // Remove the files from FTP server.
                if (!await FolderCleanerClass.StartFtpFileDelete(ftpConnect,
                                                                 sftpConnect,
                                                                 ftpFileList,
                                                                 localFileList))
                {
                    return -1;
                }

                // Remove the files from the local folder.
                if (!await FolderCleanerClass.GetFolders(downloadFolderPath,
                                                         jsonFileList,
                                                         customerId,
                                                         null,
                                                         MagicWords.ftp))
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

        /// <summary>
        /// Sending body text to TPS server success.
        /// </summary>
        /// 
        public static async Task<int> ServerOnSuccessBodyTextAsync(IMailFolderRequestBuilder requestBuilder,
                                                                   string messageId,
                                                                   string messageSubject)
        {
            try
            {
                if (!await GraphMoveEmailsToExport.MoveEmailsToExport(requestBuilder,
                                                                     messageId,
                                                                     messageSubject))
                {
                    WriteLogClass.WriteToLog(0, $"Moving email {messageSubject} to export unsuccessful ....", 2);
                    return 2;
                }

                WriteLogClass.WriteToLog(1, $"Body text sent to syste. Moved email {messageSubject} to export successfuly ....", 2);
                return 1;
            }
            catch (Exception ex)
            {
                WriteLogClass.WriteToLog(0, $"Exception at ServerOnSuccessBodyTextAsync: {ex.Message}", 0);
                return 2;
            }
        }

    }
}
