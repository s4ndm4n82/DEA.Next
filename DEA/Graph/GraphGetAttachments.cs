using Microsoft.Graph;
using System.Diagnostics.CodeAnalysis;
using ReadSettings;
using GetMailFolderIds;
using GraphAttachmentFunctions;
using WriteLog;

namespace GraphGetAttachments
{
    internal class GraphGetAttachmentsClass
    {
        /// <summary>
        /// Mainly this will start the email download process and the process to submit files to the web service.
        /// </summary>
        /// <param name="graphClient"></param>
        /// <param name="clientEmail"></param>
        /// <param name="mainMailFolder"></param>
        /// <param name="subFolder1"></param>
        /// <param name="subFolder2"></param>
        /// <returns></returns>
        public static async Task<int> GetEmailsAttacments([NotNull] GraphServiceClient graphClient, string clientEmail, string mainMailFolder, string subFolder1, string subFolder2, int customerId)
        {
            // Parameters read from the config files.
            var ConfigParam = new ReadSettingsClass();

            int maxAmountOfEmails = ConfigParam.MaxLoadEmails;
            int result = 0;
            // Get the folder ID's after searching the folder names.
            GetMailFolderIdsClass.clientFolderId folderIds = await GetMailFolderIdsClass.GetChlidFolderIds<GetMailFolderIdsClass>(graphClient, clientEmail, mainMailFolder, subFolder1, subFolder2);

            if (folderIds != null)
            {
                WriteLogClass.WriteToLog(3, $"Starting attachment download process ....", 2);

                // Initiate the email attachment download and send them to the web service. Should return a bool value.
                result = await GraphAttachmentFunctionsClass.GetMessagesWithAttachments(graphClient
                                                                                              , clientEmail
                                                                                              , folderIds.clientMainFolderId!
                                                                                              , folderIds.clientSubFolderId1!
                                                                                              , folderIds.clientSubFolderId2!
                                                                                              , maxAmountOfEmails
                                                                                              , customerId);
                // Selects the message body.
                string msgBody = result switch
                {
                    1 => "Email attachment downloade and sent to processing successfully ....",
                    2 => "Uploading file/s didn't complete successfully. Moved files to error folder ....",
                    3 => "No attachment or file type not supported. Email moved to error and forwarded to sender ....",
                    4 => "No new emails to process ... Process Ended ....",
                    _ => "Operation failed ....",
                };
                // Sets the message type.
                int msgType = result == 1 || result == 2 || result == 3 || result == 4 ? 1 : 0;

                WriteLogClass.WriteToLog(msgType, msgBody, 2);
            }
            return result;
        }
    }
}