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
                DirectoryInfo dir = Directory.GetParent(folderPath)!;

                string regExString = @"^(?:.+\\ftpfiles\\.*)$";
                Regex regExSearch = new(regExString, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
                Match regExMatch = regExSearch.Match(folderPath);

                // If regex match path will be taken using new(Path.GetDirectoryName(folderPath)!).
                // If not path will be directly assigned.
                //dir = regExMatch.Success ? new(Path.GetDirectoryName(folderPath)!) : new(folderPath);
                Console.WriteLine(dir);
                IEnumerable<FileInfo> fileNames = dir.EnumerateFiles("*.*", SearchOption.TopDirectoryOnly);

                return returnFileNames = string.Join(", ", fileNames.Select(fn => fn.Name));
            }
            catch (Exception ex)
            {
                WriteLogClass.WriteToLog(0, $"Exception at get file names: {ex.Message}", 0);
                return returnFileNames;
            }
            
        }
    }
}
