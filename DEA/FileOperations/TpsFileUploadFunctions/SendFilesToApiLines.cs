﻿using DEA.Next.HelperClasses.FolderFunctions;
using RestSharp;
using UserConfigRetriverClass;
using WriteLog;

namespace DEA.Next.FileOperations.TpsFileUploadFunctions
{
    internal static class SendFilesToApiLines
    {
        public static async Task<bool> SendFilesToApiLinesAsync(string jsonResult,
            string localFile,
            int clientId)
        {
            var jsonData = await UserConfigRetriver.RetriveUserConfigById(clientId);
            
            try
            {
                var client = new RestClient(jsonData.DomainDetails.MainDomain);
                var request = new RestRequest(jsonData.DomainDetails.TpsRequestUrl)
                {
                    Method = Method.Post,
                    RequestFormat = DataFormat.Json
                };

                request.AddBody(jsonResult);
                
                var serverResponse = await client.ExecuteAsync(request);

                if (serverResponse.IsSuccessful) return serverResponse.IsSuccessful;
                
                WriteLogClass.WriteToLog(0, $"Server status code: {serverResponse.StatusCode}," +
                                            $"content: {serverResponse.Content}", 0);
                return false;

            }
            catch (Exception ex)
            {
                WriteLogClass.WriteToLog(0, $"Exception at Sending to Json to TPS: {ex.Message}", 0);
                return false;
            }
        }
    }
}