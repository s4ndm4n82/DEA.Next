using Microsoft.Graph;
using Microsoft.Identity.Client;
using System.Net.Http.Headers;
using WriteLog;
using GraphGetAttachments;
using UserConfigSetterClass;
using Microsoft.IdentityModel.Tokens;
using AppConfigReader;
using UserConfigRetriverClass;
using DEA.Next.Graph.GraphEmailInboxFunctions;

namespace GraphHelper
{
    public class GraphHelperClass
    {
        private static GraphServiceClient? graphClient;
        private static AuthenticationResult? AuthToken;
        private static IConfidentialClientApplication? Application;

        /// <summary>
        /// Main program.cs calls InitializGetAttachment(int customerId) and sends the user id of the user that uses emails submition as the
        /// file sending method. Once resived the correct user details will be selected from the user config JSON file. And then if the
        /// collection is not null the graphApiCall() will be called.
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        public static async Task<int> InitializGetAttachment(int customerId)
        {
            int result = 0;
            UserConfigSetter.Customerdetail clientDetails = await UserConfigRetriver.RetriveUserConfigById(customerId);

            GetInboxFolderNames getInboxFolderNames = new(clientDetails.EmailDetails.EmailInboxPath);
            string mainInbox = getInboxFolderNames.GetNextInboxName();
            string subInbox1 = getInboxFolderNames.GetNextInboxName();
            string subInbox2 = getInboxFolderNames.GetNextInboxName();

            if (clientDetails != null)
            {
                try
                {
                    GraphApiCall(); // Initilizes the graph API.
                }
                catch (Exception ex)
                {
                    WriteLogClass.WriteToLog(0, $"Exception at graph API call: {ex.Message}", 0);
                }
                
            }

            if (!mainInbox.IsNullOrEmpty())
            {
                try
                {
                    // Calls the function to read ATC emails.
                    result = await GraphGetAttachmentsClass.StartAttacmentDownload(graphClient,
                                                                                   clientDetails.EmailDetails.EmailAddress!,
                                                                                   mainInbox,
                                                                                   subInbox1,
                                                                                   subInbox2,
                                                                                   customerId);
                    return result;
                }
                catch (Exception ex)
                {
                    WriteLogClass.WriteToLog(0, $"Exception at GraphHelper2Levels: {ex.Message}", 0);
                    return result;
                }
            }
            return result;
        }

        /// <summary>
        /// As the function name suggest this is the main function that calles the GraphAPI and establish the connection.
        /// </summary>
        private static async void GraphApiCall()
        {
            GraphApiInitializer graphApiInitializer = new();

            bool success = await graphApiInitializer.GraphInitialize();

            if (!success)
            {
                WriteLogClass.WriteToLog(0, "Graph client initialization faild  .....", 5);
            }
            else
            {
                WriteLogClass.WriteToLog(1, "Graph client initialization successful ....", 5);
            }
        }
        /// <summary>
        /// Initialize and returns the success message.
        /// </summary>
        private class GraphApiInitializer
        {
            private readonly AppConfigReaderClass.AppSettingsRoot jsonData;

            public GraphApiInitializer()
            {
                jsonData = AppConfigReaderClass.ReadAppDotConfig();                
            }

            public async Task<bool> GraphInitialize()
            {
                AppConfigReaderClass.Graphconfig graphSettings = jsonData.GraphConfig;

                bool success = await InitializeGraphClient(graphSettings.ClientId,
                                                           graphSettings.Instance,
                                                           graphSettings.TenantId,
                                                           graphSettings.GraphApiUrl,
                                                           graphSettings.ClientSecret,
                                                           graphSettings.Scopes);

                return success;
            }
        }

        /// <summary>
        /// Initilize the graph clinet and calls GetAuthTokenWithOutUser() to get the token. If Task<bool> keeps giving an error switch to bool. And change the return Task.FromResult(true) to return true;
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="instanceId"></param>
        /// <param name="tenantId"></param>
        /// <param name="graphUrl"></param>
        /// <param name="clientSecret"></param>
        /// <param name="scopes"></param>
        /// <returns></returns>        
        public static Task<bool> InitializeGraphClient(string clientId, string instanceId, string tenantId, string graphUrl, string clientSecret, string[] scopes)
        {
            try
            {
                graphClient = new GraphServiceClient(graphUrl,
                    new DelegateAuthenticationProvider(async (requestMessage) =>
                    {
                        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("bearer", await GetAuthTokenWithOutUser(clientId, instanceId, tenantId, clientSecret, scopes));
                    }
                    ));
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                WriteLogClass.WriteToLog(0, $"Exception at graph client initilizing: {ex.Message}", 0);
                return Task.FromResult(false);
            }
        }

        // Get the token from the Azure according to the default scopes set in the server.
        private static async Task<string> GetAuthTokenWithOutUser(string clientId, string instanceId, string tenantId, string clientSecret, string[] scopes)
        {
            string Authority = string.Concat(instanceId, tenantId);

            Application = ConfidentialClientApplicationBuilder.Create(clientId)
                          .WithClientSecret(clientSecret)
                          .WithAuthority(new Uri(Authority))
                          .Build();
            try
            {
                // Aquirs the token and assigns it AuthToken variable.
                AuthToken = await Application.AcquireTokenForClient(scopes).ExecuteAsync();
            }
            catch (MsalUiRequiredException ex)
            {
                // The application doesn't have sufficient permissions.
                // - Did you declare enough app permissions during app creation?
                // - Did the tenant admin grant permissions to the application?
                WriteLogClass.WriteToLog(0, $"Exception at token accuire: {ex.Message}", 0);
            }
            catch (MsalServiceException ex) when (ex.Message.Contains("AADSTS70011"))
            {
                // Invalid scope. The scope has to be in the form "https://resourceurl/.default"
                // Mitigation: Change the scope to be as expected.
                // Need to be set on the online under Azure Active Directory -> App Registrations -> <App Name> -> API Permissions.
                // The scopes/rules need to be "Application" type. "Delegated" type doesn't work for auto login.
                WriteLogClass.WriteToLog(0, "Scopes provided are not supported", 0);
            }

            return AuthToken!.AccessToken;

        }

        /// <summary>
        /// Generates a random 10 digit number for the sub download folder name.
        /// </summary>
        /// <param name="RndLength"></param>
        /// <returns></returns>
        public static string FolderNameRnd(int RndLength)
        {
            Random rndNumber = new();
            string numString = string.Empty;
            for (int i = 0; i < RndLength; i++)
            {
                numString = String.Concat(numString, rndNumber.Next(10).ToString());
            }
            return numString;
        }        
    }
}