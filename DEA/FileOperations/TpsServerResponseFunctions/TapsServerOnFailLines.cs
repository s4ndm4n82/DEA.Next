using DEA.Next.HelperClasses.FileFunctions;
using WriteLog;

namespace DEA.Next.FileOperations.TpsServerResponseFunctions;

public static class TapsServerOnFailLines
{
    public static async Task<int> ServerOnFailLinesAsync(string localFile,
        string setId,
        int clientId)
    {
        try
        {
            WriteLogClass.WriteToLog(1, 
                $"{localFile} file upload failed ....", 1);
                
            await HandleErrorFilesLine.HandleErrorFilesLineAsync(localFile,
                setId,
                clientId);
        }
        catch (Exception ex)
        {
            WriteLogClass.WriteToLog(0, $"Exception at TPS server on success lines: {ex.Message}", 0);
            return -1;
        }
        return 1;
    }
}