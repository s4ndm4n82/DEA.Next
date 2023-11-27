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
        public static string GetFileNames(string folderPath)
        {
            string returnFileNames = "";
            try
            {
                DirectoryInfo dirInfo = new(folderPath);
                IEnumerable<FileInfo> fileNames = dirInfo.EnumerateFiles("*.*", SearchOption.TopDirectoryOnly);

                return returnFileNames = string.Join(", ", fileNames.Select(fn => fn.Name));
            }
            catch (Exception ex)
            {
                WriteLogClass.WriteToLog(0, $"Exception at get file names: {ex.Message}", 0);
                return returnFileNames;
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
