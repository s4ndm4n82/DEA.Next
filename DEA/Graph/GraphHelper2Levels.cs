using Microsoft.Graph;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using DEA;
using ReadSettings;
using WriteLog;
using FolderCleaner;
using GraphHelpFunctions;
using GetRecipientEmail; // Might need it later.
using CreateMetadataFile; // Might need to use this later so leaving it.

namespace DEA2Levels
{
    internal class GraphHelper2Levels
    {
        public static async Task GetEmailsAttacmentsAccount([NotNull] GraphServiceClient graphClient, string clientEmail, string mainMailFolder, string subFolder1, string subFolder2)
        {
            // Parameters read from the config files.
            var ConfigParam = new ReadSettingsClass();

            int MaxAmountOfEmails = ConfigParam.MaxLoadEmails;
            string ImportFolderPath = Path.Combine(ConfigParam.ImportFolderLetter, ConfigParam.ImportFolderPath);

            var folderIds = await T.getChlidFolderIds<T.clientFolderIdsObject>(graphClient, clientEmail, mainMailFolder, subFolder1, subFolder2);

            WriteLogClass.WriteToLog(3, $"Folder id with in GetAttachments: {folderIds}", string.Empty);
        }
    }
}