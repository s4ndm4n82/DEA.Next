using AppConfigReader;
using CreatEmail;
using ErrorFolderChecker;
using WriteLog;

AppConfigReaderClass.AppSettingsRoot jsonData = AppConfigReaderClass.ReadAppDotConfig();
AppConfigReaderClass.Programsettings programSettings = jsonData.ProgramSettings;

if (programSettings.SendErrorEmail)
{
    CreateEmailClass.StartCreatingEmail(ErrorFolderCheckerClass.ErrorFolderChecker().Item2, ErrorFolderCheckerClass.ErrorFolderChecker().Item1);
}
else
{
    WriteLogClass.WriteToLog(1, "Auto error folder check and email sender is disabled. Please set to true in the config file or run manually.", 1);
}