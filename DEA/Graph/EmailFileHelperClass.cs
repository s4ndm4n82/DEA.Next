using System.Text.RegularExpressions;
using WriteLog;

namespace EmailFileHelper
{
    internal class EmailFileHelperClass
    {
        public static string FileNameCleaner(string attachedFileName)
        {
            try
            {
                // Getting the file name and extention seperatly.
                string attachmentExtension = Path.GetExtension(attachedFileName).ToLower(); // Extension only from the file name. And converts it to lower case.
                string attachmentFileName = Path.GetFileNameWithoutExtension(attachedFileName); // File name only in order to clean it.

                string cleanedFileName = attachedFileName.Length > 50 ? CleanFileName(TruncateFilename(attachmentFileName, 10)) : CleanFileName(attachmentFileName);

                return Path.ChangeExtension(cleanedFileName, attachmentExtension);
            }
            catch (Exception ex)
            {
                WriteLogClass.WriteToLog(0, $"Exception at FileNameCleaner: {ex}", 0);
                return "";
            }
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
            try
            {
                string FullToDownloadFile = Path.Combine(filePath, fileName);
                string FileNameOnly = Path.GetFileNameWithoutExtension(FullToDownloadFile);
                string FileExtention = Path.GetExtension(FullToDownloadFile);
                string FilePathOnly = Path.GetDirectoryName(FullToDownloadFile);
                int Count = 1;

                while (File.Exists(FullToDownloadFile)) // If file exists starts to rename from next file.
                {
                    string NewFileName = string.Format("{0}({1})", FileNameOnly, Count++); // Makes the new file name.
                    FullToDownloadFile = Path.Combine(FilePathOnly!, NewFileName + FileExtention); // Set tthe new path as the download file path.
                }

                return FullToDownloadFile;
            }
            catch (Exception ex)
            {
                WriteLogClass.WriteToLog(0, $"Exception at file renamer: {ex.Message}", 0);
                return "";
            }
        }

        private static string CleanFileName(string fileName)
        {
            try
            {
                // String variables to use with the RegEx.
                string regexPattern = "[\\~#%&*{}/:;,.<>?|\\[\\]\"-]"; // RegEx will search for all these characters.
                string regexReplaceCharacter = "_"; // Above all the characters will be replaces by this sharacter.

                // RegEx function.
                Regex regexNameCleaner = new(regexPattern, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);

                // Rebuilding the clean full filename. The second regex replace gets reid of any aditional spaces if there's any.
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
