using DEA.Next.HelperClasses.FolderFunctions;
using UserConfigRetriverClass;
using WriteLog;
using WriteNamesToLog;

namespace DEA.Next.FileOperations.TpsServerResponseFunctions;

public static class TpsServerOnSuccessLines
{
    public static async Task<int> ServerOnSuccessLinesAsync(string mainFileName, string localFile, bool lastItem)
    {
        try
        {
            var filePath = Path.GetDirectoryName(localFile);
            
            WriteLogClass.WriteToLog(1, 
                $"Uploaded file \"{Path.GetFileName(localFile)}\" was successfully sent to TPS ....", 1);
            
            await FolderCleanerLines.RemoveUploadedFilesLinesAsync(localFile);

            if (!lastItem) return 1;
            if (filePath == null) return -1;
            var leftOverFileList = Directory.GetFiles(filePath);
            var mainFile = leftOverFileList
                .FirstOrDefault(fileName => Path.GetFileNameWithoutExtension(fileName) == 
                                            Path.GetFileNameWithoutExtension(mainFileName));
            if (mainFile == null) return -1;
            if (await FolderCleanerLines.RemoveUploadedFilesLinesAsync(mainFile)) return 1;

            return 0;
        }
        catch (Exception ex)
        {
            WriteLogClass.WriteToLog(0, $"Exception at TPS server on success lines: {ex.Message}", 0);
            return -1;
        }
    }
}