using Microsoft.Graph;
using WriteLog;

namespace GraphMoveEmailsrClass
{
    internal class GraphMoveEmailsFolder
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
        public static async Task<bool> MoveEmailsToAnotherFolder(IMailFolderRequestBuilder requestBuilder,
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
