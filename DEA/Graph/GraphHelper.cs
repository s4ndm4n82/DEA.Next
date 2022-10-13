using Microsoft.Graph;
using Microsoft.Identity.Client;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using WriteLog;
using DEA2Levels;
using DEAHelper1Leve;
using ReadSettings;
using UserConfigReader;
using CreateMetadataFile; // Might need to use this later so leaving it.
using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DEA
{
    public class GraphHelper
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
        public static async Task InitializGetAttachment(int customerId)
        {
            var jsonData = await UserConfigReaderClass.ReadAppDotConfig<UserConfigReaderClass.CustomerDetailsObject>();
            var clientDetails = jsonData.CustomerDetails!.FirstOrDefault(cid => cid.id == customerId);

            if(clientDetails != null)
            {
                try
                {
                    graphApiCall(); // Initilizes the graph API.
                }
                catch (Exception ex)
                {
                    WriteLogClass.WriteToLog(2, $"Exception at graph API call: {ex.Message}", string.Empty);
                }
                
            }

            if (!clientDetails!.EmailDetails!.SubInbox1.IsNullOrEmpty())
            {
                try
                {
                    // Calls the function to read ATC emails.
                    await GraphHelper2Levels.GetEmailsAttacmentsAccount(graphClient!, clientDetails.EmailDetails.EmailAddress!);
                }
                catch (Exception ex)
                {
                    WriteLogClass.WriteToLog(2, $"Exception at GraphHelper2Levels: {ex.Message}", string.Empty);
                }
            }
            else
            {
                try
                {
                    // Calls the function for reading accounting emails for attachments.
                    await GraphHelper1LevelClass.GetEmailsAttacments1Level(graphClient!, clientDetails.EmailDetails.EmailAddress!);
                }
                catch (Exception ex)
                {
                    WriteLogClass.WriteToLog(2, $"Exception at GraphHelper1Levels: {ex.Message}", string.Empty);
                }
            }
        }

        private static async void graphApiCall()
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
                    WriteLogClass.WriteToLog(1, "Set the Graph API permissions. Using dotnet user-secrets set or appsettings.json.... User secrets is not correct.", string.Empty);
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
            if (!await GraphHelper.InitializeGraphClient(ClientId, Instance, TenantId, GraphApiUrl, ClientSecret, Scopes))
            {
                WriteLogClass.WriteToLog(1, "Graph client initialization faild  .....", string.Empty);
            }
            else
            {
                WriteLogClass.WriteToLog(3, "Graph client initialization successful ....", string.Empty);
                Thread.Sleep(5000);
                WriteLogClass.WriteToLog(3, "Starting attachment download process ....", string.Empty);
                await GraphHelper.InitializGetAttachment();
            }

            WriteLogClass.WriteToLog(3, "Email processing ended ...\n", string.Empty);
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

        // Initilize the graph clinet and calls GetAuthTokenWithOutUser() to get the token.
        // If Task<bool> keeps giving an error switch to bool. And change the return Task.FromResult(true) to return true;
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
                WriteLogClass.WriteToLog(1, $"Exception at graph client initilizing: {ex.Message}", string.Empty);
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
                WriteLogClass.WriteToLog(1, $"Exception at token accuire: {ex.Message}", string.Empty);
            }
            catch (MsalServiceException ex) when (ex.Message.Contains("AADSTS70011"))
            {
                // Invalid scope. The scope has to be in the form "https://resourceurl/.default"
                // Mitigation: Change the scope to be as expected.
                WriteLogClass.WriteToLog(1, "Scopes provided are not supported", string.Empty);
            }

            return AuthToken!.AccessToken;

        }

        // Generate the random 10 digit number as the folder name.
        public static string FolderNameRnd(int RndLength)
        {
            Random RndNumber = new();
            string NumString = string.Empty;
            for (int i = 0; i < RndLength; i++)
            {
                NumString = String.Concat(NumString, RndNumber.Next(10).ToString());
            }
            return NumString;
        }

        // Check the exsistance of the download folders.
        public static string CheckFolders(string FolderSwitch)
        {
            // Get current execution path.
            string FolderPath = string.Empty;
            string? PathRootFolder = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string DownloadFolderName = "Download";
            string LogFolderName = "Logs";
            string PathDownloadFolder = Path.Combine(PathRootFolder!, DownloadFolderName);
            string PathLogFolder = Path.Combine(PathRootFolder!, LogFolderName);

            // Check if download folder exists. If not creates the fodler.
            if (!System.IO.Directory.Exists(PathDownloadFolder))
            {
                try
                {
                    System.IO.Directory.CreateDirectory(PathDownloadFolder);
                }
                catch (Exception ex)
                {
                    WriteLogClass.WriteToLog(1, $"Exception at download folder creation: {ex.Message}", string.Empty);
                }
            }

            if (!System.IO.Directory.Exists(PathLogFolder))
            {
                try
                {
                    System.IO.Directory.CreateDirectory(PathLogFolder);
                }
                catch (Exception ex)
                {
                    WriteLogClass.WriteToLog(1, $"Exception at download folder creation: {ex.Message}", string.Empty);
                }
            }

            if (FolderSwitch == "Download")
            {
                FolderPath = PathDownloadFolder;
            }
            else if (FolderSwitch == "Log")
            {
                FolderPath = PathLogFolder;
            }
            else
            {
                FolderPath = string.Empty;
            }

            return FolderPath;
        }

        // Downnloads the attachments to local harddrive.
        public static bool DownloadAttachedFiles(string DownloadFolderPath, string DownloadFileName, byte[] DownloadFileData)
        {
            if (!System.IO.Directory.Exists(DownloadFolderPath))
            {
                try
                {
                    System.IO.Directory.CreateDirectory(DownloadFolderPath);
                }
                catch (Exception ex)
                {
                    WriteLogClass.WriteToLog(1, $"Exception at download folder creation: {ex.Message}", string.Empty);
                }
            }

            try
            {
                var PathFullDownloadFile = Path.Combine(DownloadFolderPath, DownloadFileName);
                var FileNameOnly = Path.GetFileNameWithoutExtension(PathFullDownloadFile);
                var FileExtention = Path.GetExtension(PathFullDownloadFile);
                var FilePathOnly = Path.GetDirectoryName(PathFullDownloadFile);
                int Count = 1;

                while (System.IO.File.Exists(PathFullDownloadFile)) // If file exists starts to rename from next file.
                {
                    var NewFileName = string.Format("{0}({1})", FileNameOnly, Count++); // Makes the new file name.
                    PathFullDownloadFile = Path.Combine(FilePathOnly!, NewFileName + FileExtention); // Set tthe new path as the download file path.
                }

                // Full path for the attachment to be downloaded with the attachment name                
                System.IO.File.WriteAllBytes(PathFullDownloadFile, DownloadFileData);
                return true;
            }
            catch (Exception ex)
            {
                WriteLogClass.WriteToLog(1, $"Exception at download path: {ex.Message}", string.Empty);
                return false;
            }
        }

        // Move the folder to main import folder on the local machine.
        public static bool MoveFolder(string SourceFolderPath, string DestiFolderPath)
        {
            try
            {
                var SourceParent = System.IO.Directory.GetParent(SourceFolderPath);
                var SourceFolders = System.IO.Directory.GetDirectories(SourceParent!.FullName, "*.*", SearchOption.AllDirectories);

                foreach (var SourceFolder in SourceFolders)
                {
                    var SourceLastFolder = SourceFolder.Split(Path.DirectorySeparatorChar).Last(); // Get the last folder from the source path.
                    var SourceFiles = System.IO.Directory.GetFiles(SourceFolder, "*.*", SearchOption.AllDirectories); // Get the source file list.
                    var FullDestinationPath = Path.Combine(DestiFolderPath, SourceLastFolder); // Makes the destiantion path with the last folder name

                    if (!System.IO.Directory.Exists(FullDestinationPath)) // Create the folder if not exists.
                    {
                        System.IO.Directory.CreateDirectory(FullDestinationPath);
                    }

                    foreach (var SourceFile in SourceFiles) // Loop throug the files list.
                    {
                        var SourceFileName = System.IO.Path.GetFileName(SourceFile); // Get the source file name.
                        var SourcePath = Path.Combine(SourceFolder, SourceFileName); // Makes the full source path.
                        var DestinationPath = Path.Combine(FullDestinationPath, SourceFileName); // Makes the full destination path.

                        if (!System.IO.Directory.Exists(DestinationPath))
                        {
                            System.IO.File.Move(SourcePath, DestinationPath); // Moves the files to the destination path.

                            WriteLogClass.WriteToLog(3, $"Moving file {SourceFileName}", string.Empty);
                        }
                        else
                        {
                            WriteLogClass.WriteToLog(3, $"File already exists .... skipping to the next file.", string.Empty);
                            continue;
                        }
                    }

                    if (System.IO.Directory.Exists(SourceFolder)) // Delete the file from source if exists.
                    {
                        System.IO.Directory.Delete(SourceFolder, true);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                WriteLogClass.WriteToLog(1, $"Error getting event: {ex.Message}", string.Empty);
                return false;
            }
        }

        // Forwards emails with out any attachment to the sender.
        public static async Task<(string?, bool)> ForwardEmtpyEmail(string FolderId1, string FolderId2, string ErrFolderId, string MsgId2, string _Email, int AttnStatus)
        {
            try
            {
                if (!string.IsNullOrEmpty(FolderId2))
                {
                    // Get ths the emails details like subject and from email.
                    var MsgDetails = await graphClient!.Users[$"{_Email}"].MailFolders["Inbox"]
                            .ChildFolders[$"{FolderId1}"]
                            .ChildFolders[$"{FolderId2}"]
                            .ChildFolders[$"{ErrFolderId}"]
                            .Messages[$"{MsgId2}"]
                            .Request()
                            .Select(em => new
                            {
                                em.Subject,
                                em.From
                            })
                            .GetAsync();

                    // Variables to be used with graph forward.
                    var FromName = MsgDetails.From.EmailAddress.Name;
                    var FromEmail = MsgDetails.From.EmailAddress.Address;
                    var MailComment = string.Empty;

                    if (AttnStatus != 1)
                    {
                        MailComment = "Hi,<br /><b>Please do not reply to this email</b><br />. Below email doesn't contain any attachment."; // Can be change with html.
                    }
                    else
                    {
                        MailComment = "Hi,<br /><b>Please do not reply to this email</b><br />Below email's attachment file type is not accepted. Please send attachments as .pdf or .jpg.";
                    }

                    // Recipient setup for the mail header.
                    var toRecipients = new List<Recipient>()
                    {
                        new Recipient
                        {
                            EmailAddress = new EmailAddress
                            {
                                Name = FromName,
                                Address = FromEmail
                            }
                        }
                    };

                    // Forwarding the non attachment email using .forward().
                    await graphClient.Users[$"{_Email}"].MailFolders["Inbox"]
                        .ChildFolders[$"{FolderId1}"]
                        .ChildFolders[$"{FolderId2}"]
                        .ChildFolders[$"{ErrFolderId}"]
                        .Messages[$"{MsgId2}"]
                        .Forward(toRecipients, null, MailComment)
                        .Request()
                        .PostAsync();

                    return (FromEmail, true);
                }
                else
                {
                    // Get ths the emails details like subject and from email.
                    var MsgDetails = await graphClient!.Users[$"{_Email}"].MailFolders["Inbox"]
                            .ChildFolders[$"{FolderId1}"]
                            .ChildFolders[$"{ErrFolderId}"]
                            .Messages[$"{MsgId2}"]
                            .Request()
                            .Select(em => new
                            {
                                em.Subject,
                                em.From
                            })
                            .GetAsync();

                    // Variables to be used with graph forward.
                    var FromName = MsgDetails.From.EmailAddress.Name;
                    var FromEmail = MsgDetails.From.EmailAddress.Address;
                    var MailComment = string.Empty;

                    if (AttnStatus != 1)
                    {
                        MailComment = "Hi,<br /><b>Please do not reply to this email</b><br />. Below email doesn't contain any attachment."; // Can be change with html.
                    }
                    else
                    {
                        MailComment = "Hi,<br /><b>Please do not reply to this email</b><br />Below email's attachment file type is not accepted. Please send attachments as .pdf or .jpg.";
                    }

                    // Recipient setup for the mail header.
                    var toRecipients = new List<Recipient>()
                {
                    new Recipient
                    {
                        EmailAddress = new EmailAddress
                        {
                            Name = FromName,
                            Address = FromEmail
                        }
                    }
                };

                    // Forwarding the non attachment email using .forward().
                    await graphClient.Users[$"{_Email}"].MailFolders["Inbox"]
                        .ChildFolders[$"{FolderId1}"]
                        .ChildFolders[$"{ErrFolderId}"]
                        .Messages[$"{MsgId2}"]
                        .Forward(toRecipients, null, MailComment)
                        .Request()
                        .PostAsync();

                    return (FromEmail, true);
                }

            }
            catch (Exception ex)
            {
                return (ex.Message, false);
            }
        }

        //Moves the email to Downloded folder.
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
                WriteLogClass.WriteToLog(1, $"Exception at moving emails to folders: {ex.Message}", string.Empty);
                return false;
            }
        }
    }
}