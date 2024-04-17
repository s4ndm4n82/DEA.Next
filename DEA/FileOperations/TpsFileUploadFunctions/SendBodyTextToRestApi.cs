using RestSharp;
using System.Net;
using UserConfigRetriverClass;
using UserConfigSetterClass;

namespace DEA.Next.FileOperations.TpsFileUploadFunctions
{
    internal class SendBodyTextToRestApi
    {
        public static async Task<int> SendBodyTextToRestAsync(string jsonString,
                                                              int clientId)
        {
            UserConfigSetter.Customerdetail customerDetails = await UserConfigRetriver.RetriveUserConfigById(clientId);

            // Creating rest api request.
            RestClient client = new($"{customerDetails.DomainDetails.MainDomain}");
            RestRequest tpsRequest = new($"{customerDetails.DomainDetails.TpsRequestUrl}")
            {
                Method = Method.Post,
                RequestFormat = DataFormat.Json
            };

            tpsRequest.AddBody(jsonString);

            RestResponse serverResponse = await client.ExecuteAsync(tpsRequest); // Executes the request and send to the server.

            if (serverResponse.StatusCode != HttpStatusCode.OK)
            {

            }

            return 0;
        }
    }
}
