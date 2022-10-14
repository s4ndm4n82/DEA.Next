using Microsoft.Graph;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using DEA;
using ReadSettings;
using WriteLog;
using GetRecipientEmail;
using FolderCleaner;

namespace DEAHelper1Leve
{
    internal class GraphHelper1LevelClass
    {
        public static async Task GetEmailsAttacments1Level([NotNull] GraphServiceClient graphClient, string clientEmail)
        {
            // Parameters read from the config files.
            var ConfigParam = new ReadSettingsClass();

            int MaxAmountOfEmails = ConfigParam.MaxLoadEmails;
            string ImportFolderPath = Path.Combine(ConfigParam.ImportFolderLetter, ConfigParam.ImportFolderPath);
           
        }
    }
}