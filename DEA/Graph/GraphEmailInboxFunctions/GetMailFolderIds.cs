using DEA.Next.HelperClasses.OtherFunctions;
using AppConfigReader;
using Microsoft.Graph;
using System.Diagnostics.CodeAnalysis;
using WriteLog;

namespace GetMailFolderIds
{
    internal class GetMailFolderIdsClass
    {
        /// <summary>
        /// Class to set all the ID retrived from the graph SDK.
        /// </summary>
        public class ClientFolderId
        {
            public string ClientMainFolderId { get; set; }
            public string ClientSubFolderId1 { get; set; }
            public string ClientSubFolderId2 { get; set; }
        }

        /// <summary>
        /// Get's the inbox id's form the sub inboxes on the email server. This will go deep as 3 levels auto matically.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="graphClient"></param>
        /// <param name="clientEmail"></param>
        /// <param name="clientMainFolderName"></param>
        /// <param name="clientSubFolderName1"></param>
        /// <param name="clientSubfolderName2"></param>
        /// <returns></returns>
        public static async Task<ClientFolderId> GetChlidFolderIds<T>([NotNull] GraphServiceClient graphClient,
                                                                                string clientEmail,
                                                                                string clientMainFolderName,
                                                                                string clientSubFolderName1,
                                                                                string clientSubfolderName2)
        {
            AppConfigReaderClass.AppSettingsRoot jsonData = AppConfigReaderClass.ReadAppDotConfig();
            AppConfigReaderClass.Programsettings programSettings = jsonData.ProgramSettings;

            ClientFolderId folderIds = new();

            try
            {
                // Getting the clinet inbox name.
                if (!string.IsNullOrWhiteSpace(clientEmail))
                {
                    // Creating the main request builder.
                    IMailFolderRequestBuilder mainRequestBuilder = await GetRequestBuilderAsync(graphClient,
                                                                         clientEmail,
                                                                         null,
                                                                         null);
                    if (mainRequestBuilder != null)
                    {
                        // Creating the inbox id.
                        IMailFolderRequestBuilder mainFolderBuilder = await GetChildFolderIdByName(mainRequestBuilder,
                                                                                                   clientMainFolderName,
                                                                                                   programSettings.MaxMainEmailFolders);

                        if (mainFolderBuilder == null)
                        {
                            WriteLogClass.WriteToLog(0, $"Client main folder ID is empty ....", 0);
                            return null;
                        }

                        // Setting the inbox id.
                        folderIds.ClientMainFolderId = mainFolderBuilder.Request().Select("id").GetAsync().Result.Id;
                    }
                }

                // Getting the sub inboxe ID.
                if (!string.IsNullOrEmpty(clientSubFolderName1) && !string.IsNullOrEmpty(folderIds.ClientMainFolderId))
                {
                    // Creating the sub main request builder.
                    IMailFolderRequestBuilder subRequestBuilder1 = await GetRequestBuilderAsync(graphClient,
                                                                                                clientEmail,
                                                                                                folderIds.ClientMainFolderId,
                                                                                                null);
                    if (subRequestBuilder1 != null)
                    {
                        // Getting the sub inbox id.
                        IMailFolderRequestBuilder subFolderBuilder1 = await GetChildFolderIdByName(subRequestBuilder1,
                                                                                                   clientSubFolderName1,
                                                                                                   programSettings.MaxSubEmailFolders);

                        if (subFolderBuilder1 == null)
                        {
                            WriteLogClass.WriteToLog(0, $"Client sub folder1 ID is empty ....", 0);
                            return null;
                        }

                        // Setting the sub inbox id.
                        folderIds.ClientSubFolderId1 = subFolderBuilder1.Request().Select("id").GetAsync().Result.Id;
                    }                    
                }

                // Getting the last sub inboxe ID.
                if (!string.IsNullOrEmpty(clientSubfolderName2) && !string.IsNullOrEmpty(folderIds.ClientSubFolderId1))
                {
                    // Creating the sub main request builder.
                    IMailFolderRequestBuilder subRequestBuilder2 = await GetRequestBuilderAsync(graphClient,
                                                                                                clientEmail,
                                                                                                folderIds.ClientMainFolderId,
                                                                                                folderIds.ClientSubFolderId1);

                    if (subRequestBuilder2 != null)
                    {
                        // Getting the sub inbox id.
                        IMailFolderRequestBuilder subFolderBuilder2 = await GetChildFolderIdByName(subRequestBuilder2,
                                                                            clientSubfolderName2,
                                                                            programSettings.MaxSubEmailFolders);

                        if (subFolderBuilder2 == null)
                        {
                            WriteLogClass.WriteToLog(0, $"Client sub folder2 ID is empty ....", 0);
                            return null;
                        }

                        // Setting the sub inbox id.
                        folderIds.ClientSubFolderId2 = subFolderBuilder2.Request().Select("id").GetAsync().Result.Id;
                    }
                }
                return folderIds;
            }
            catch (Exception ex)
            {
                WriteLogClass.WriteToLog(0, $"Exception at getting folder id's: {ex.Message}", 0);
                return null;
            }            
        }

