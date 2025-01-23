using DEA.Next.Graph.GraphClientRelatedFunctions;
using Microsoft.Graph;
using WriteLog;

namespace DEA.Next.Graph.GraphHelperClasses;

internal class CreateRequestBuilderClass
{
    /// <summary>
    ///     Builds the folder request to access email inboxes.
    /// </summary>
    /// <param name="firstFolderId"></param>
    /// <param name="secondFolderId"></param>
    /// <param name="thirdFolderId"></param>
    /// <param name="emailId"></param>
    /// <returns></returns>
    public static async Task<IMailFolderRequestBuilder> CreatRequestBuilder(
        string firstFolderId,
        string secondFolderId,
        string thirdFolderId,
        string emailId)
    {
        try
        {
            var graphClient = await GraphHelper.InitializeGraphClient();

            // List of inbox names.
            List<string> folderIdList = [firstFolderId, secondFolderId, thirdFolderId];

            // Removes any empty variable.
            folderIdList.RemoveAll(string.IsNullOrEmpty);

            // Creates the request builder.
            var requestBuilder = graphClient!.Users[$"{emailId}"].MailFolders["Inbox"];

            // foreach (var folderId in folderIdList)
            // {
            //     requestBuilder = requestBuilder.ChildFolders[$"{folderId}"];
            // }

            return folderIdList.Aggregate(
                requestBuilder, (current, folderId) =>
                    current.ChildFolders[$"{folderId}"]);
        }
        catch (Exception ex)
        {
            WriteLogClass.WriteToLog(0,
                $"Exception at creating request builder: {ex.Message}",
                0);
            throw;
        }
    }
}