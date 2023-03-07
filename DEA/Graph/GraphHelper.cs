using Microsoft.Graph;
using Microsoft.Identity.Client;
using System.Net.Http.Headers;
using WriteLog;
using GraphGetAttachments;
using UserConfigReader;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

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
            UserConfigReaderClass.CustomerDetailsObject jsonData = UserConfigReaderClass.ReadAppDotConfig<UserConfigReaderClass.CustomerDetailsObject>();
            UserConfigReaderClass.Customerdetail clientDetails = jsonData.CustomerDetails!.FirstOrDefault(cid => cid.id == customerId)!;

            if(clientDetails != null)
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

            if (!clientDetails!.EmailDetails!.MainInbox.IsNullOrEmpty())
            {
                try
                {
                    // Calls the function to read ATC emails.
                    result = await GraphGetAttachmentsClass.GetEmailsAttacments(graphClient!, clientDetails.EmailDetails.EmailAddress!,
                                                                                clientDetails.EmailDetails.MainInbox!, clientDetails.EmailDetails.SubInbox1!,
                                                                                clientDetails.EmailDetails.SubInbox2!, customerId);
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
            // Getting the Graph and checking the settings for Graph.
            var appConfig = LoadAppSettings();

            // Declaring variable to be used with in the if below.
            var ClientId = string.Empty;
            var TenantId = string.Empty;
            var Instance = string.Empty;
            var GraphApiUrl = string.Empty;
            var ClientSecret = string.Empty;
            string[] Scopes = new string[] { };

            // If appConfig is equal to null look for settings with in the appsettings.json file.
            if (appConfig == null)
            {
                // Read the appsettings json file and loads the text in to AppCofigJson variable.
                // File should be with in the main working directory.
                var AppConfigJson = new ConfigurationBuilder()
                    .SetBasePath(System.IO.Directory.GetCurrentDirectory())
                    .AddJsonFile(@".\Config\appsettings.json", optional: true, reloadOnChange: true)
                    .Build();

                // Initilize the variables with values.
                ClientId = AppConfigJson.GetSection("GraphConfig").GetSection("ClientId").Value;
                TenantId = AppConfigJson.GetSection("GraphConfig").GetSection("TenantId").Value;
                Instance = AppConfigJson.GetSection("GraphConfig").GetSection("Instance").Value;
                GraphApiUrl = AppConfigJson.GetSection("GraphConfig").GetSection("GraphApiUrl").Value;
                ClientSecret = AppConfigJson.GetSection("GraphConfig").GetSection("ClientSecret").Value;
                Scopes = new string[] { $"{AppConfigJson.GetSection("GraphConfig").GetSection("Scopes").Value}" };

                // If Json file is also returns empty then below error would be shown.
                if (string.IsNullOrEmpty(ClientId) ||
                    string.IsNullOrEmpty(TenantId) ||
                    string.IsNullOrEmpty(Instance) ||
                    string.IsNullOrEmpty(GraphApiUrl) ||
                    string.IsNullOrEmpty(ClientSecret))
                {
                    WriteLogClass.WriteToLog(1, "Set the Graph API permissions. Using dotnet user-secrets set or appsettings.json.... User secrets is not correct.", 1);
                }
            }
            else
            {
                // If appConfig is not equal to null then assings all the setting to variables from UserSecrets.
                ClientId = appConfig["ClientId"];
                TenantId = appConfig["TenantId"];
                Instance = appConfig["Instance"];
                GraphApiUrl = appConfig["GraphApiUrl"];
                ClientSecret = appConfig["ClientSecret"];
                Scopes = new string[] { $"{appConfig["Scopes"]}" };// Gets the application permissions which are set from the Azure AD.
            }

            // Calls InitializeGraphClient to get the token and connect to the graph API.
            if (!await InitializeGraphClient(ClientId!, Instance!, TenantId!, GraphApiUrl!, ClientSecret!, Scopes))
            {
                WriteLogClass.WriteToLog(1, "Graph client initialization faild  .....", 1);
            }
            else
            {
                WriteLogClass.WriteToLog(1, "Graph client initialization successful ....", 1);
                Thread.Sleep(2000);
            }
        }
        // Loads the settings from user sectrets file.
        static IConfigurationRoot? LoadAppSettings()
        {
            var appConfigUs = new ConfigurationBuilder()
                 .AddUserSecrets<Program>()
                 .Build();

            // Check for required settings in app secrets.
            if (string.IsNullOrEmpty(appConfigUs["ClientId"]) ||
                string.IsNullOrEmpty(appConfigUs["TenantId"]) ||
                string.IsNullOrEmpty(appConfigUs["Instance"]) ||
                string.IsNullOrEmpty(appConfigUs["GraphApiUrl"]) ||
                string.IsNullOrEmpty(appConfigUs["ClientSecret"]))
            {
                return null;
            }
            else
            {
                return appConfigUs;
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
        public static async Task<string> GetAuthTokenWithOutUser(string clientId, string instanceId, string tenantId, string clientSecret, string[] scopes)
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

        /// <summary>
        /// Moves the email to Downloded folder.
        /// </summary>
        /// <param name="FirstFolderId"></param>
        /// <param name="SecondFolderId"></param>
        /// <param name="ThirdFolderId"></param>
        /// <param name="MsgId"></param>
        /// <param name="DestiId"></param>
        /// <param name="_Email"></param>
        /// <returns></returns>
        public static async Task<bool> MoveEmails(string FirstFolderId, string SecondFolderId, string ThirdFolderId, string MsgId, string DestiId, string _Email)
        {
            try
            {
                if (string.IsNullOrEmpty(ThirdFolderId) && string.IsNullOrEmpty(SecondFolderId))
                {
                    //Graph api call to move the email message.
                    await graphClient!.Users[$"{_Email}"].MailFolders["Inbox"]
                        .ChildFolders[$"{FirstFolderId}"]
                        .Messages[$"{MsgId}"]
                        .Move(DestiId)
                        .Request()
                        .PostAsync();
                }
                else if (string.IsNullOrEmpty(ThirdFolderId))
                {
                    //Graph api call to move the email message.
                    await graphClient!.Users[$"{_Email}"].MailFolders["Inbox"]
                        .ChildFolders[$"{FirstFolderId}"]
                        .ChildFolders[$"{SecondFolderId}"]
                        .Messages[$"{MsgId}"]
                        .Move(DestiId)
                        .Request()
                        .PostAsync();
                }
                else
                {
                    //Graph api call to move the email message.
                    await graphClient!.Users[$"{_Email}"].MailFolders["Inbox"]
                        .ChildFolders[$"{FirstFolderId}"]
                        .ChildFolders[$"{SecondFolderId}"]
                        .ChildFolders[$"{ThirdFolderId}"]
                        .Messages[$"{MsgId}"]
                        .Move(DestiId)
                        .Request()
                        .PostAsync();
                }
                return true;
            }
            catch (Exception ex)
            {
                WriteLogClass.WriteToLog(0, $"Exception at moving emails to folders: {ex.Message}", 0);
                return false;
            }
        }
    }
}