using AppConfigReader;
using DEA.Next.Graph.GraphEmailInboxFunctions;
using DEA.Next.Graph.GraphHelperClasses;
using GetMailFolderIds;
using Microsoft.Graph;
using System.Diagnostics.CodeAnalysis;
using WriteLog;

namespace DEA.Next.Graph.GraphEmailBodyRelatedActions
{
    internal class GraphReadEmailBody
    {
        public static async Task<int> StartEmailBodyRead([NotNull] GraphServiceClient graphClient,
                                                         string clientEmail,
                                                         string mainInbox,
                                                         string secondInbox,
                                                         string thirdInbox,
                                                         int clientId)
        {
            // Reading the application config client to retrive max email amount to load.
            AppConfigReaderClass.AppSettingsRoot jsonData = AppConfigReaderClass.ReadAppDotConfig();
            AppConfigReaderClass.Programsettings maxMalis = jsonData.ProgramSettings;

            // Converting email folder names to ids
            GetMailFolderIdsClass.ClientFolderId folderIds = await GetMailFolderIdsClass.GetChlidFolderIds<GetMailFolderIdsClass>(graphClient,
                                                                                                                                  clientEmail,
                                                                                                                                  mainInbox,
                                                                                                                                  secondInbox,
                                                                                                                                  thirdInbox);
            try
            {
                if (folderIds == null)
                {
                    WriteLogClass.WriteToLog(0, "Folder Ids are null", 0);
                    return 4;
                }

                // Creating the graph request builder.
                IMailFolderRequestBuilder requestBuilder = await CreateRequestBuilderClass.CreatRequestBuilder(graphClient,
                                                                                                               folderIds.ClientMainFolderId,
                                                                                                               folderIds.ClientSubFolderId1,
                                                                                                               folderIds.ClientSubFolderId2,
                                                                                                               clientEmail);
                // Getting the deleted items folder id.
                string deletedItemsId = await GetDeletedItemsId.GetDeletedItemsIdAsync(graphClient, clientEmail);

                if (requestBuilder == null)
                {
                    WriteLogClass.WriteToLog(0, "Request builder at email body reader is null", 0);
                    return 4;
                }

                // Reading the email body and sending it to the TPS.

            }
            catch (Exception ex)
            {
                WriteLogClass.WriteToLog(0, $"Exception at start email body reader: {ex.Message}", 0);
            }

            return 0;
        }
    }
}
