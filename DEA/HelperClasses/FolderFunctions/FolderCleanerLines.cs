using WriteLog;

namespace DEA.Next.HelperClasses.FolderFunctions;

public static class FolderCleanerLines
{
    public static async Task<bool> RemoveUploadedFilesLinesAsync(string localeFile)
    {
        try
        {
            await Task.Run(() =>
            {
                if (File.Exists(localeFile)) File.Delete(localeFile); // Remove the uploaded files.
            });
        }
        catch (Exception ex)
        {
            WriteLogClass.WriteToLog(0, $"Exception at Folder cleaner lines: {ex.Message}", 0);
            return false;
        }

        return true;
    }
}