using System.Diagnostics.CodeAnalysis;
using AppConfigReader;
using DEA.Next.HelperClasses.OtherFunctions;
using Microsoft.Graph;
using WriteLog;

namespace DEA.Next.Graph.GraphEmailInboxFunctions;

internal class GetDeletedItemsId
{
    /// <summary>
    ///     Get the deleted items inbox id.
    /// </summary>
    /// <param name="graphClient">Graph authentication.</param>
    /// <param name="clientEmail">Clients email.</param>
    /// <returns>Returns the ID.</returns>
    public static async Task<string> GetDeletedItemsIdAsync([NotNull] GraphServiceClient? graphClient,
        string clientEmail)
    {
        // Max inbox folders to load.
        var jsonData = AppConfigReaderClass.ReadAppDotConfig();
        var programSettings = jsonData.ProgramSettings;

        try
        {
            // Get all the main folder details.
            var emailFolders = await graphClient
                .Users[clientEmail]
                .MailFolders
                .Request()
                .Top(programSettings.MaxMainEmailFolders)
                .GetAsync();

            var deletedItemsId = emailFolders
                .FirstOrDefault(folder => folder.DisplayName
                    .Equals(MagicWords.DeletedItems, StringComparison.OrdinalIgnoreCase))?.Id;

            return string.IsNullOrEmpty(deletedItemsId) ? string.Empty : deletedItemsId;
        }
        catch (Exception e)
        {
            WriteLogClass.WriteToLog(0
                , $"Error getting deleted items id: {e.Message} ...."
                , 0);
            throw;
        }
    }
}