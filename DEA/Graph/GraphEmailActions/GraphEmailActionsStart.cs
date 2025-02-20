using AppConfigReader;
using DEA.Next.Graph.GraphAttachmentRelatedActions;
using DEA.Next.Graph.GraphEmailInboxFunctions;
using DEA.Next.Graph.GraphHelperClasses;
using DEA.Next.HelperClasses.OtherFunctions;
using GetMailFolderIds;
using Microsoft.Graph;
using ProcessStatusMessageSetter;
using WriteLog;

namespace DEA.Next.Graph.GraphEmailActions;

internal class GraphGetAttachmentsClass
{
    /// <summary>
    ///     Mainly this will start the email download process and the process to submit files to the web service.
    /// </summary>
    /// <param name="graphClient"></param>
    /// <param name="clientEmail"></param>
    /// <param name="mainMailFolder"></param>
    /// <param name="subFolder1"></param>
    /// <param name="subFolder2"></param>
    /// <param name="customerId"></param>
    /// <returns></returns>
    public static async Task<int> StartAttachmentDownload(GraphServiceClient? graphClient,
        string clientEmail,
        string mainMailFolder,
        string subFolder1,
        string subFolder2,
        Guid customerId)
    {
        // Parameters read from the application config files.
        var jsonData = AppConfigReaderClass.ReadAppDotConfig();
        var maxMails = jsonData.ProgramSettings;

        var result = 0;
        // Get the folder ID's after searching the folder names.
        var folderIds = await GetMailFolderIdsClass.GetChlidFolderIds<GetMailFolderIdsClass>(graphClient,
            clientEmail,
            mainMailFolder,
            subFolder1,
            subFolder2);

        try
        {
            // Write the mailbox path.
            WriteTheMailBoxPath(mainMailFolder, subFolder1, subFolder2);

            // Create the request builder.
            var requestBuilder = await CreateRequestBuilderClass.CreatRequestBuilder(graphClient,
                folderIds.ClientMainFolderId,
                folderIds.ClientSubFolderId1,
                folderIds.ClientSubFolderId2,
                clientEmail);

            var deletedItemsId = await GetDeletedItemsId.GetDeletedItemsIdAsync(graphClient, clientEmail);

            // Initiate the email attachment download and send them to the web service. Should return a bool value.
            result = await GraphAttachmentFunctionsClass.GetMessagesWithAttachments(requestBuilder,
                clientEmail,
                deletedItemsId,
                maxMails.MaxEmails,
                customerId);

            // Log the result.
            WriteLogClass.WriteToLog(ProcessStatusMessageSetterClass.SetMessageTypeOther(result),
                ProcessStatusMessageSetterClass.SetProcessStatusOther(result, MagicWords.Email), 2);
        }
        catch (Exception ex)
        {
            WriteLogClass.WriteToLog(0,
                $"Exception at start email attachment download: {ex.Message}",
                0);
        }

        return result;
    }

    private static void WriteTheMailBoxPath(string mainMailFolder,
        string subFolder1,
        string subFolder2)
    {
        if (!string.IsNullOrEmpty(mainMailFolder))
        {
            // List of inbox sub folder names.
            List<string> folderList = [mainMailFolder, subFolder1, subFolder2];
            // Remove the empty ones.
            folderList.RemoveAll(string.IsNullOrEmpty);

            if (folderList.Count != 0)
                WriteLogClass.WriteToLog(1,
                    $"Starting attachment download process from inbox /{string.Join("/", folderList)} ....",
                    2);
        }
        else
        {
            WriteLogClass.WriteToLog(1, "Folder names are empty ....", 2);
        }
    }
}