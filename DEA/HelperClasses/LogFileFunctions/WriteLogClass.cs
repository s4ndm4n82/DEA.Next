using Serilog;
using Serilog.Core;
using Serilog.Events;
using FolderFunctions;
using DEA.Next.HelperClasses.OtherFunctions;

namespace WriteLog
{
    public class WriteLogClass
    {
        /// <summary>
        /// Class that handles the writing of the log file.
        /// </summary>
        /// <param name="loggingLevel"></param>
        /// <param name="LogEntry"></param>
        /// <param name="logType"></param>
        public static void WriteToLog(int loggingLevel, string LogEntry, int logType)
        {
            if (loggingLevel >= 0 && logType >= 0)
            {
                // Log file name.
                string LogFileName = "DEA_Logfile_" + DateTime.Now.ToString("dd_MM_yyyy") + ".txt";
                // Path to the log file.
                string LogFile = Path.Combine(FolderFunctionsClass.CheckFolders(MagicWords.logs), LogFileName);
                // Log file entry type or message entry location type array.
                string[] entryTypes = { "[ ERR ]", "[ PRG ]", "[ EML ]", "[ FTP ]", "[ RST ]", "[ GRP ]" };
                // Log file entry type or message entry location type.
                string entryType = entryTypes[logType];

                LoggingLevelSwitch LogControlSwitch = new(); // Creating new log options.
                // Log levels array.
                LogEventLevel[] logLevels = { LogEventLevel.Error, LogEventLevel.Information, LogEventLevel.Warning, LogEventLevel.Debug, LogEventLevel.Verbose, LogEventLevel.Fatal };
                // Retriving log event from the array above.
                LogEventLevel logLevel = logLevels[loggingLevel];
                // Creating the log entry text line.
                string textLine = string.Concat($"{entryType}  ", LogEntry);
                // Log options.
                LogControlSwitch.MinimumLevel = logLevel;
                Log.Logger = new LoggerConfiguration()
                            .MinimumLevel.ControlledBy(LogControlSwitch)
                            .WriteTo.File(LogFile)
                            .WriteTo.Console()
                            .CreateLogger();
                // Writing the log to the file.
                WriteLog(loggingLevel, textLine);
                // Closing the file and flushing the memory.
                Log.CloseAndFlush();
            }
        }

        /// <summary>
        /// Writs the log file with the correct message type.
        /// </summary>
        /// <param name="logLevel"></param>
        /// <param name="logEntryString"></param>
        private static void WriteLog(int logLevel, string logEntryString)
        {
            
            switch (logLevel)
            {
                case 0:
                    Log.Error(logEntryString);
                    break;
                case 1:
                    Log.Information(logEntryString); 
                    break;
                case 2:
                    Log.Warning(logEntryString);
                    break;
                case 3:
                    Log.Debug(logEntryString);
                    break;
                case 4:
                    Log.Verbose(logEntryString);
                    break;
                default:
                    Log.Fatal(logEntryString);
                    break;
            }
        }
    }
}
