using Microsoft.Graph;
using System.Diagnostics.CodeAnalysis;
using GetMailFolderIds;
using WriteLog;
using AppConfigReader;
using ProcessStatusMessageSetter;
using DEA.Next.Graph.GraphHelperClasses;
using DEA.Next.Graph.GraphEmailInboxFunctions;
using DEA.Next.HelperClasses.OtherFunctions;
using DEA.Next.Graph.GraphAttachmentRetlatedActions;
using UserConfigSetterClass;
using UserConfigRetriverClass;
using DEA.Next.Graph.GraphEmailBodyRelatedActions;

namespace DEA.Next.Graph.GraphEmailActons
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
        public static async Task<int> StartAttacmentDownload([NotNull] GraphServiceClient graphClient,
                                                                       string clientEmail,
                                                                       string mainMailFolder,
                                                                       string subFolder1,
                                                                       string subFolder2,
                                                                       int customerId)
        {
            // Parameters read from the application config files.
            AppConfigReaderClass.AppSettingsRoot jsonData = AppConfigReaderClass.ReadAppDotConfig();
            AppConfigReaderClass.Programsettings maxMails = jsonData.ProgramSettings;

            // Read from the client config file.
            UserConfigSetter.Emaildetails emailDetails = await UserConfigRetriver.RetriveEmailConfigById(customerId);

            int result = 0;
            // Get the folder ID's after searching the folder names.
            GetMailFolderIdsClass.ClientFolderId folderIds = await GetMailFolderIdsClass.GetChlidFolderIds<GetMailFolderIdsClass>(graphClient,
                                                                                                                                  clientEmail,
                                                                                                                                  mainMailFolder,
                                                                                                                                  subFolder1,
                                                                                                                                  subFolder2);
            try
            {
                if (folderIds == null)
                {
                    WriteLogClass.WriteToLog(0, $"Folders were not found ....", 0);
                    return 4;
                }

                // Write the mail box path.
                WriteTheMailBoxPath(mainMailFolder, subFolder1, subFolder2);

                // Create the request builder.
                IMailFolderRequestBuilder requestBuilder = await CreateRequestBuilderClass.CreatRequestBuilder(graphClient,
                                                                                                               folderIds.ClientMainFolderId,
                                                                                                               folderIds.ClientSubFolderId1,
                                                                                                               folderIds.ClientSubFolderId2,
                                                                                                               clientEmail);

                string deletedItemsId = await GetDeletedItemsId.GetDeletedItemsIdAsync(graphClient, clientEmail);

                // Check if the request builder is null.
                if (requestBuilder == null)
                {
                    WriteLogClass.WriteToLog(0, $"Failed to create request builder ....", 0);
                    return 4;
                }

                // If the email body need to be read this will be used.
                if (emailDetails.EmailRead == 1 && emailDetails.EmailList.Any())
                {
                    return await GraphReadEmailBody.ReadEmailBodyAsync(requestBuilder, maxMails.MaxEmails, customerId);
                }

                // Initiate the email attachment download and send them to the web service. Should return a bool value.
                result = await GraphAttachmentFunctionsClass.GetMessagesWithAttachments(requestBuilder,
                                                                                        clientEmail,
                                                                                        deletedItemsId,
                                                                                        maxMails.MaxEmails,
                                                                                        customerId);

                // Log the result.
                WriteLogClass.WriteToLog(ProcessStatusMessageSetterClass.SetMessageTypeOther(result),
                                         ProcessStatusMessageSetterClass.SetProcessStatusOther(result, MagicWords.email), 2);
            }
            catch (Exception ex)
            {
                WriteLogClass.WriteToLog(0, $"Exception at start email attachment download: {ex.Message}", 0);
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
                List<string> folderList = new() { mainMailFolder, subFolder1, subFolder2 };
                // Remove the empty ones.
                folderList.RemoveAll(string.IsNullOrEmpty);

                if (folderList.Any())
                {
                    WriteLogClass.WriteToLog(1, $"Starting attachment download process from inbox /{string.Join("/", folderList)} ....", 2);
                }
            }
            else
            {
                WriteLogClass.WriteToLog(1, "Folder names are empty ....", 2);
            }
        }
    }
}