using System.Text.RegularExpressions;
using WriteLog;

namespace EmailFileHelper
{
    internal class EmailFileHelperClass
    {
        public static string FileNameCleaner(string attachedFileName)
        {

            // Getting the file name and extention seperatly.
            string attachmentExtension = Path.GetExtension(attachedFileName).ToLower(); // Extension only from the file name. And converts it to lower case.
            string attachmentFileName = Path.GetFileNameWithoutExtension(attachedFileName); // File name only in order to clean it.

            // String variables to use with the RegEx.
            string regexPattern = "[\\~#%&*{}/:;,.<>?|\\[\\]\"-]"; // RegEx will search for all these characters.
            string regexReplaceCharacter = "_"; // Above all the characters will be replaces by this sharacter.

            // RegEx function.
            Regex regexNameCleaner = new(regexPattern, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);

            // Rebuilding the clean full filename. The second regex replace gets reid of any aditional spaces if there's any.
            return Path.ChangeExtension(Regex.Replace(regexNameCleaner.Replace(attachmentFileName, regexReplaceCharacter), @"[\s]+", ""), attachmentExtension);            
        }
        public static bool FileDownloader(string DownloadFolderPath, string DownloadFileName, byte[] DownloadFileData)
        {
            if (!Directory.Exists(DownloadFolderPath))
            {
                try
                {
                    Directory.CreateDirectory(DownloadFolderPath);
                }
                catch (Exception ex)
                {
                    WriteLogClass.WriteToLog(0, $"Exception at download folder creation: {ex.Message}", 0);
                }
            }

            try
            {

                // Writes the file on to the local hard disk.
                // If same file name exists it will be renamed.
                File.WriteAllBytes(FileRenamer(DownloadFolderPath, DownloadFileName), DownloadFileData);
                return true;
            }
            catch (Exception ex)
            {
                WriteLogClass.WriteToLog(0, $"Exception at download path: {ex.Message}", 0);
                return false;
            }
        }

        private static string FileRenamer(string filePath, string fileName)
        {
            var FullToDownloadFile = Path.Combine(filePath, fileName);
            var FileNameOnly = Path.GetFileNameWithoutExtension(FullToDownloadFile);
            var FileExtention = Path.GetExtension(FullToDownloadFile);
            var FilePathOnly = Path.GetDirectoryName(FullToDownloadFile);
            int Count = 1;

            while (File.Exists(FullToDownloadFile)) // If file exists starts to rename from next file.
            {
                var NewFileName = string.Format("{0}({1})", FileNameOnly, Count++); // Makes the new file name.
                FullToDownloadFile = Path.Combine(FilePathOnly!, NewFileName + FileExtention); // Set tthe new path as the download file path.
            }

            return FullToDownloadFile;
        }
    }
}
