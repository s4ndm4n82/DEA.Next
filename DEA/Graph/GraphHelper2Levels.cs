using Microsoft.Graph;
using System.Diagnostics.CodeAnalysis;
using ReadSettings;
using GetMailFolderIds;
using GraphAttachmentFunctions;

namespace DEA2Levels
{
    internal class GraphHelper2Levels
    {
        /// <summary>
        /// Mainly this will start the email download process and the process to submit files to the web service.
        /// </summary>
        /// <param name="graphClient"></param>
        /// <param name="clientEmail"></param>
        /// <param name="mainMailFolder"></param>
        /// <param name="subFolder1"></param>
        /// <param name="subFolder2"></param>
        /// <returns></returns>
        public static async Task GetEmailsAttacments2Levels([NotNull] GraphServiceClient graphClient, string clientEmail, string mainMailFolder, string subFolder1, string subFolder2, int customerId)
        {
            // Parameters read from the config files.
            var ConfigParam = new ReadSettingsClass();

            int maxAmountOfEmails = ConfigParam.MaxLoadEmails;
            string importFolderPath = Path.Combine(ConfigParam.ImportFolderLetter, ConfigParam.ImportFolderPath);

            // Get the folder ID's after searching the folder names.
            var folderIds = await GetMailFolderIdsClass.GetChlidFolderIds<GetMailFolderIdsClass>(graphClient, clientEmail, mainMailFolder, subFolder1, subFolder2);

            // Initiate the email attachment download and send them to the web service. Should return a bool value.
            var messages = await GraphAttachmentFunctionsClass.GetMessagesWithAttachments(graphClient
                                                                                          , clientEmail
                                                                                          , folderIds.clientMainFolderId!
                                                                                          , folderIds.clientSubFolderId1!
                                                                                          , folderIds.clientSubFolderId2!
                                                                                          , maxAmountOfEmails
                                                                                          , customerId);
        }
    }
}