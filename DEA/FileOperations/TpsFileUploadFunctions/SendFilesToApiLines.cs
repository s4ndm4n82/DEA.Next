using DEA.Next.HelperClasses.FolderFunctions;
using RestSharp;
using UserConfigRetriverClass;
using WriteLog;

namespace DEA.Next.FileOperations.TpsFileUploadFunctions
{
    internal static class SendFilesToApiLines
    {
        /// <summary>
        /// Sends a JSON request to the API.
        /// </summary>
        /// <param name="jsonResult">The JSON request to send.</param>
        /// <param name="localFile">The local file path.</param>
        /// <param name="clientId">The client ID.</param>
        /// <returns>A Task that represents the asynchronous operation. The task result contains a boolean indicating if the request was successful.</returns>
        public static async Task<bool> SendFilesToApiAsync(string jsonResult,
            string localFile,
            int clientId)
        {
            // Retrieve user configuration by client ID
            var jsonData = await UserConfigRetriver.RetriveUserConfigById(clientId);
    
            try
            {
                // Create a new RestClient instance with the main domain from the user configuration
                var client = new RestClient(jsonData.DomainDetails.MainDomain);
        
                // Create a new RestRequest instance with the TpsRequestUrl from the user configuration
                var request = new RestRequest(jsonData.DomainDetails.TpsRequestUrl)
                {
                    Method = Method.Post,
                    RequestFormat = DataFormat.Json
                };

                // Add the JSON request to the request body
                request.AddBody(jsonResult);
        
                // Execute the request asynchronously
                var serverResponse = await client.ExecuteAsync(request);

                // If the server response is successful, return true
                if (serverResponse.IsSuccessful) return serverResponse.IsSuccessful;
        
                // Log the server status code and content
                WriteLogClass.WriteToLog(0, $"Server status code: {serverResponse.StatusCode}," +
                                            $" content: {serverResponse.Content}", 0);
                return false;

            }
            catch (Exception ex)
            {
                // Log the exception message
                WriteLogClass.WriteToLog(0, $"Exception at Sending to Json to TPS: {ex.Message}", 0);
                return false;
            }
        }
    }
}
