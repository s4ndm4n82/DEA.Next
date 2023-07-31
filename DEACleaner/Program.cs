using AppConfigReader;
using LogFileCleaner;
using WriteLog;

AppConfigReaderClass.AppSettingsRoot jsonData = AppConfigReaderClass.ReadAppDotConfig();
AppConfigReaderClass.Programsettings programSettings = jsonData.ProgramSettings;

if (programSettings.CleanLogs)
{
    int deleteStatus = LogFileCleanerClass.StartCleaner();

    string logEntry = deleteStatus == 1 ? "Log file deletion ended successfully ...." : "log file deletion unsuccessfull ....";
    int logType = deleteStatus != 1 ? 0 : 1;

    WriteLogClass.WriteToLog(logType, logEntry, 1);
}
else
{
    WriteLogClass.WriteToLog(1, "Auto log cleaning is disabled. Please set to true in the config file or run manually.", 1);
}