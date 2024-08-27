using AppConfigReader;
using FindFolder;
using WriteLog;

namespace DEACleaner.MainClasses
{
    internal static class LogFileCleanerClass
    {
        public static int StartCleaner()
        {
            var jsonData = AppConfigReaderClass.ReadAppDotConfig();
            var timeSettings = jsonData.TimingSettings;
            return LogFileDeleter(FindFoldersClass.FindFolder("logs"), timeSettings.LogsDeleteAfter);
        }

        private static int LogFileDeleter(DirectoryInfo folderPath, int maxDays)
        {
            var deleteStatus = 0;
            var loopCounter = 0;
            var logFiles = folderPath
                .EnumerateFiles("*.txt", SearchOption.TopDirectoryOnly)
                .Where(fn => DateTime.UtcNow - fn.CreationTimeUtc > TimeSpan.FromDays(maxDays));
            var fileInfos = logFiles as FileInfo[] ?? logFiles.ToArray();
            var fileCount = fileInfos.Length;

            try
            {
                foreach (var logFile in fileInfos)
                {
                    logFile.Delete();
                    loopCounter++;
                }

                var logEntry = fileCount == loopCounter ? $"Deleted {fileCount} log files from log folder ...."
                    : $"Files not deleted .... operation stopped ....";
                var logType = fileCount == loopCounter ? 1 : 0;
                
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