using DEA.Next.FileOperations.TpsJsonStringCreatorFunctions;
using UserConfigRetriverClass;
using WriteLog;

namespace DEA.Next.FileOperations.TpsFileFunctions
{
    internal static class SendToWebServiceWithLines
    {
        /// <summary>
        /// Sends data to a web service asynchronously. The data can be sent as a batch or line by line, depending on the trigger string in the main file name.
        /// </summary>
        /// <param name="data">The list of dictionaries containing the data to be sent to the web service.</param>
        /// <param name="newInvoiceNumber">The new invoice number.</param>
        /// <param name="mainFileName">The name of the main file.</param>
        /// <param name="localFilePath">The local file path.</param>
        /// <param name="setId">The set ID.</param>
        /// <param name="lastItem">Indicates whether this is the last item in the batch.</param>
        /// <param name="clientId">The client ID.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an integer indicating the result of the operation. Returns -1 if an exception occurs.</returns>
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
                // Retrieve user configuration
                var jsonData = await UserConfigRetriver.RetriveUserConfigById(clientId);

                // Check if the main file name contains the trigger string for line by line processing
                var trigger = mainFileName
                    .Contains(jsonData.ReadContentSettings.ReadByLineTrigger, StringComparison.OrdinalIgnoreCase);

                if (trigger)
                {
                    // Send data line by line
                    return await SendToWebServiceAsLine(data,
                        newInvoiceNumber,
                        mainFileName,
                        localFilePath,
                        setId,
                        lastItem,
                        clientId);
                }

                // Send data as a batch
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
                // Log exception and return -1
                WriteLogClass.WriteToLog(0, $"Exception at Sending to TPS: {ex.Message}", 0);
                return -1;
            }
        }

        /// <summary>
        /// Sends the data to a web service asynchronously, line by line.
        /// </summary>
        /// <param name="data">The list of data items to send.</param>
        /// <param name="newInvoiceNumber">The new invoice number.</param>
        /// <param name="mainFileName">The main file name.</param>
        /// <param name="localFilePath">The local file path.</param>
        /// <param name="setId">The set ID.</param>
        /// <param name="lastItem">Indicates whether this is the last item in the batch.</param>
        /// <param name="clientId">The client ID.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the result of the operation.</returns>
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
                // Retrieve user configuration
                var jsonData = await UserConfigRetriver.RetriveUserConfigById(clientId);

                // Check if the file extension is accepted
                var fileExtension = Path.GetExtension(localFilePath);
                var acceptedExtensions = string.Concat(".", jsonData.ReadContentSettings.OutputFileExtension);
                if (fileExtension == acceptedExtensions && data != null)
                {
                    // Send data to web service line by line
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
                // Log any exceptions that occur during the process
                WriteLogClass.WriteToLog(0, $"Exception at Send To Web Service As Line: {ex.Message}", 0);
                return result;
            }

            return result;
        }

        /// <summary>
        /// Sends data to a web service as a batch.
        /// </summary>
        /// <param name="data">The list of data to send.</param>
        /// <param name="newInvoiceNumber">The new invoice number.</param>
        /// <param name="mainFileName">The main file name.</param>
        /// <param name="localFilePath">The local file path.</param>
        /// <param name="setId">The set ID.</param>
        /// <param name="lastItem">Indicates whether this is the last item in the batch.</param>
        /// <param name="clientId">The client ID.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the result of the operation.</returns>
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
                // Retrieve user configuration
                var jsonData = await UserConfigRetriver.RetriveUserConfigById(clientId);

                // Check if the file extension is accepted
                var fileExtension = Path.GetExtension(localFilePath);
                var acceptedExtensions = string.Concat(".", jsonData.ReadContentSettings.OutputFileExtension);

                if (data != null && fileExtension == acceptedExtensions)
                {
                    // Send data to web service as a batch
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
                    // Log successful upload
                    WriteLogClass.WriteToLog(1, "Files uploaded successfully ....", 1);
                    return result;
                }

                // Log no files to upload
                WriteLogClass.WriteToLog(0, "No files to upload ....", 1);
                return result;
            }
            catch (Exception e)
            {
                // Log any exceptions that occur during the process
                WriteLogClass.WriteToLog(0, $"Exception at Sending to TPS: {e.Message}", 0);
                return result;
            }
        }
    }
}