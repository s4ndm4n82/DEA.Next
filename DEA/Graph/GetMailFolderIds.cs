using Microsoft.Graph;
using System.Diagnostics.CodeAnalysis;
using WriteLog;

namespace GetMailFolderIds
{
    internal class GetMailFolderIdsClass
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
        public static async Task<clientFolderId> GetChlidFolderIds<T>([NotNull] GraphServiceClient graphClient, string clientEmail, string clientMainFolderName, string clientSubFolderName1, string clientSubfolderName2)
        {
            var folderId = new clientFolderId();

            if (!string.IsNullOrEmpty(clientMainFolderName))
            {
                try
                {
                    _inboxClientMainFolder = await graphClient.Users[$"{clientEmail}"].MailFolders["Inbox"]
                                         .ChildFolders
                                         .Request()
                                         .GetAsync();

                    folderId.clientMainFolderId = _inboxClientMainFolder.FirstOrDefault(x => x.DisplayName == clientMainFolderName)!.Id;
                }
                catch (Exception ex)
                {
                    WriteLogClass.WriteToLog(3, $"Exception at getting main folder id: {ex.Message}", string.Empty);
                }

            }

            if (!string.IsNullOrEmpty(folderId.clientMainFolderId) && !string.IsNullOrEmpty(clientSubFolderName1) && string.IsNullOrEmpty(clientSubfolderName2))
            {
                try
                {
                    _inboxSubFolder1 = await graphClient.Users[$"{clientEmail}"].MailFolders["Inbox"]
                                   .ChildFolders[$"{folderId.clientMainFolderId}"]
                                   .ChildFolders
                                   .Request()
                                   .GetAsync();

                    folderId.clientSubFolderId1 = _inboxSubFolder1.FirstOrDefault(y => y.DisplayName == clientSubFolderName1)!.Id;
                }
                catch (Exception ex)
                {
                    WriteLogClass.WriteToLog(3, $"Exception at getting sub folder1 id: {ex.Message}", string.Empty);
                }
            }

            if (!string.IsNullOrEmpty(folderId.clientMainFolderId) && !string.IsNullOrEmpty(folderId.clientSubFolderId1) && !string.IsNullOrEmpty(clientSubfolderName2))
            {
                try
                {
                    _inboxSubFolder2 = await graphClient.Users[$"{clientEmail}"].MailFolders["Inbox"]
                                   .ChildFolders[$"{folderId.clientMainFolderId}"]
                                   .ChildFolders[$"{folderId.clientSubFolderId1}"]
                                   .ChildFolders
                                   .Request()
                                   .GetAsync();

                    folderId.clientSubFolderId2 = _inboxSubFolder2.FirstOrDefault(z => z.DisplayName == clientSubfolderName2)!.Id;
                }
                catch (Exception ex)
                {
                    WriteLogClass.WriteToLog(3, $"Exception at getting sub folder2 id: {ex.Message}", string.Empty);
                }
            }

            return folderId;
            
        }
    }
}
