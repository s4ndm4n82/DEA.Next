﻿using DEA.Next.FileOperations.TpsJsonStringCreatorFunctions;
using FluentFTP;
using UserConfigRetriverClass;
using UserConfigSetterClass;
using WriteLog;

namespace DEA.Next.FileOperations.TpsFileFunctions
{
    /// <summary>
    /// This function will start the data file upload process.
    /// </summary>
    internal class SendToWebServiceDataFile
    {
        public static async Task<int> SendToWebServiceDataFileAsync(AsyncFtpClient ftpConnect,
                                                                    int customerId,
                                                                    string localFolderPath,
                                                                    string[] ftpFileList,
                                                                    string[] localFileList)
        {
            try
            {
                WriteLogClass.WriteToLog(1, "Starting file upload process .... ", 4);

                UserConfigSetter.Customerdetail customerDetails = await UserConfigRetriver.RetriveUserConfigById(customerId);

                // Creat the local file list.
                string[] downloadedFileList = SendToWebServiceHelpertFunctions.MakeDownloadedFileList(customerDetails,
                                                                                                      localFolderPath,
                                                                                                      ftpFileList);

                if (!downloadedFileList.Any())
                {
                    WriteLogClass.WriteToLog(1, "No matching files in the download list ....", 1); // No matching files in the download lis
                    return -1;
                }

                return await MakeJsonRequestDataFileFunction.MakeJsonRequestDataFileAsync(ftpConnect,
                                                                                          customerId,
                                                                                          localFolderPath,
                                                                                          downloadedFileList,
                                                                                          ftpFileList,
                                                                                          localFileList);
            }
            catch (Exception ex)
            {
                WriteLogClass.WriteToLog(0, $"Exception at SendToWebServiceDataFile: {ex.Message}", 0);
                return -1;
            }
        }
    }
}