using AppConfigReader;
using CreatEmail;
using ErrorFolderChecker;

AppConfigReaderClass.AppSettingsRoot jsonData = AppConfigReaderClass.ReadAppDotConfig();
AppConfigReaderClass.Programsettings programSettings = jsonData.ProgramSettings;

if (programSettings.SendErrorEmail)
{
    CreateEmailClass.StartCreatingEmail(ErrorFolderCheckerClass.ErrorFolderChecker().Item2, ErrorFolderCheckerClass.ErrorFolderChecker().Item1);
}