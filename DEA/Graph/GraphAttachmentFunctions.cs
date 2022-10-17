using Microsoft.Graph;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using ReadSettings;
using WriteLog;

namespace GraphAttachmentFunctions
{
    internal class GraphAttachmentFunctionsClass
    {
        public static async Task<T> GetMessagesWithAttachments<T>([NotNull] GraphServiceClient graphClient, string inEmail, string mainFolderId, string subFolderId1, string subFolderId2)
        {
            if (!string.IsNullOrEmpty(mainFolderId))
            {
                IMailFolderMessagesCollectionPage messages = await graphClient.Users[$"{inEmail}"].MailFolders["Inbox"]
            }
        }
    }
}
