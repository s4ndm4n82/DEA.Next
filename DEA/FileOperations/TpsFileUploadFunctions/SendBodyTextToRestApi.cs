using DEA.Next.FileOperations.TpsServerResponseFunctions;
using Microsoft.Graph;
using RestSharp;
using System.Net;
using DEA.Next.Extensions;

namespace DEA.Next.FileOperations.TpsFileUploadFunctions;

internal class SendBodyTextToRestApi
{
    public static async Task<int> SendBodyTextToRestAsync(IMailFolderRequestBuilder requestBuilder,
        string messageId,
        string messageSubject,
        string jsonString,
        Guid clientId)
    {
        var (mainDomain, query) = await clientId.SplitUrl();

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
        {
            return await TpsServerOnFaile.ServerOnFailBodyTextAsync(requestBuilder,
                messageId,
                serverResponse.Content,
                serverResponse.StatusCode);
        }

        return await TpsServerOnSuccess.ServerOnSuccessBodyTextAsync(requestBuilder,
            messageId,
            messageSubject);
    }
}