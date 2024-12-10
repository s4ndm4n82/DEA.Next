using AppConfigReader;
using DEA.Next.HelperClasses.OtherFunctions;
using Microsoft.Graph;
using System.Diagnostics.CodeAnalysis;
using WriteLog;

namespace DEA.Next.Graph.GraphEmailInboxFunctions;

internal class GetDeletedItemsId
{
    /// <summary>
    /// Get the deleted items inbox id.
    /// </summary>
    /// <param name="graphClient">Graph authentication.</param>
    /// <param name="clientEmail">Clients email.</param>
    /// <returns>Returns the ID.</returns>
    public static async Task<string> GetDeletedItemsIdAsync([NotNull]GraphServiceClient graphClient, string clientEmail)
    {
        // Max inbox folders to load.
        AppConfigReaderClass.AppSettingsRoot jsonData = AppConfigReaderClass.ReadAppDotConfig();
        AppConfigReaderClass.Programsettings programSettings = jsonData.ProgramSettings;

        // Get all the main folder details.
        IUserMailFoldersCollectionPage emailFolders = await graphClient
            .Users[clientEmail]
            .MailFolders
            .Request()
            .Top(programSettings.MaxMainEmailFolders)
            .GetAsync();

        // Check if the email folders is null.
        if (emailFolders == null)
        {
            WriteLogClass.WriteToLog(0, $"Email folders is null ....", 0);
            return string.Empty;
        }

        // Get the deleted items id.
        return emailFolders.FirstOrDefault(folder => folder.DisplayName == MagicWords.Deleteditems).Id;
    }
}