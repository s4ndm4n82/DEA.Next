using DEA.Next.Graph.GraphHelperClasses;
using Microsoft.Graph;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using WriteLog;

namespace GraphMoveEmailsToErrorFolderClass
{
    internal class GraphMoveEmailsToErrorFolder
    {
        /// <summary>
        /// Moves the email to Downloded folder.
        /// </summary>
        /// <param name="firstFolderId"></param>
        /// <param name="secondFolderId"></param>
        /// <param name="thirdFolderId"></param>
        /// <param name="MsgId"></param>
        /// <param name="DestiId"></param>
        /// <param name="_Email"></param>
        /// <returns></returns>
        public static async Task<bool> MoveEmailsToErrorFolder(IMailFolderRequestBuilder requestBuilder,
                                                               string MsgId,
                                                               string DestiId)
        {
            try
            {
                Message returnResult = await requestBuilder
                                       .Messages[$"{MsgId}"]
                                       .Move(DestiId)
                                       .Request()
                                       .PostAsync();

                if ( returnResult == null )
                {
                    WriteLogClass.WriteToLog(0, $"Error moving email ...", 0);
                    return false;
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
