using UserConfigRetriverClass;
using WriteLog;
using WriteNamesToLog;

namespace DEA.Next.FileOperations.TpsServerResponseFunctions;

public class TpsServerOnSuccessLines
{
    public static async Task<int> ServerOnSuccessLinesAsync(string folderPath,
        string[] localFileList)
    {
        try
        {
            WriteLogClass.WriteToLog(1, $"Uploaded files to TPS: {WriteNamesToLogClass.GetFileNames(localFileList)}",
                1);
        }
        catch (Exception ex)
        {
            WriteLogClass.WriteToLog(0, $"Exception at TPS server on success lines: {ex.Message}", 0);
            return -1;
        }

        return 1;
    }
}