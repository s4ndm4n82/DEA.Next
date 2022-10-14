using Microsoft.Graph;
using System.Diagnostics.CodeAnalysis;
using DEA;
using WriteLog;
using static GraphHelpFunctions.T;

namespace GraphHelpFunctions
{
    internal class T
    {
        private static IMailFolderChildFoldersCollectionPage? _inboxClientMainFolder;
        private static IMailFolderChildFoldersCollectionPage? _inboxSubFolder1;
        private static IMailFolderChildFoldersCollectionPage? _inboxSubFolder2;

        public class clientFolderId
        {
            public string? clientMainFolderId { get; set; }
            public string? clientSubFolderId1 { get; set; }
            public string? clientSubFolderId2 { get; set; }
        }
        private static async Task<T> getChlidFolderIds<T>([NotNull] GraphServiceClient graphClient, string clientEmail, string clientMainFolderName, string clientSubFolderName1, string clientSubfolderName2)
        {
            var folderId = new clientFolderId;

            if (!string.IsNullOrEmpty(clientMainFolderName))
            {
                // Search for the subfolder named error.
                var searchMainClientFolder = new List<QueryOption>
                                            {
                                                new QueryOption ("filer", $"displayName eq %27{clientMainFolderName}%27")
                                            };

                _inboxClientMainFolder = await graphClient.Users[$"{clientEmail}"].MailFolders["Inbox"]
                                            .ChildFolders
                                            .Request(searchMainClientFolder)
                                            .GetAsync();

                 folderId.clientMainFolderId = _inboxClientMainFolder.FirstOrDefault(x => x.DisplayName == clientMainFolderName)!.Id;

                WriteLogClass.WriteToLog(3, $"Client main folder id: {id}", string.Empty);
            }

            return folderId.clientMainFolderId;
            
        }

        public static implicit operator T(string v)
        {
            throw new NotImplementedException();
        }
    }
}
