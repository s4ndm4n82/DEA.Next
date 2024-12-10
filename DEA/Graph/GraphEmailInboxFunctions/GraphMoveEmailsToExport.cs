using DEA.Next.HelperClasses.OtherFunctions;
using GraphMoveEmailsrClass;
using Microsoft.Graph;
using WriteLog;

namespace GraphMoveEmailsToExportClass;

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
    public static async Task<bool> MoveEmailsToExport(IMailFolderRequestBuilder requestBuilder,                                                          
        string messageId,
        string messageSubject)
    {
        try
        {
            // Checking if the request builder is null.
            if (requestBuilder == null)
            {
                WriteLogClass.WriteToLog(0, $"Request builder is null ...", 0);
                return false;
            }

            // Getting the list of child folders.
            IMailFolderChildFoldersCollectionPage emailMoveLocation = await requestBuilder
                .ChildFolders
                .Request()
                .GetAsync();
            // Get the ID of the export folder.
            string exportFolderId = emailMoveLocation.FirstOrDefault(fldr => fldr.DisplayName == MagicWords.Exported).Id;

            // Checking if the export folder exists.
            if (string.IsNullOrWhiteSpace(exportFolderId))
            {
                WriteLogClass.WriteToLog(0, $"Export folder not found ....", 0);
                return false;
            }

            // Moving the email to the export folder.
            if (await GraphMoveEmailsFolder.MoveEmailsToAnotherFolder(requestBuilder,
                    messageId,
                    exportFolderId))
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
        catch (Exception ex)
        {
            WriteLogClass.WriteToLog(0, $"Exception error moving email: {ex.Message}", 0);
            return false;
        }
    }
}