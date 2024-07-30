using DEA.Next.FileOperations.TpsJsonStringCreatorFunctions;
using UserConfigRetriverClass;
using WriteLog;
using Directory = Microsoft.Graph.Directory;

namespace DEA.Next.FileOperations.TpsFileFunctions
{
    internal static class SendToWebServiceWithLines
    {
        public static async Task<int> SendToWebServiceAsync(List<Dictionary<string, string>>? data,
            List<string>? valuesList,
            string newInvoiceNumber,
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
                    && valuesList!= null)
                    return await SendToWebServiceAsLine(valuesList,
                        newInvoiceNumber,
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
            string newInvoiceNumber,
            string mainFileName,
            string localFilePath,
            string setId,
            int clientId)
        {
            var result = -1;
            try
            {
                var jsonData = await UserConfigRetriver.RetriveUserConfigById(clientId);
                var fileExtension = Path.GetExtension(localFilePath);
                var acceptedExtensions = string.Concat(".", jsonData.ReadContentSettings.OutputFileExtension);

                if (fileExtension == acceptedExtensions && valuesList.Any())
                {
                    result = await MakeJsonRequestLinesFunction.MakeJsonRequestByLine(valuesList,
                        newInvoiceNumber,
                        mainFileName,
                        localFilePath,
                        setId,
                        clientId);
                }
            }
            catch (Exception ex)
            {
                WriteLogClass.WriteToLog(0, $"Exception at Send To Web Service As Line: {ex.Message}", 0);
                return result;
            }
            return result;
        }

        private static async Task<int> SendToWebServiceAsBatch(List<Dictionary<string, string>>? data,
            string mainFileName,
            string localFilePath,
            string setId,
            int clientId)
        {
            var result = -1;
            try
            {
                var jsonData = await UserConfigRetriver.RetriveUserConfigById(clientId);
                var fileExtension = Path.GetExtension(localFilePath);
                var acceptedExtensions = string.Concat(".", jsonData.ReadContentSettings.OutputFileExtension);

                if (data != null && fileExtension == acceptedExtensions)
                {
                    result = await MakeJsonRequestLinesFunction.MakeJsonRequestBatch(data,
                       mainFileName,
                       localFilePath,
                       setId,
                       clientId);
                }
                
                WriteLogClass.WriteToLog(0, "No files to upload to TPS.", 1);
            }
            catch (Exception e)
            {
                WriteLogClass.WriteToLog(0, $"Exception at Sending to TPS: {e.Message}", 0);
                return result;
            }
            
            return result;
        }
    }
}
