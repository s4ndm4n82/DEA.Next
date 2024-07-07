using UserConfigRetriverClass;
using WriteLog;

namespace DEA.Next.FileOperations.TpsJsonStringCreatorFunctions
{
    internal class MakeJsonRequestLinesFunction
    {
        public static async Task<int> MakeJsonRequestLines(string mainFileName,
            string setId,
            int clientId,
            string[] localFileList)
        {
            try
            {
                var jsonData = await UserConfigRetriver.RetriveUserConfigById(clientId);

                var fieldsList = MakeJsonRequestHelperClass.ReturnIdFieldListLines(mainFileName,
                    setId,
                    clientId);
                
                foreach (var localFile in localFileList)
                {
                    var fileList = MakeJsonRequestHelperClass.ReturnFilesListLines(localFile);
                }
            }
            catch (Exception ex)
            {
                WriteLogClass.WriteToLog(0, $"Exception at Json serialization: {ex.Message}", 0);
                return -1;
            }
            return -1;
        }
    }
}
