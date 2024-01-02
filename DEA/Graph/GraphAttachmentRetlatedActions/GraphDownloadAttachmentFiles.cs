using FolderFunctions;
using GetRecipientEmail;
using GraphHelper;
using Microsoft.Graph;
using UserConfigSetterClass;
using File = System.IO.File;
using Directory = System.IO.Directory;
using WriteLog;
using FileRenamerClass;

namespace GraphDownloadAttachmentFilesClass
{
    internal class GraphDownloadAttachmentFiles
    {
        /// <summary>
        /// Extract the recipient email from the message.
        /// </summary>
        /// <param name="graphClient"></param>
        /// <param name="clientDetails"></param>
        /// <param name="mainFolderId"></param>
        /// <param name="subFolderId1"></param>
        /// <param name="subFolderId2"></param>
        /// <param name="messageId"></param>
        /// <param name="inEmail"></param>
        /// <returns></returns>
        public static string DetermineRecipientEmail(GraphServiceClient graphClient,
                                                     UserConfigSetter.Customerdetail clientDetails,
                                                     string mainFolderId,
                                                     string subFolderId1,
                                                     string subFolderId2,
                                                     string messageId,
                                                     string inEmail)
        {
            if (clientDetails.FileDeliveryMethod.ToLower() == "email")
            {
                return GetRecipientEmailClass.GetRecipientEmail(graphClient,
                                                                mainFolderId,
                                                                subFolderId1,
                                                                subFolderId2,
                                                                messageId,
                                                                inEmail);
            }
            return string.Empty;
        }

        /// <summary>
        /// Creates the local download folder path.
        /// </summary>
        /// <param name="recipientEmail"></param>
        /// <returns></returns>
        public static string CreateDownloadPath(string recipientEmail)
        {
            string attachmentsRoot = FolderFunctionsClass.CheckFolders("attachments");
            string uniqueFolder = GraphHelperClass.FolderNameRnd(10);
            return Path.Combine(attachmentsRoot, recipientEmail, uniqueFolder);
        }

        /// <summary>
        /// Filter the attachments accordinmg to the accepted extensions and files size.
        /// </summary>
        /// <param name="attachments"></param>
        /// <param name="acceptedExtensions"></param>
        /// <returns></returns>
        public static IEnumerable<Attachment> FilterAttachments(IEnumerable<Attachment> attachments, List<string> acceptedExtensions)
        {
            List<string> normalizedAcceptedExtensions = acceptedExtensions.Select(ext => ext.ToLower()).ToList();

            return attachments.Where(attachment => normalizedAcceptedExtensions.Contains(Path.GetExtension(attachment.Name).ToLower())
                                     && attachment.Size > 10240
                                     || (Path.GetExtension(attachment.Name).ToLower() == ".pdf" && attachment.Size < 10240));
        }

        /// <summary>
        /// Fetch the attachment data to download.
        /// </summary>
        /// <param name="graphClient"></param>
        /// <param name="inEmail"></param>
        /// <param name="mainFolderId"></param>
        /// <param name="subFolderId1"></param>
        /// <param name="subFolderId2"></param>
        /// <param name="messageId"></param>
        /// <param name="attachmentId"></param>
        /// <returns></returns>
        public static async Task<Attachment> FetchAttachmentData(GraphServiceClient graphClient,
                                                                 string inEmail,
                                                                 string mainFolderId,
                                                                 string subFolderId1,
                                                                 string subFolderId2,
                                                                 string messageId,
                                                                 string attachmentId)
        {
            List<string> folderIds = new() { mainFolderId, subFolderId1, subFolderId2 };
            folderIds.RemoveAll(string.IsNullOrEmpty);

            if (!folderIds.Any())
            {
                return null;
            }

            // Build the request path dynamically based on the provided folder IDs
            IMailFolderRequestBuilder requestBuilder = graphClient.Users[inEmail].MailFolders["Inbox"];

            foreach (var folderId in folderIds)
            {
                requestBuilder = requestBuilder.ChildFolders[folderId];
            }

            // Fetch the attachment data
            var attachmentData = await requestBuilder.Messages[messageId].Attachments[attachmentId].Request().GetAsync();
            return attachmentData;
        }

        /// <summary>
        /// Save the fetched attachment data as a file in to the local download folder.
        /// </summary>
        /// <param name="attachmentData"></param>
        /// <param name="filePath"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static async Task<bool> SaveAttachmentToFile(Attachment attachmentData, string filePath, string fileName)
        {
            if (!Directory.Exists(filePath))
            {
                try
                {
                    Directory.CreateDirectory(filePath);
                }
                catch (Exception ex)
                {
                    WriteLogClass.WriteToLog(0, $"Exception when creating directory {filePath}: {ex.Message}", 0);
                    return false;
                }
            }

            if (attachmentData is FileAttachment fileAttachment)
            {
                try
                {
                    await File.WriteAllBytesAsync(FileRenamer.FileRenamerFunction(filePath, fileName), fileAttachment.ContentBytes);
                    return true;
                }
                catch (Exception ex)
                {
                    // Handle exceptions, e.g., log error and return false
                    WriteLogClass.WriteToLog(0, $"Exception when saving file {filePath}: {ex.Message}", 0);
                    return false;
                }
            }
            return false;
        }        
    }
}
