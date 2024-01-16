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
        public static async Task<int> ServerOnFail(string deliveryType,
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
                    WriteLogClass.WriteToLog(1, "Moving files failed ....", 1);
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
                    if (await FolderCleanerClass.StartFtpFileDelete(ftpConnect, ftpFileList, localFileList))
                    {
                        if (FolderCleanerClass.DeleteEmptyFolders(fullFilePath))
                        {
                            return 2;
                        }
                    }
                }
                return 0; // Deafult return
            }
            catch (Exception ex)
            {
                WriteLogClass.WriteToLog(1, $"Error in ServerOnFail: {ex.Message}", 1);
                return -1;
            }
        }
    }
}
