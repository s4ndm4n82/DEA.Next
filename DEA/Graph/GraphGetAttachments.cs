using Microsoft.Graph;
using System.Diagnostics.CodeAnalysis;
using GetMailFolderIds;
using GraphAttachmentFunctions;
using WriteLog;
using AppConfigReader;
using ProcessStatusMessageSetter;

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
            AppConfigReaderClass.AppSettingsRoot jsonData = AppConfigReaderClass.ReadAppDotConfig();
            AppConfigReaderClass.Programsettings maxMalis = jsonData.ProgramSettings;

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
                                                                                              , maxMalis.MaxEmails
                                                                                              , customerId);

                WriteLogClass.WriteToLog(ProcessStatusMessageSetterClass.SetMessageTypeOther(result), ProcessStatusMessageSetterClass.SetProcessStatusOther(result, "email"), 2);
            }
            return result;
        }
    }
}