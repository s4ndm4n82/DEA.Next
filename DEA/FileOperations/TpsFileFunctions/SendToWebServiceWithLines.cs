using DEA.Next.FileOperations.TpsJsonStringCreatorFunctions;
using UserConfigRetriverClass;
using WriteLog;

namespace DEA.Next.FileOperations.TpsFileFunctions
{
    internal class SendToWebServiceWithLines
    {
        public static async Task<int> SendToWebServiceWithLinesAsync(string mainFileName,
                                                                     string localFilePath,
                                                                     int clientId)
        {
            var jsonData = await UserConfigRetriver.RetriveUserConfigById(clientId);
            var acceptedExtensions = jsonData.DocumentDetails.DocumentExtensions;
            
            // Create the file list of the downloaded files.
            var localFileList = SendToWebServiceHelpertFunctions.MakeLocalFileList(localFilePath,
                acceptedExtensions);

            if (localFileList.Any())
            {
                return await MakeJsonRequestLinesFunction.MakeJsonRequestLines(mainFileName,
                    clientId,
                    localFileList);
            }
            
            WriteLogClass.WriteToLog(0, "No files to upload to TPS.", 1);
            return -1;

        }
    }
}
