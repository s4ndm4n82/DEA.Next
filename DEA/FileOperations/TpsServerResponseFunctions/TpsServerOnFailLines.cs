using DEA.Next.HelperClasses.FileFunctions;
using WriteLog;

namespace DEA.Next.FileOperations.TpsServerResponseFunctions;

public static class TpsServerOnFailLines
{
    public static async Task<int> ServerOnFailLinesAsync(string mainFileName,
        string localFile,
        string setId,
        bool lastItem,
        int clientId)
    {
        try
        {
            WriteLogClass.WriteToLog(1, 
                $"{Path.GetFileName(localFile)} file upload failed ....", 1);

            var filePath = Path.GetDirectoryName(localFile);
            await HandleErrorFilesLine.HandleErrorFilesLineAsync(localFile,
                setId,
                clientId);
            
            if (lastItem)
            {
                if (filePath == null) return -1;
                var leftOverFileList = Directory.GetFiles(filePath);
                var mainFile = leftOverFileList
                    .FirstOrDefault(fileName => Path.GetFileNameWithoutExtension(fileName) == 
                                                Path.GetFileNameWithoutExtension(mainFileName));
                if (mainFile == null) return -1;
                await HandleErrorFilesLine.HandleErrorFilesLineAsync(mainFile,
                    setId,
                    clientId);
            }
        }
        catch (Exception ex)
        {
            WriteLogClass.WriteToLog(0, $"Exception at TPS server on success lines: {ex.Message}", 0);
            return -1;
        }
        return 1;
    }
}