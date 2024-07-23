using DEA.Next.FileOperations.TpsJsonStringCreatorFunctions;
using UserConfigRetriverClass;
using WriteLog;

namespace DEA.Next.FileOperations.TpsFileFunctions
{
    internal static class SendToWebServiceWithLines
    {
        public static async Task<int> SendToWebServiceWithLinesAsync(List<Dictionary<string, string>> data,
            string mainFileName,
            string localFilePath,
            string setId,
            int clientId)
        {
            try
            {
                var jsonData = await UserConfigRetriver.RetriveUserConfigById(clientId);
                var acceptedExtensions = string.Concat(".", jsonData.ReadContentSettings.OutputFileExtension);
            
                // Create the file list of the downloaded files.
                /*var localFileList = SendToWebServiceHelpertFunctions.MakeLocalFileList(localFilePath,
                    acceptedExtensions);*/

                if (File.Exists(localFilePath) && data.Any())
                {
                    return await MakeJsonRequestLinesFunction.MakeJsonRequestLines(data,
                        mainFileName,
                        localFilePath,
                        setId,
                        clientId);
                }
            
                WriteLogClass.WriteToLog(0, "No files to upload to TPS.", 1);
                return -1;
            }
            catch (Exception ex)
            {
                WriteLogClass.WriteToLog(0, $"Exception at Sending to TPS: {ex.Message}", 0);
                return -1;
            }
        }
    }
}
