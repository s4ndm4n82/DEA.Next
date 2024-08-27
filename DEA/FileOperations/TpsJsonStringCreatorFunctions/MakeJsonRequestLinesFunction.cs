using DEA.Next.FileOperations.TpsFileUploadFunctions;
using DEA.Next.FileOperations.TpsJsonStringClasses;
using DEA.Next.FileOperations.TpsServerResponseFunctions;
using DEA.Next.HelperClasses.ConfigFileFunctions;
using Newtonsoft.Json;
using UserConfigSetterClass;
using WriteLog;

namespace DEA.Next.FileOperations.TpsJsonStringCreatorFunctions
{
    internal static class MakeJsonRequestLinesFunction
    {
        /// <summary>
        /// This method creates a JSON request batch and sends it to an API.
        /// If the request is successful, it handles the success response.
        /// If the request fails, it handles the failure response.
        /// </summary>
        /// <param name="data">The list of dictionaries containing the data to be serialized into JSON.</param>
        /// <param name="newInvoiceNumber">The new invoice number.</param>
        /// <param name="mainFileName">The name of the main file.</param>
        /// <param name="localFilePath">The local file path where the JSON request will be saved.</param>
        /// <param name="setId">The set ID.</param>
        /// <param name="lastItem">Indicates whether this is the last item in the batch.</param>
        /// <param name="clientId">The client ID.</param>
        /// <returns>An integer indicating the result of the operation. Returns -1 if an exception occurs.</returns>
        public static async Task<int> MakeJsonRequestBatch(List<Dictionary<string, string>>? data,
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
                var jsonData = await UserConfigRetriever.RetrieveUserConfigById(clientId);

                // Get the list of fields to include in the JSON request
                var fieldsList = MakeJsonRequestHelperClass.ReturnIdFieldListBatch(mainFileName,
                    newInvoiceNumber,
                    setId,
                    clientId);

                // Create the JSON request
                var jsonRequest = await CreatTheJsonRequestBatch(localFilePath,
                    jsonData,
                    data,
                    fieldsList);

                // Send the JSON request to the API
                var result = await SendFilesToApiLines.SendFilesToApiAsync(jsonRequest,
                    localFilePath,
                    clientId);

                if (!result)
                {
                    // Handle failure response
                    return await TpsServerOnFailLines.ServerOnFailLinesAsync(mainFileName,
                        localFilePath,
                        setId,
                        lastItem,
                        clientId);
                }

                if (File.Exists(localFilePath))
                {
                    // Handle success response
                    return await TpsServerOnSuccessLines.ServerOnSuccessLinesAsync(mainFileName,
                        localFilePath,
                        lastItem);
                }
            }
            catch (Exception ex)
            {
                // Log any exceptions that occur during JSON serialization
                WriteLogClass.WriteToLog(0, $"Exception at Json serialization: {ex.Message}", 0);
                return -1;
            }

            return -1;
        }

        /// <summary>
        /// Creates a JSON request object for uploading batch data to a server.
        /// </summary>
        /// <param name="localFile">The path to the local file.</param>
        /// <param name="jsonData">The user configuration data.</param>
        /// <param name="data">The list of data items.</param>
        /// <param name="fieldsList">The list of fields.</param>
        /// <returns>The JSON request string.</returns>
        private static async Task<string> CreatTheJsonRequestBatch(string localFile,
            UserConfigSetter.Customerdetail jsonData,
            List<Dictionary<string, string>>? data,
            TpsJsonLinesUploadString.Fields[] fieldsList)
        {
            try
            {
                // Get the list of local files asynchronously
                var localFileList = await Task.Run(() => MakeJsonRequestHelperClass.ReturnFilesListBatch(localFile));

                // Get the list of tables asynchronously
                var tablesList = await Task.Run(() => MakeJsonRequestHelperClass.ReturnTableListBatch(data));

                // Create a new TpsJsonLinesUploadObject with the provided data
                var tpsJsonRequest = new TpsJsonLinesUploadString.TpsJsonLinesUploadObject
                {
                    Token = jsonData.Token,
                    Username = jsonData.UserName,
                    TemplateKey = jsonData.TemplateKey,
                    Queue = jsonData.Queue,
                    ProjectId = jsonData.ProjectId,
                    Fields = fieldsList,
                    Tables = tablesList,
                    Files = localFileList
                };

                // Serialize the TpsJsonLinesUploadObject to JSON with indentation
                var jsonResult = JsonConvert.SerializeObject(tpsJsonRequest, Formatting.Indented);

                return jsonResult;
            }
            catch (Exception e)
            {
                // Log any exceptions that occur during JSON serialization
                WriteLogClass.WriteToLog(0, $"Exception at lines Json serialization: {e.Message}", 0);
                return string.Empty;
            }
        }

