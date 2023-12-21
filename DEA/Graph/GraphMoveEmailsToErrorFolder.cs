using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WriteLog;

namespace GraphMoveEmailsToErrorFolderClass
{
    internal class GraphMoveEmailsToErrorFolder
    {
        /// <summary>
        /// Moves the email to Downloded folder.
        /// </summary>
        /// <param name="FirstFolderId"></param>
        /// <param name="SecondFolderId"></param>
        /// <param name="ThirdFolderId"></param>
        /// <param name="MsgId"></param>
        /// <param name="DestiId"></param>
        /// <param name="_Email"></param>
        /// <returns></returns>
        public static async Task<bool> MoveEmailsToErrorFolder(GraphServiceClient graphClient,
                                                               string FirstFolderId,
                                                               string SecondFolderId,
                                                               string ThirdFolderId,
                                                               string MsgId,
                                                               string DestiId,
                                                               string _Email)
        {
            try
            {
                if (string.IsNullOrEmpty(ThirdFolderId) && string.IsNullOrEmpty(SecondFolderId))
                {
                    //Graph api call to move the email message.
                    await graphClient!.Users[$"{_Email}"].MailFolders["Inbox"]
                        .ChildFolders[$"{FirstFolderId}"]
                        .Messages[$"{MsgId}"]
                        .Move(DestiId)
                        .Request()
                        .PostAsync();
                }
                else if (string.IsNullOrEmpty(ThirdFolderId))
                {
                    //Graph api call to move the email message.
                    await graphClient!.Users[$"{_Email}"].MailFolders["Inbox"]
                        .ChildFolders[$"{FirstFolderId}"]
                        .ChildFolders[$"{SecondFolderId}"]
                        .Messages[$"{MsgId}"]
                        .Move(DestiId)
                        .Request()
                        .PostAsync();
                }
                else
                {
                    //Graph api call to move the email message.
                    await graphClient!.Users[$"{_Email}"].MailFolders["Inbox"]
                        .ChildFolders[$"{FirstFolderId}"]
                        .ChildFolders[$"{SecondFolderId}"]
                        .ChildFolders[$"{ThirdFolderId}"]
                        .Messages[$"{MsgId}"]
                        .Move(DestiId)
                        .Request()
                        .PostAsync();
                }
                return true;
            }
            catch (Exception ex)
            {
                WriteLogClass.WriteToLog(0, $"Exception at moving emails to folders: {ex.Message}", 0);
                return false;
            }
        }
    }
}
