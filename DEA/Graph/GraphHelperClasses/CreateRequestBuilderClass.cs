﻿using Microsoft.Graph;
using System.Diagnostics.CodeAnalysis;
using WriteLog;

namespace DEA.Next.Graph.GraphHelperClasses
{
    internal class CreateRequestBuilderClass
    {
        public static async Task<IMailFolderRequestBuilder> CreatRequestBuilder([NotNull] GraphServiceClient graphClient,
                                                                         string firstFolderId,
                                                                         string secondFolderId,
                                                                         string thirdFolderId,
                                                                         string emailId)
        {
            try
            {
                List<string> folderIdList = new() { firstFolderId, secondFolderId, thirdFolderId };
                folderIdList.RemoveAll(string.IsNullOrEmpty); // Removes any empty variable.

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
