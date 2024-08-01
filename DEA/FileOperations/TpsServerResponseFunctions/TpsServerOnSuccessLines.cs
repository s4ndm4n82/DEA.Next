using DEA.Next.HelperClasses.FolderFunctions;
using UserConfigRetriverClass;
using WriteLog;
using WriteNamesToLog;

namespace DEA.Next.FileOperations.TpsServerResponseFunctions;

public static class TpsServerOnSuccessLines
{
    public static async Task<int> ServerOnSuccessLinesAsync(string localFile)
    {
        try
        {
            var fileName = Path.GetFileName(localFile);
            
            WriteLogClass.WriteToLog(1, 
                $"Uploaded file \"{fileName}\" was successfully sent to TPS ....", 1);
            
            if (await FolderCleanerLines.RemoveUploadedFilesLinesAsync(localFile)) return 1;

            return 0;
        }
        catch (Exception ex)
        {
            WriteLogClass.WriteToLog(0, $"Exception at TPS server on success lines: {ex.Message}", 0);
            return -1;
        }
    }
}