        /// <summary>
        /// Get's the ID of the error folder. Which used to move the emails which get marked as errored emails.
        /// </summary>
        /// <param name="graphClient"></param>
        /// <param name="clientEmail"></param>
        /// <param name="mainFolderId"></param>
        /// <param name="subFolderId1"></param>
        /// <param name="subFolderId2"></param>
        /// <returns></returns>
        public static async Task<string> GetErrorFolderId(IMailFolderRequestBuilder requestBuilder)
        {
            IMailFolderChildFoldersCollectionPage errorFolderDetails = await requestBuilder
                                                                             .ChildFolders
                                                                             .Request()
                                                                             .GetAsync(); 
            if (errorFolderDetails == null)
            {
                WriteLogClass.WriteToLog(0, $"Error folder details is null ....", 0);
                return null;
            }

            return errorFolderDetails.FirstOrDefault(efd => efd.DisplayName.ToLower() == MagicWords.error)!.Id;
        }

        /// <summary>
        /// Builds the main request to retrive the inbox id's.
        /// </summary>
        /// <param name="graphClient"></param>
        /// <param name="inEmail"></param>
        /// <param name="mainFolderId"></param>
        /// <param name="subFolderId1"></param>
        /// <returns>Request builder is returned.</returns>
        private static async Task<IMailFolderRequestBuilder> GetRequestBuilderAsync(GraphServiceClient graphClient,
                                                                                    string inEmail,
                                                                                    string mainFolderId,
                                                                                    string subFolderId1)
        {
            // TODO: Convert this to a foreach loop.

            try
            {
                IMailFolderRequestBuilder returnBuilder = graphClient
                                                          .Users[$"{inEmail}"]
                                                          .MailFolders[$"{MagicWords.inbox}"];

                if (!string.IsNullOrWhiteSpace(mainFolderId))
                {
                    returnBuilder = graphClient
                                    .Users[$"{inEmail}"]
                                    .MailFolders[$"{MagicWords.inbox}"]
                                    .ChildFolders[$"{mainFolderId}"];
                }

                if (!string.IsNullOrWhiteSpace(subFolderId1))
                {
                    returnBuilder = graphClient
                                    .Users[$"{inEmail}"]
                                    .MailFolders[$"{MagicWords.inbox}"]
                                    .ChildFolders[$"{mainFolderId}"]
                                    .ChildFolders[$"{subFolderId1}"];
                }

                return returnBuilder;
            }
            catch (Exception ex)
            {
                WriteLogClass.WriteToLog(0, $"Exception at getting request builder: {ex.Message}", 0);
                return null;
            }
        }

        /// <summary>
        /// Search for the child folder id by name.
        /// </summary>
        /// <param name="requestBuilder"></param>
        /// <param name="childFolderName"></param>
        /// <param name="maxFoldersToLoad"></param>
        /// <returns>Return the child folder ID.</returns>
        private static async Task<IMailFolderRequestBuilder> GetChildFolderIdByName(IMailFolderRequestBuilder requestBuilder,
                                                                                    string childFolderName,
                                                                                    int maxFoldersToLoad)
        {
            try
            {
                IMailFolderChildFoldersCollectionPage childFolderRequestBuilder = await requestBuilder
                                                                                        .ChildFolders
                                                                                        .Request()
                                                                                        .Top(maxFoldersToLoad)
                                                                                        .Filter($"displayName eq '{childFolderName}'")
                                                                                        .GetAsync();

                MailFolder childFolderId = childFolderRequestBuilder.FirstOrDefault();

                if (childFolderId != null)
                {
                    return requestBuilder.ChildFolders[childFolderId.Id];
                }

                return null;
            }
            catch (Exception ex)
            {
                WriteLogClass.WriteToLog(0, $"Exception at getting child folder id: {ex.Message}", 0);
                return null;
            }
        }
    }
}
