using AppConfigReader;
using FindFolder;
using WriteLog;

namespace LogFileCleaner
{
    internal class LogFileCleanerClass
    {
        public static int StartCleaner()
        {
            AppConfigReaderClass.AppSettingsRoot jsonData = AppConfigReaderClass.ReadAppDotConfig();
            AppConfigReaderClass.Timingsettings timeSettings = jsonData.TimingSettings;
            
            return LogFileDeleter(FindFoldersClass.FindFolder("logs"), timeSettings.LogsDeleteAfter);
        }

        private static int LogFileDeleter(DirectoryInfo folderPath, int maxDays)
        {
            int deleteStatus = 0;
            IEnumerable<FileInfo> logFiles = folderPath.EnumerateFiles("*.txt", SearchOption.TopDirectoryOnly).Where(fn => DateTime.UtcNow - fn.CreationTimeUtc > TimeSpan.FromDays(maxDays));
            int fileCount = logFiles.Count();
            int loopCounter = 0;

            try
            {
                foreach (FileInfo logFile in logFiles)
                {
                    logFile.Delete();
                    loopCounter++;
                }

                string logEntry = fileCount == loopCounter ? $"Deleted {fileCount} log files from log folder ...." : $"FilesList not deleted .... operation stopped ....";
                int logType = fileCount == loopCounter ? 1 : 0;

                WriteLogClass.WriteToLog(logType, logEntry, 1);
                return deleteStatus = 1;
            }
            catch (Exception ex)
            {
                WriteLogClass.WriteToLog(0, $"Exception at log file deleter: {ex.Message}", 0);
                return deleteStatus;
            }        
        }
    }
}
