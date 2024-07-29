using DEA.Next.FileOperations.TpsJsonStringCreatorFunctions;
using UserConfigRetriverClass;
using WriteLog;
using Directory = Microsoft.Graph.Directory;

namespace DEA.Next.FileOperations.TpsFileFunctions
{
    internal static class SendToWebServiceWithLines
    {
        public static async Task<int> SendToWebServiceWithLinesAsync(List<Dictionary<string, string>>? data,
            List<string>? valuesList,
            string[]? mainFieldList,
            string[]? lineFieldList,
            string mainFileName,
            string localFilePath,
            string setId,
            int clientId)
        {
            try
            {
                var jsonData = await UserConfigRetriver.RetriveUserConfigById(clientId);
                
                var trigger = mainFileName
                    .Contains(jsonData.ReadContentSettings.ReadByLineTrigger, StringComparison.OrdinalIgnoreCase);

                if (jsonData.ReadContentSettings.ReadByLine
                    && trigger
                    && valuesList!= null
                    && mainFieldList != null
                    && lineFieldList != null)
                    return await SendToWebServiceAsLine(valuesList,
                        mainFieldList,
                        lineFieldList,
                        mainFileName,
                        localFilePath,
                        setId,
                        clientId);

                return await SendToWebServiceAsBatch(data,
                    mainFileName,
                    localFilePath,
                    setId,
                    clientId);
            }
            catch (Exception ex)
            {
                WriteLogClass.WriteToLog(0, $"Exception at Sending to TPS: {ex.Message}", 0);
                return -1;
            }
        }
        
        private static async Task<int> SendToWebServiceAsLine(List<string> valuesList,
            string[] mainFieldList,
            string[] lineFieldList,
            string mainFileName,
            string localFilePath,
            string stepId,
            int clientId)
        {
            try
            {
                var jsonData = await UserConfigRetriver.RetriveUserConfigById(clientId);
                var fileExtension = Path.GetExtension(localFilePath);
                var acceptedExtensions = string.Concat(".", jsonData.ReadContentSettings.OutputFileExtension);
                var result = -1;
            }
            catch (Exception ex)
            {
                WriteLogClass.WriteToLog(0, $"Exception at Send To Web Service As Line: {ex.Message}", 0);
                return -1;
            }
            return 1;
        }

        private static async Task<int> SendToWebServiceAsBatch(List<Dictionary<string, string>>? data,
            string mainFileName,
            string localFilePath,
            string setId,
            int clientId)
        {
            try
            {
                var jsonData = await UserConfigRetriver.RetriveUserConfigById(clientId);
                var fileExtension = Path.GetExtension(localFilePath);
                var acceptedExtensions = string.Concat(".", jsonData.ReadContentSettings.OutputFileExtension);
                var result = -1;

                if (fileExtension == acceptedExtensions && data.Any())
                {
                   return await MakeJsonRequestLinesFunction.MakeJsonRequestBatch(data,
                       mainFileName,
                       localFilePath,
                       setId,
                       clientId);
                }
                
                WriteLogClass.WriteToLog(0, "No files to upload to TPS.", 1);
                return -1;
            }
            catch (Exception e)
            {
                WriteLogClass.WriteToLog(0, $"Exception at Sending to TPS: {e.Message}", 0);
                return -1;
            }
        }
    }
}
