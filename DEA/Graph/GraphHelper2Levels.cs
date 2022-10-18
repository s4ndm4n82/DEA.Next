using Microsoft.Graph;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using DEA;
using ReadSettings;
using WriteLog;
using FolderCleaner;
using GetMailFolderIds;
using GraphAttachmentFunctions;

namespace DEA2Levels
{
    internal class GraphHelper2Levels
    {
        public static async Task GetEmailsAttacments2Levels([NotNull] GraphServiceClient graphClient, string clientEmail, string mainMailFolder, string subFolder1, string subFolder2)
        {
            // Parameters read from the config files.
            var ConfigParam = new ReadSettingsClass();

            int MaxAmountOfEmails = ConfigParam.MaxLoadEmails;
            string ImportFolderPath = Path.Combine(ConfigParam.ImportFolderLetter, ConfigParam.ImportFolderPath);

            var folderIds = await GetMailFolderIdsClass.GetChlidFolderIds<GetMailFolderIdsClass>(graphClient, clientEmail, mainMailFolder, subFolder1, subFolder2);

            var messages = await GraphAttachmentFunctionsClass.GetMessagesWithAttachments(graphClient, clientEmail, folderIds.clientMainFolderId!, folderIds.clientSubFolderId1!, folderIds.clientSubFolderId2!);
        }
    }
}