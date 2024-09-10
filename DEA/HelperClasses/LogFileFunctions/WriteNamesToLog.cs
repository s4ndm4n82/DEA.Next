using WriteLog;

namespace WriteNamesToLog
{
    internal class WriteNamesToLogClass
    {
        /// <summary>
        /// Used to enumerate the filenames one after the other in the log file.
        /// </summary>
        /// <param name="folderPath"></param>
        /// <returns></returns>
        public static string GetFileNames(string[] jsonFileList)
        {
            try
            {
                if (jsonFileList == null && jsonFileList.Length !> 0)
                {
                    WriteLogClass.WriteToLog(0, "File list is empty", 0);
                    return "";
                }
                
                return string.Join(", ", jsonFileList);
            }
            catch (Exception ex)
            {
                WriteLogClass.WriteToLog(0, $"Exception at get file names: {ex.Message}", 0);
                return "";
            }
            
        }
        public static string WriteMissedFilenames(IEnumerable<string> missedFileName)
        {
            string returnMissedFileNames = "";
            try
            {
                return returnMissedFileNames = string.Join(", ", missedFileName);
            }
            catch (Exception ex)
            {
                WriteLogClass.WriteToLog(0, $"Exception at write missed file names: {ex.Message}", 0);
                return returnMissedFileNames;
            }
        }
    }
}
