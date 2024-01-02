using GraphMoveEmailsToErrorFolderClass;
using Microsoft.Graph;
using WriteLog;

namespace GraphMoveEmailsToExportClass
{
    internal class GraphMoveEmailsToExport
    {
        /// <summary>
        /// After download ends with succession this function will be called. Which will move the completed email to another folder.
        /// Normally it's called "Exported".
        /// </summary>
        /// <param name="graphClient"></param>
        /// <param name="mainFolderId"></param>
        /// <param name="subFolderId1"></param>
        /// <param name="subFolderId2"></param>
        /// <param name="messageId"></param>
        /// <param name="messageSubject"></param>
        /// <param name="inEmail"></param>
        /// <returns>A bool value (true or false)</returns>
        public static async Task<bool> MoveEmailsToExport(GraphServiceClient graphClient,
                                                          string mainFolderId,
                                                          string subFolderId1,
                                                          string subFolderId2,
                                                          string messageId,
                                                          string messageSubject,
                                                          string inEmail)
        {
            IMailFolderChildFoldersCollectionPage moveLocation;
            MailFolder exportFolder = null!;

            if (!string.IsNullOrEmpty(mainFolderId) && string.IsNullOrEmpty(subFolderId1) && string.IsNullOrEmpty(subFolderId2))
            {
                try
                {
                    moveLocation = await graphClient.Users[$"{inEmail}"].MailFolders["Inbox"]
                               .ChildFolders[$"{mainFolderId}"]
                               .ChildFolders
                               .Request()
                               .GetAsync();

                    exportFolder = moveLocation.FirstOrDefault(x => x.DisplayName == "Exported")!;
                }
                catch (Exception ex)
                {
                    WriteLogClass.WriteToLog(0, $"Exception at detination folder name 1st if: {ex.Message}", 0);
                }
            }

            if (!string.IsNullOrEmpty(mainFolderId) && !string.IsNullOrEmpty(subFolderId1) && string.IsNullOrEmpty(subFolderId2))
            {
                try
                {
                    moveLocation = await graphClient.Users[$"{inEmail}"].MailFolders["Inbox"]
                               .ChildFolders[$"{mainFolderId}"]
                               .ChildFolders[$"{subFolderId1}"]
                               .ChildFolders
                               .Request()
                               .GetAsync();

                    exportFolder = moveLocation.FirstOrDefault(x => x.DisplayName == "Exported")!;
                }
                catch (Exception ex)
                {
                    WriteLogClass.WriteToLog(0, $"Exception at detination folder name 2nd if: {ex.Message}", 0);
                }
            }

            if (!string.IsNullOrEmpty(mainFolderId) && !string.IsNullOrEmpty(subFolderId1) && !string.IsNullOrEmpty(subFolderId2))
            {
                try
                {
                    moveLocation = await graphClient.Users[$"{inEmail}"].MailFolders["Inbox"]
                               .ChildFolders[$"{mainFolderId}"]
                               .ChildFolders[$"{subFolderId1}"]
                               .ChildFolders[$"{subFolderId2}"]
                               .ChildFolders
                               .Request()
                               .GetAsync();

                    exportFolder = moveLocation.FirstOrDefault(x => x.DisplayName == "Exported")!;
                }
                catch (Exception ex)
                {
                    WriteLogClass.WriteToLog(0, $"Exception at detination folder name 3rd if: {ex.Message}", 0);
                }
            }

            if (await GraphMoveEmailsToErrorFolder.MoveEmailsToErrorFolder(graphClient,
                                                                           mainFolderId,
                                                                           subFolderId1,
                                                                           subFolderId2,
                                                                           messageId,
                                                                           exportFolder.Id,
                                                                           inEmail))
            {
                WriteLogClass.WriteToLog(1, $"Email {messageSubject} moved to export folder ...", 2);
                return true;
            }
            else
            {
                WriteLogClass.WriteToLog(1, $"Email {messageSubject} not moved to export folder ...", 2);
                return false;
            }
        }
    }
}
