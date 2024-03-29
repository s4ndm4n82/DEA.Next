﻿using GraphMoveEmailsrClass;
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
        public static async Task<bool> MoveEmailsToExport(IMailFolderRequestBuilder requestBuilder,                                                          
                                                          string messageId,
                                                          string messageSubject)
        {
            if (requestBuilder == null)
            {
                WriteLogClass.WriteToLog(0, $"Request builder is null ...", 0);
                return false;
            }

            IMailFolderChildFoldersCollectionPage emailMoveLocation = await requestBuilder
                                                                            .ChildFolders
                                                                            .Request()
                                                                            .GetAsync();

            string exportFolderId = emailMoveLocation.FirstOrDefault(fldr => fldr.DisplayName == "Exported").Id;

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
    }
}
