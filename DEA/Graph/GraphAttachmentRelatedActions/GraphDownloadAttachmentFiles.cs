using DEA.Next.Entities;
using DEA.Next.Graph.GraphClientRelatedFunctions;
using DEA.Next.HelperClasses.ConfigFileFunctions;
using DEA.Next.HelperClasses.OtherFunctions;
using FileRenamerClass;
using FolderFunctions;
using GetRecipientEmail;
using Microsoft.Graph;
using WriteLog;
using File = System.IO.File;
using Directory = System.IO.Directory;

namespace DEA.Next.Graph.GraphAttachmentRelatedActions;

internal class GraphDownloadAttachmentFiles
{
    /// <summary>
    ///     Extract the recipient email from the message.
    /// </summary>
    /// <param name="requestBuilder"></param>
    /// <param name="messageId"></param>
    /// <param name="customerId"></param>
    /// <returns></returns>
    public static async Task<string> DetermineRecipientEmail(IMailFolderRequestBuilder requestBuilder,
        string messageId,
        Guid customerId)
    {
        var clientDetails = await UserConfigRetriever.RetrieveUserConfigById(customerId);

        if (clientDetails.FileDeliveryMethod.Equals(MagicWords.Email, StringComparison.CurrentCultureIgnoreCase))
            return await GetRecipientEmailClass.GetRecipientEmail(requestBuilder, messageId);
        return string.Empty;
    }

    /// <summary>
    ///     Creates the local download folder path.
    /// </summary>
    /// <param name="recipientEmail"></param>
    /// <returns></returns>
    public static string CreateDownloadPath(string recipientEmail)
    {
        var attachmentsRoot = FolderFunctionsClass.CheckFolders(MagicWords.Attachments);
        var uniqueFolder = GraphHelper.FolderNameRnd(10);
        return Path.Combine(attachmentsRoot, recipientEmail, uniqueFolder);
    }

    /// <summary>
    ///     Filter the attachments according to the accepted extensions and files size.
    /// </summary>
    /// <param name="attachments"></param>
    /// <param name="acceptedExtensions"></param>
    /// <returns></returns>
    public static IEnumerable<Attachment> FilterAttachments(IEnumerable<Attachment> attachments,
        IEnumerable<DocumentDetails> acceptedExtensions)
    {
        var lowerCaseExtensions = acceptedExtensions
            .Select(ext => ext.Extension.ToLower())
            .ToList();

        return attachments
            .Where(attachment =>
            {
                var extension = Path.GetExtension(attachment.Name).ToLower();
                return (lowerCaseExtensions.Contains(extension) && attachment.Size > 10240)
                       || (extension.Equals(string.Concat(".", MagicWords.Pdf),
                               StringComparison.CurrentCultureIgnoreCase)
                           && attachment.Size < 10240);
            });
    }

    /// <summary>
    ///     Fetch the attachment data to download.
    /// </summary>
    /// <param name="requestBuilder"></param>
    /// <param name="messageId"></param>
    /// <param name="attachmentId"></param>
    /// <returns></returns>
    public static async Task<Attachment> FetchAttachmentData(IMailFolderRequestBuilder requestBuilder,
        string messageId,
        string attachmentId)
    {
        // Fetch the attachment data
        var attachmentData = await requestBuilder
            .Messages[messageId]
            .Attachments[attachmentId]
            .Request()
            .GetAsync();

        return attachmentData;
    }

    /// <summary>
    ///     Save the fetched attachment data as a file in to the local download folder.
    /// </summary>
    /// <param name="attachmentData"></param>
    /// <param name="filePath"></param>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public static async Task<bool> SaveAttachmentToFile(Attachment attachmentData,
        string filePath,
        string fileName)
    {
        if (!Directory.Exists(filePath))
            try
            {
                Directory.CreateDirectory(filePath);
            }
            catch (Exception ex)
            {
                WriteLogClass.WriteToLog(0,
                    $"Exception when creating directory {filePath}: {ex.Message}",
                    0);
                return false;
            }

        if (attachmentData is not FileAttachment fileAttachment) return false;

        try
        {
            await File.WriteAllBytesAsync(FileRenamer.FileRenamerFunction(filePath, fileName),
                fileAttachment.ContentBytes);

            return true;
        }
        catch (Exception ex)
        {
            // Handle exceptions, e.g., log error and return false
            WriteLogClass.WriteToLog(0,
                $"Exception when saving file {filePath}: {ex.Message}",
                0);
            return false;
        }
    }
}