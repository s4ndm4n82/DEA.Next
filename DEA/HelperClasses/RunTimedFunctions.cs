using AppConfigReader;
using AppConfigUpdater;

namespace RunTimedFunctions
{
    internal class RunTimedFunctionsClass
    {
        public static bool CallErrorFolderChecker()
        {
            double timeItnterval = TimeSpan.FromMinutes(GetJsonFileData().ErrorCheckInterval).TotalMinutes;
            double previousTime = TimeSpan.Parse(GetJsonFileData().PreviousRunTime).TotalMinutes;
            double timeNow = TimeSpan.Parse(DateTime.Now.ToString("t")).TotalMinutes;

            double timeDiff = timeNow - previousTime;

            if (timeDiff >= timeItnterval)
            {
                AppConfigUpdaterClass.UpdateConfigFile(DateTime.Now.ToString("t"), null);
                return true;
            }
            return false;
        }

        public static string CallFileCleaner()
        {
            int dateInterval = GetJsonFileData().LogsDeleteAfter;
            DateTime previousRunDate = DateTime.Parse(GetJsonFileData().PreviousRunDate);
            DateTime dateNow = DateTime.Parse(DateTime.Now.ToString("d"));

            TimeSpan dateDiff = dateNow - previousRunDate;

            if (dateDiff.TotalDays >= dateInterval)
            {
                AppConfigUpdaterClass.UpdateConfigFile(null, dateNow.ToString());
                return "Working";
            }

            return "Not working";
        }
        private static AppConfigReaderClass.Timingsettings GetJsonFileData()
        {
            AppConfigReaderClass.AppSettingsRoot jsonData = AppConfigReaderClass.ReadAppDotConfig();
            AppConfigReaderClass.Timingsettings timeSettings = jsonData.TimingSettings;

            return timeSettings;
        }
    }
}
