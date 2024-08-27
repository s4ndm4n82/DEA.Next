using AppConfigReader;
using DEACleaner.MainClasses;
using WriteLog;

var jsonData = AppConfigReaderClass.ReadAppDotConfig();
var programSettings = jsonData.ProgramSettings;

if (programSettings.CleanLogs)
{
    var deleteStatus = LogFileCleanerClass.StartCleaner();

    var logEntry = deleteStatus == 1 ? "Log file deletion ended successfully ...." : "log file deletion unsuccessful ....";
    var logType = deleteStatus != 1 ? 0 : 1;

    WriteLogClass.WriteToLog(logType, logEntry, 1);
}
else
{
    WriteLogClass.WriteToLog(1, "Auto log cleaning is disabled. Please set to true in the config file or run manually.", 1);
}