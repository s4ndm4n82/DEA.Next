using WriteLog;
using DEA.Next.HelperClasses.OtherFunctions;
using FolderFunctions;

namespace DEA.Next.HelperClasses.FileFunctions;

public static class HandleErrorFilesLine
{
    public static async Task<bool> HandleErrorFilesLineAsync(string localFilePath,
        string setId,
        int clientId)
    {
        try
        {
            await Task.Run(() =>
            {
                var sourceFolderPath = Path.GetDirectoryName(localFilePath);

                if (sourceFolderPath == null) return false;
                
                var sourceLastFolderName = sourceFolderPath.Split(Path.DirectorySeparatorChar).Last();
                var sourceFileName = Path.GetFileName(localFilePath);
                
                var destinationFolderName = string.Concat("ID_", clientId, " ", "SetId_", setId);
                var destinationFolderPath = Path.Combine(FolderFunctionsClass.CheckFolders(MagicWords.error),
                    destinationFolderName,
                    sourceLastFolderName);

                if (!Directory.Exists(destinationFolderPath)) Directory.CreateDirectory(destinationFolderPath);

                return MoveFilesToErrorFolderLine(sourceFolderPath,
                    destinationFolderPath,
                    sourceFileName);
            });
        }
        catch (Exception ex)
        {
            WriteLogClass.WriteToLog(0, $"Exception at TPS server on success lines: {ex.Message}", 0);
            return false;
        }

        return true;
    }
    
    private static bool MoveFilesToErrorFolderLine(string sourceFolderPath,
        string destinationFolderPath,
        string fileName)
    {
        try
        {
            var sourceFile = Path.Combine(sourceFolderPath, fileName);
            var destinationFile = Path.Combine(destinationFolderPath, fileName);

            if (File.Exists(destinationFile))
            {
                WriteLogClass.WriteToLog(1, "File already exists ....", 1);
                return false;
            }
            
            File.Move(sourceFile, destinationFile);
        }
        catch (Exception ex)
        {
            WriteLogClass.WriteToLog(0, $"Exception at TPS server on success lines: {ex.Message}", 0);
            return false;
        }
            
        return true;
    }
}