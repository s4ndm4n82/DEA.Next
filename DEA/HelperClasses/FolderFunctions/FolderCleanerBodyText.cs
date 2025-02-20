using DEA.Next.Graph.GraphHelperClasses;
using WriteLog;

namespace DEA.Next.HelperClasses.FolderFunctions;

public static class FolderCleanerBodyText
{
    public static async Task<bool> DeleteDownloadedAttachments(List<AttachmentFile> attachments)
    {
        var loop = 0;
        try
        {
            foreach (var attachment in attachments.Where(attachment => File.Exists(attachment.FullPath)))
            {
                File.Delete(attachment.FullPath);
                loop++;
            }

            if (loop == attachments.Count)
            {
                WriteLogClass.WriteToLog(1, "Deleted all attachments ....", 2);
                var folderPath = Path.GetDirectoryName(attachments
                    .Select(f => f.FullPath)
                    .FirstOrDefault());

                if (Directory.Exists(folderPath))
                {
                    Directory.Delete(folderPath);
                    WriteLogClass.WriteToLog(1, "Deleted folder ....", 2);
                }
            }
            else
            {
                WriteLogClass.WriteToLog(0, "Error deleting attachments ....", 2);
            }

            await Task.CompletedTask;
            return true;
        }
        catch (Exception e)
        {
            WriteLogClass.WriteToLog(0, $"Error deleting attachments body: {e.Message}", 0);
            return false;
        }
    }
}