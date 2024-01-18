using Microsoft.Graph;
using System.Diagnostics.CodeAnalysis;
using WriteLog;

namespace DEA.Next.Graph.GraphHelperClasses
{
    internal class CreateRequestBuilderClass
    {
        /// <summary>
        /// Builds the folder request to access email inboxes.
        /// </summary>
        /// <param name="graphClient"></param>
        /// <param name="firstFolderId"></param>
        /// <param name="secondFolderId"></param>
        /// <param name="thirdFolderId"></param>
        /// <param name="emailId"></param>
        /// <returns></returns>
        public static async Task<IMailFolderRequestBuilder> CreatRequestBuilder([NotNull] GraphServiceClient graphClient,
                                                                         string firstFolderId,
                                                                         string secondFolderId,
                                                                         string thirdFolderId,
                                                                         string emailId)
        {
            try
            {
                // List of inbox names.
                List<string> folderIdList = new() { firstFolderId, secondFolderId, thirdFolderId };
                
                // Removes any empty variable.
                folderIdList.RemoveAll(string.IsNullOrEmpty);

                // Creates the request builder.
                IMailFolderRequestBuilder requestBuilder = graphClient!.Users[$"{emailId}"].MailFolders["Inbox"];

                foreach (string folderId in folderIdList)
                {
                    requestBuilder = requestBuilder.ChildFolders[$"{folderId}"];
                }

                return requestBuilder;
            }
            catch (Exception ex)
            {
                WriteLogClass.WriteToLog(0, $"Exception at creating request builder: {ex.Message}", 0);
                return null;
            }
        }
    }
}
