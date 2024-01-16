using FluentFTP;

namespace DEA.Next.FileOperations.TpsFileFunctions
{
    internal class SendToWebServiceDataFile
    {
        public static async Task<int> SendToWebServiceDataFileAsync(AsyncFtpClient ftpConnect,
                                                                    int customerId,
                                                                    string localFolderPath,
                                                                    string ftpFolderName,
                                                                    string[] ftpFileList,
                                                                    string[] localFileList)
        {
            try
            {

            }
            catch (Exception ex)
            {

            }
        }
    }
}
