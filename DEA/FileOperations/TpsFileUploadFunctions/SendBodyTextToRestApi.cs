using System.Net;
using DEA.Next.Extensions;
using DEA.Next.FileOperations.TpsServerResponseFunctions;
using RestSharp;

namespace DEA.Next.FileOperations.TpsFileUploadFunctions;

internal class SendBodyTextToRestApi
{
    public static async Task<int> SendBodyTextToRestAsync(Guid customerId, string jsonString)
    {
        var (mainDomain, query) = await customerId.SplitUrl();

        // Creating rest api request.
        RestClient client = new(mainDomain);
        RestRequest tpsRequest = new(query)
        {
            Method = Method.Post,
            RequestFormat = DataFormat.Json
        };

        tpsRequest.AddBody(jsonString);

        var serverResponse = await client.ExecuteAsync(tpsRequest); // Executes the request and send to the server.

        if (serverResponse.StatusCode != HttpStatusCode.OK)
            return await TpsServerOnFaile.ServerOnFailBodyTextAsync(requestBuilder,
                messageId,
                serverResponse.Content,
                serverResponse.StatusCode);

        return await TpsServerOnSuccess.ServerOnSuccessBodyTextAsync(requestBuilder,
            messageId,
            messageSubject);
    }
}