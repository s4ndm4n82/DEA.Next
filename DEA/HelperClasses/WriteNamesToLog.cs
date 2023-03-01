using Microsoft.Graph;
using System.Text.RegularExpressions;
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
                DirectoryInfo dir = new(Path.GetDirectoryName(folderPath)!);
                IEnumerable<string> fileNames = dir.GetFiles("*.*", SearchOption.TopDirectoryOnly).Select(fn => fn.Name);

                return returnFileNames = string.Join(", ", fileNames);
            }
            catch (Exception ex)
            {
                WriteLogClass.WriteToLog(0, $"Exception at get file names: {ex.Message}", 0);
                return returnFileNames;
            }
            
        }
    }
}
