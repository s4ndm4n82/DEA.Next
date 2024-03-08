using System.Text.RegularExpressions;
using WriteLog;

namespace FileNameCleanerClass
{
    internal class FileNameCleaner
    {
        public static string FileNameCleanerFunction(string attachedFileName)
        {
            try
            {
                // Getting the file name and extension separately.
                string attachmentExtension = Path.GetExtension(attachedFileName).ToLower(); // Extension only from the file name. And converts it to lower case.
                string attachmentFileName = Path.GetFileNameWithoutExtension(attachedFileName); // File name only in order to clean it.

                string cleanedFileName = attachedFileName.Length > 150 ? CleanFileName(TruncateFilename(attachmentFileName, 100)) : CleanFileName(attachmentFileName);

                return Path.ChangeExtension(cleanedFileName, attachmentExtension);
            }
            catch (Exception ex)
            {
                WriteLogClass.WriteToLog(0, $"Exception at FileNameCleaner: {ex}", 0);
                return "";
            }
        }

        private static string CleanFileName(string fileName)
        {
            try
            {
                // String variables to use with the RegEx.
                string regexPattern = "[\\~#%&*{}/:;,.<>?|\\[\\]\"-]"; // RegEx will search for all these characters.
                string regexReplaceCharacter = "_"; // Above all the characters will be replaces by this character.

                // RegEx function.
                Regex regexNameCleaner = new(regexPattern, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);

                // Rebuilding the clean full filename. The second regex replace gets rid of any additional spaces if there's any.
                return Regex.Replace(regexNameCleaner.Replace(fileName, regexReplaceCharacter), @"[\s]+", "");
            }
            catch (Exception ex)
            {
                WriteLogClass.WriteToLog(0, $"Exception at file name cleaner: {ex.Message}", 0);
                return "";
            }
        }

        private static string TruncateFilename(string fileName, int maxLength)
        {
            try
            {
                return fileName.Length > maxLength ? fileName.Substring(0, maxLength) : fileName;
            }
            catch (Exception ex)
            {
                WriteLogClass.WriteToLog(0, $"Exception at truncate filename: {ex.Message}", 0);
                return "";
            }
        }
    }
}
