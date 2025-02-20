using System.Net.Http.Headers;
using AppConfigReader;
using DEA.Next.Graph.GraphEmailActions;
using DEA.Next.Graph.GraphEmailInboxFunctions;
using DEA.Next.HelperClasses.ConfigFileFunctions;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using WriteLog;
using Random = System.Random;

namespace DEA.Next.Graph.GraphClientRelatedFunctions;

public class GraphHelper
{
    /// <summary>
    ///     Main program.cs calls InitializeGetAttachment(int customerId) and
    ///     sends the user id of the user that uses emails submission as the file sending method. Once received,
    ///     the correct user details will be selected from the user config JSON file. And then if the
    ///     collection is not null the graphApiCall() will be called.
    /// </summary>
    /// <param name="customerId"></param>
    /// <returns></returns>
    public static async Task<int> InitializeGetAttachment(Guid customerId)
    {
        var customerDetailsDetails = await UserConfigRetriever.RetrieveEmailConfigById(customerId);
        var emailDetails = customerDetailsDetails.EmailDetails;

        if (emailDetails is null) return 0;

        GetInboxFolderNames getInboxFolderNames = new(emailDetails.EmailInboxPath);
        var mainInbox = getInboxFolderNames.GetNextInboxName();
        var subInbox1 = getInboxFolderNames.GetNextInboxName();
        var subInbox2 = getInboxFolderNames.GetNextInboxName();

        try
        {
            var success = await InitializeGraphClient();

            WriteLogClass.WriteToLog(1, "Graph client initialization successful ....", 5);

            if (!string.IsNullOrEmpty(mainInbox))
                return await GraphGetAttachmentsClass.StartAttachmentDownload(success,
                    emailDetails.Email,
                    mainInbox,
                    subInbox1,
                    subInbox2,
                    customerId);
            WriteLogClass.WriteToLog(0, "Main inbox is not found ....", 0);
            return 0;
        }
        catch (Exception ex)
        {
            WriteLogClass.WriteToLog(0, $"Exception at graph API call: {ex.Message}", 0);
            return 0;
        }
    }

    /// <summary>
    ///     Initialize the graph client and calls GetAuthTokenWithOutUser() to get the token. If Task
    ///     <bool>
    ///         keeps giving
    ///         an error switch to bool. And change the return Task.FromResult(true) to return true;
    ///     </bool>
    /// </summary>
    /// <returns></returns>
    public static async Task<GraphServiceClient> InitializeGraphClient()
    {
        try
        {
            var jsonData = AppConfigReaderClass.ReadAppDotConfig();
            var graphSettings = jsonData.GraphConfig;
            var token = await GetAuthTokenWithOutUser(graphSettings.ClientId,
                graphSettings.Instance,
                graphSettings.TenantId,
                graphSettings.ClientSecret,
                graphSettings.Scopes);

            var graphClient = new GraphServiceClient(graphSettings.GraphApiUrl,
                new DelegateAuthenticationProvider(async requestMessage =>
                    {
                        await Task.Run(() =>
                        {
                            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                        });
                    }
                ));
            return graphClient;
        }
        catch (Exception ex)
        {
            WriteLogClass.WriteToLog(0, $"Exception at graph client initializing: {ex.Message}", 0);
            throw;
        }
    }

    // Get the token from the Azure according to the default scopes set in the server.
    private static async Task<string> GetAuthTokenWithOutUser(string clientId, string instanceId, string tenantId,
        string clientSecret, string[] scopes)
    {
        var authority = string.Concat(instanceId, tenantId);

        var application = ConfidentialClientApplicationBuilder.Create(clientId)
            .WithClientSecret(clientSecret)
            .WithAuthority(new Uri(authority))
            .Build();
        try
        {
            // Get the token and assigns it AuthToken variable.
            var authToken = await application.AcquireTokenForClient(scopes).ExecuteAsync();
            return authToken.AccessToken;
        }
        catch (MsalUiRequiredException ex)
        {
            // The application doesn't have sufficient permissions.
            // - Did you declare enough app permissions during app creation?
            // - Did the tenant admin grant permissions to the application?
            WriteLogClass.WriteToLog(0, $"Exception at token acquire: {ex.Message}", 0);
            return string.Empty;
        }
        catch (MsalServiceException ex) when (ex.Message.Contains("AADSTS70011"))
        {
            // Invalid scope. The scope has to be in the form "https://resourceurl/.default"
            // Mitigation: Change the scope to be as expected.
            // Need to be set on the online under Azure Active Directory -> App Registrations -> <App Name> -> API Permissions.
            // The scopes/rules need to be "Application" type. "Delegated" type doesn't work for auto login.
            WriteLogClass.WriteToLog(0, "Scopes provided are not supported", 0);
            return string.Empty;
        }
    }

    /// <summary>
    ///     Generates a random 10-digit number for the sub download folder name.
    /// </summary>
    /// <param name="rndLength"></param>
    /// <returns></returns>
    public static string FolderNameRnd(int rndLength)
    {
        Random rndNumber = new();
        var numString = string.Empty;
        for (var i = 0; i < rndLength; i++) numString = string.Concat(numString, rndNumber.Next(10).ToString());
        return numString;
    }
}