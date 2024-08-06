using DEA.Next.FileOperations.TpsJsonStringCreatorFunctions;
using UserConfigRetriverClass;
using WriteLog;

namespace DEA.Next.FileOperations.TpsFileFunctions
{
    internal static class SendToWebServiceWithLines
    {
        public static async Task<int> SendToWebServiceAsync(List<Dictionary<string, string>>? data,
            string newInvoiceNumber,
            string mainFileName,
            string localFilePath,
            string setId,
            bool lastItem,
            int clientId)
        {
            try
            {
                var jsonData = await UserConfigRetriver.RetriveUserConfigById(clientId);
                
                var trigger = mainFileName
                    .Contains(jsonData.ReadContentSettings.ReadByLineTrigger, StringComparison.OrdinalIgnoreCase);

                if (trigger)
                    return await SendToWebServiceAsLine(data,
                        newInvoiceNumber,
                        mainFileName,
                        localFilePath,
                        setId,
                        lastItem,
                        clientId);

                return await SendToWebServiceAsBatch(data,
                    newInvoiceNumber,
                    mainFileName,
                    localFilePath,
                    setId,
                    lastItem,
                    clientId);
            }
            catch (Exception ex)
            {
                WriteLogClass.WriteToLog(0, $"Exception at Sending to TPS: {ex.Message}", 0);
                return -1;
            }
        }
        
        private static async Task<int> SendToWebServiceAsLine(List<Dictionary<string, string>>? data,
            string newInvoiceNumber,
            string mainFileName,
            string localFilePath,
            string setId,
            bool lastItem,
            int clientId)
        {
            var result = -1;
            try
            {
                var jsonData = await UserConfigRetriver.RetriveUserConfigById(clientId);
                var fileExtension = Path.GetExtension(localFilePath);
                var acceptedExtensions = string.Concat(".", jsonData.ReadContentSettings.OutputFileExtension);

                if (fileExtension == acceptedExtensions && data != null)
                {
                    result = await MakeJsonRequestLinesFunction.MakeJsonRequestByLine(data,
                        newInvoiceNumber,
                        mainFileName,
                        localFilePath,
                        setId,
                        lastItem,
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
            string newInvoiceNumber,
            string mainFileName,
            string localFilePath,
            string setId,
            bool lastItem,
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
                       newInvoiceNumber,
                       mainFileName,
                       localFilePath,
                       setId,
                       lastItem,
                       clientId);
                }

                if (result == 1)
                {
                    WriteLogClass.WriteToLog(1, "Files uploaded successfully ....", 1);
                    return result;
                }
                
                WriteLogClass.WriteToLog(0, "No files to upload ....", 1);
                return result;
            }
            catch (Exception e)
            {
                WriteLogClass.WriteToLog(0, $"Exception at Sending to TPS: {e.Message}", 0);
                return result;
            }
        }
    }
}
