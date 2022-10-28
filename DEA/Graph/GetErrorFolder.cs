using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetErrorFolder
{
    internal class GetErrorFolderClass
    {
        public static async Task<string> GetErrorFolderId([NotNull] GraphServiceClient graphClient, string clientEmail, string mainFolderId, string subFolderId1, string subFolderId2)
        {
            string errorFolerId = string.Empty;

            if (!string.IsNullOrWhiteSpace(mainFolderId) && string.IsNullOrWhiteSpace(subFolderId1) && string.IsNullOrWhiteSpace(subFolderId2))
            {
                var errorFolderDetails = await graphClient.Users[$"{clientEmail}"].MailFolders["Inbox"]
                                         .ChildFolders[$"{mainFolderId}"]
                                         .ChildFolders
                                         .Request()
                                         .GetAsync();

                errorFolerId = errorFolderDetails.FirstOrDefault(x => x.DisplayName.ToLower() == "error")!.Id;
            }
        }
    }
}
