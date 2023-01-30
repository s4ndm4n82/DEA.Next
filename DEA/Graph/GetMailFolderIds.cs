using Microsoft.Graph;
using System.Diagnostics.CodeAnalysis;
using WriteLog;

namespace GetMailFolderIds
{
    internal class GetMailFolderIdsClass
    {
    /// <summary>
    /// Get inbox folder ID's. 
    /// </summary>
        private static IMailFolderChildFoldersCollectionPage? _inboxClientMainFolder;
        private static IMailFolderChildFoldersCollectionPage? _inboxSubFolder1;
        private static IMailFolderChildFoldersCollectionPage? _inboxSubFolder2;

        public class clientFolderId
        {
            public string? clientMainFolderId { get; set; }
            public string? clientSubFolderId1 { get; set; }
            public string? clientSubFolderId2 { get; set; }
        }

        /// <summary>
        /// Get's the inbox id's form the sub inboxes on the email server. This will go deep as 3 levels auto matically.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="graphClient"></param>
        /// <param name="clientEmail"></param>
        /// <param name="clientMainFolderName"></param>
        /// <param name="clientSubFolderName1"></param>
        /// <param name="clientSubfolderName2"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Get's the ID of the error folder. Which used to move the emails which get marked as errored emails.
        /// </summary>
        /// <param name="graphClient"></param>
        /// <param name="clientEmail"></param>
        /// <param name="mainFolderId"></param>
        /// <param name="subFolderId1"></param>
        /// <param name="subFolderId2"></param>
        /// <returns></returns>
        public static async Task<string> GetErrorFolderId([NotNull] GraphServiceClient graphClient, string clientEmail, string mainFolderId, string subFolderId1, string subFolderId2)
        {
            IMailFolderChildFoldersCollectionPage? errorFolderDetails = null;
            string errorFolerId = string.Empty;

            if (!string.IsNullOrWhiteSpace(mainFolderId) && string.IsNullOrWhiteSpace(subFolderId1) && string.IsNullOrWhiteSpace(subFolderId2))
            {
                errorFolderDetails = await graphClient.Users[$"{clientEmail}"].MailFolders["Inbox"]
                                         .ChildFolders[$"{mainFolderId}"]
                                         .ChildFolders
                                         .Request()
                                         .GetAsync();

                errorFolerId = errorFolderDetails.FirstOrDefault(efd => efd.DisplayName.ToLower() == "error")!.Id;
            }

            if (!string.IsNullOrWhiteSpace(mainFolderId) && !string.IsNullOrWhiteSpace(subFolderId1) && string.IsNullOrWhiteSpace(subFolderId2))
            {
                errorFolderDetails = await graphClient.Users[$"{clientEmail}"].MailFolders["Inbox"]
                                    .ChildFolders[$"{mainFolderId}"]
                                    .ChildFolders[$"{subFolderId1}"]
                                    .ChildFolders
                                    .Request()
                                    .GetAsync();

                errorFolerId = errorFolderDetails.FirstOrDefault(efd => efd.DisplayName.ToLower() == "error")!.Id;
            }

            if (!string.IsNullOrWhiteSpace(mainFolderId) && !string.IsNullOrWhiteSpace(subFolderId1) && !string.IsNullOrWhiteSpace(subFolderId2))
            {
                errorFolderDetails = await graphClient.Users[$"{clientEmail}"].MailFolders["Inbox"]
                                    .ChildFolders[$"{mainFolderId}"]
                                    .ChildFolders[$"{subFolderId1}"]
                                    .ChildFolders[$"{subFolderId2}"]
                                    .ChildFolders
                                    .Request()
                                    .GetAsync();

                errorFolerId = errorFolderDetails.FirstOrDefault(efd => efd.DisplayName.ToLower() == "error")!.Id;
            }

            return errorFolerId;
        }
    }
}
