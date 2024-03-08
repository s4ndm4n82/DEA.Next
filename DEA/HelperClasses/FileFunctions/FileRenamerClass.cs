using WriteLog;

namespace FileRenamerClass
{
    internal class FileRenamer
    {
        public static string FileRenamerFunction(string filePath, string fileName)
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
                    FullToDownloadFile = Path.Combine(FilePathOnly!, NewFileName + FileExtention); // Set the new path as the download file path.
                }

                return FullToDownloadFile;
            }
            catch (Exception ex)
            {
                WriteLogClass.WriteToLog(0, $"Exception at file re-namer: {ex.Message}", 0);
                return "";
            }
        }
    }
}
