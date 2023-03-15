using AppConfigReader;
using FolderFunctions;
using WriteLog;

namespace LogFileCleanerClass
{
    internal class LogFileCleaner
    {
        public static int StartCleaner()
        {
            AppConfigReaderClass.AppSettingsRoot jsonData = AppConfigReaderClass.ReadAppDotConfig();
            AppConfigReaderClass.Programsettings programsettings = jsonData.ProgramSettings;

            int logDeleteDays = programsettings.LogsDeleteAfter;
            //DirectoryInfo logFolderPath = new(FolderFunctionsClass.CheckFolders("logs"));
            DirectoryInfo logFolderPath = new(@"G:\Users\S4NDM4N\Development\Repos\s4ndm4n82\DEA.Next\DEA\bin\Debug\net6.0\Logs");            
            Console.WriteLine(logFolderPath);

            return LogFileDeleter(logFolderPath, logDeleteDays);
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
                    //logFile.Delete();
                    Console.WriteLine(logFile.Name);
                    loopCounter++;
                }

                string logEntry = fileCount == loopCounter ? $"Deleted {fileCount} log files from log folder ...." : $"Files not deleted .... operation stopped ....";
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