        /// <summary>
        /// Makes a JSON request line by line.
        /// </summary>
        /// <param name="data">The list of data items.</param>
        /// <param name="newInvoiceNumber">The new invoice number.</param>
        /// <param name="mainFileName">The main file name.</param>
        /// <param name="localFilePath">The local file path.</param>
        /// <param name="setId">The set ID.</param>
        /// <param name="lastItem">Indicates whether this is the last item in the batch.</param>
        /// <param name="clientId">The client ID.</param>
        /// <returns>The result of the operation.</returns>
        public static async Task<int> MakeJsonRequestByLine(List<Dictionary<string, string>>? data,
            string newInvoiceNumber,
            string mainFileName,
            string localFilePath,
            string setId,
            bool lastItem,
            int clientId)
        {
            // Retrieve user configuration
            var jsonData = await UserConfigRetriever.RetrieveUserConfigById(clientId);

            // Get the list of fields
            var fieldsList = MakeJsonRequestHelperClass.ReturnIdFieldListLines(data,
                mainFileName,
                setId,
                clientId);

            // Create the JSON request
            var jsonRequest = await CreatTheJsonRequestLines(localFilePath,
                jsonData,
                fieldsList);

            // Send files to the API asynchronously
            var result = await SendFilesToApiLines.SendFilesToApiAsync(jsonRequest,
                localFilePath,
                clientId);

            // Handle failure case
            if (!result)
                return await TpsServerOnFailLines.ServerOnFailLinesAsync(mainFileName,
                    localFilePath,
                    setId,
                    lastItem,
                    clientId);

            // Handle success case
            if (File.Exists(localFilePath))
                return await TpsServerOnSuccessLines.ServerOnSuccessLinesAsync(mainFileName,
                    localFilePath,
                    lastItem);
            
            return -1;
        }

        /// <summary>
        /// Creates a JSON request object for uploading line data to a server.
        /// </summary>
        /// <param name="localFile">The path to the local file.</param>
        /// <param name="jsonData">The user configuration data.</param>
        /// <param name="fieldList">The list of fields.</param>
        /// <returns>The JSON request string.</returns>
        private static async Task<string> CreatTheJsonRequestLines(string localFile,
            UserConfigSetter.Customerdetail jsonData,
            TpsJsonLinesUploadString.Fields[] fieldList)
        {
            try
            {
                // Get the list of local files asynchronously
                var localFileList = await Task.Run(() => MakeJsonRequestHelperClass.ReturnFilesListBatch(localFile));

                // Create a new TpsJsonLinesUploadObject with the provided data
                var tpsJsonRequest = new TpsJsonLinesUploadString.TpsJsonLinesUploadObject
                {
                    Token = jsonData.Token, // Set the token
                    Username = jsonData.UserName, // Set the username
                    TemplateKey = jsonData.TemplateKey, // Set the template key
                    Queue = jsonData.Queue, // Set the queue
                    ProjectId = jsonData.ProjectId, // Set the project ID
                    Fields = fieldList, // Set the fields
                    Tables = Array.Empty<TpsJsonLinesUploadString.Tables>(), // Set the tables (empty for line data)
                    Files = localFileList // Set the files
                };

                // Serialize the TpsJsonLinesUploadObject to JSON with indentation
                var jsonResult = JsonConvert.SerializeObject(tpsJsonRequest, Formatting.Indented);

                return jsonResult;
            }
            catch (Exception e)
            {
                // Log any exceptions that occur during JSON serialization
                WriteLogClass.WriteToLog(0, $"Exception at lines Json serialization: {e.Message}", 0);
                return string.Empty;
            }
        }
    }
}