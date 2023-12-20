using FolderFunctions;
using GetRecipientEmail;
using GraphHelper;
using Microsoft.Graph;
using UserConfigSetterClass;
using File = System.IO.File;
using Directory = System.IO.Directory;
using WriteLog;

namespace GraphDownloadAttachmentFilesClass
{
    internal class GraphDownloadAttachmentFiles
    {
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

        public static string CreateDownloadPath(string recipientEmail)
        {
            string attachmentsRoot = FolderFunctionsClass.CheckFolders("attachments");
            string uniqueFolder = GraphHelperClass.FolderNameRnd(10);
            return Path.Combine(attachmentsRoot, recipientEmail, uniqueFolder);
        }

        public static IEnumerable<Attachment> FilterAttachments(IEnumerable<Attachment> attachments, List<string> acceptedExtensions)
        {
            List<string> normalizedAcceptedExtensions = acceptedExtensions.Select(ext => ext.ToLower()).ToList();

            return attachments.Where(attachment => normalizedAcceptedExtensions.Contains(Path.GetExtension(attachment.Name).ToLower())
                                     && attachment.Size > 10240
                                     || (Path.GetExtension(attachment.Name).ToLower() == ".pdf" && attachment.Size < 10240));
        }

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
                    await File.WriteAllBytesAsync(FileRenamer(filePath, fileName), fileAttachment.ContentBytes);
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

        private static string FileRenamer(string filePath, string fileName)
        {
            try
            {
                string FullToDownloadFile = Path.Combine(filePath, fileName);
                string FileNameOnly = Path.GetFileNameWithoutExtension(FullToDownloadFile);
                string FileExtention = Path.GetExtension(FullToDownloadFile);
                string FilePathOnly = Path.GetDirectoryName(FullToDownloadFile);
                int Count = 1;

                while (File.Exists(FullToDownloadFile)) // If file exists starts to rename from next file.
                {
                    string NewFileName = string.Format("{0}({1})", FileNameOnly, Count++); // Makes the new file name.
                    FullToDownloadFile = Path.Combine(FilePathOnly!, NewFileName + FileExtention); // Set tthe new path as the download file path.
                }

                return FullToDownloadFile;
            }
            catch (Exception ex)
            {
                WriteLogClass.WriteToLog(0, $"Exception at file renamer: {ex.Message}", 0);
                return "";
            }
        }
    }
}
