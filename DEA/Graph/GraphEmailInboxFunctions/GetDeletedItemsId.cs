using AppConfigReader;
using DEA.Next.HelperClasses.OtherFunctions;
using Microsoft.Graph;
using System.Diagnostics.CodeAnalysis;
using WriteLog;

namespace DEA.Next.Graph.GraphEmailInboxFunctions
{
    internal class GetDeletedItemsId
    {
        public static async Task<string> GetDeletedItemsIdAsync([NotNull]GraphServiceClient graphClient, string clientEmail)
        {
            AppConfigReaderClass.AppSettingsRoot jsonData = AppConfigReaderClass.ReadAppDotConfig();
            AppConfigReaderClass.Programsettings programSettings = jsonData.ProgramSettings;

            IUserMailFoldersCollectionPage emailFolders = await graphClient
                                                                .Users[clientEmail]
                                                                .MailFolders
                                                                .Request()
                                                                .Top(programSettings.MaxMainEmailFolders)
                                                                .GetAsync();

            if (emailFolders == null)
            {
                WriteLogClass.WriteToLog(0, $"Email folders is null ....", 0);
                return string.Empty;
            }

            return emailFolders.FirstOrDefault(folder => folder.DisplayName == MagicWords.deleteditems).Id;
        }
    }
}
