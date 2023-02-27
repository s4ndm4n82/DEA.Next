using DEA;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using FolderFunctions;

namespace WriteLog
{
    internal class WriteLogClass
    {
        public static void WriteToLog(int Level, string LogEntry, int LogType)
        {   
            string LogFileName = "DEA_Logfile_" + DateTime.Now.ToString("dd_MM_yyyy") + ".txt";
            string LogFile = Path.Combine(FolderFunctionsClass.CheckFolders("logs"), LogFileName);
            string entryType;

            switch (LogType)
            {
                case 1 :
                    entryType = "[ PRG ]";
                    break;

                case 2 :
                    entryType = "[ EML ]";
                    break;

                case 3 :
                    entryType = "[ FTP ]";
                    break;

                case 4 :
                    entryType = "[ RST ]";
                    break;

                case 5 :
                    entryType = "[ GRP ]";
                    break;

                default :
                    entryType = "[ ERR ]";
                    break;
            }

            LoggingLevelSwitch LogControlSwitch = new();

            LogEventLevel LogLevel;
            
            switch (Level)
            {
                case 1 :
                    LogLevel = LogEventLevel.Error;                    
                    break;

                case 2 :
                    LogLevel = LogEventLevel.Warning;
                    break;

                case 3 :
                    LogLevel = LogEventLevel.Information;
                    break;

                case 4 :
                     LogLevel = LogEventLevel.Debug;
                    break;

                case 5 :
                     LogLevel = LogEventLevel.Verbose;
                    break;

                default :
                    LogLevel = LogEventLevel.Fatal;
                    break;
            }

            string textLine = string.Concat($"{entryType}  ", LogEntry);

            LogControlSwitch.MinimumLevel = LogLevel;

            Log.Logger = new LoggerConfiguration()
                        .MinimumLevel.ControlledBy(LogControlSwitch)
                        .WriteTo.File(LogFile)
                        .WriteTo.Console()                        
                        .CreateLogger();

            WriteLog(Level, textLine);
            
            Log.CloseAndFlush();
        }

        private static void WriteLog(int LogLevel, string LogEntryString)
        {
            if (LogLevel == 1)
            {
                Log.Error(LogEntryString);
            }
            else if (LogLevel == 2)
            {
                Log.Warning(LogEntryString);
            }
            else if (LogLevel == 3)
            {
                Log.Information(LogEntryString);
            }
            else if (LogLevel == 4)
            {
                Log.Debug(LogEntryString);
            }
            else if (LogLevel == 5)
            {
                Log.Verbose(LogEntryString);
            }
            else
            {
                Log.Error(LogEntryString);
            }
        }
    }
}
