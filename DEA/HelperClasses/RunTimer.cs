using AppConfigReader;
using AppConfigUpdater;

namespace RunTimer
{
    internal class RunTimerClass
    {
        public static bool RunTimeChecker()
        {
            string previousDate = GetJsonFileData().PreviousRunDate;
            double timeItnterval = TimeSpan.FromMinutes(GetJsonFileData().ErrorCheckInterval).TotalMinutes;
            double previousTime = TimeSpan.Parse(GetJsonFileData().PreviousRunTime).TotalMinutes;
            double timeNow = TimeSpan.Parse(DateTime.Now.ToString("t")).TotalMinutes;

            double timeDiff = timeNow - previousTime;

            if (timeDiff >= timeItnterval)
            {
                if (previousDate != DateTime.Now.ToString("d"))
                {
                    AppConfigUpdaterClass.UpdateConfigFile(DateTime.Now.ToString("t"), DateTime.Now.ToString("d"));
                }
                else
                {
                    AppConfigUpdaterClass.UpdateConfigFile(DateTime.Now.ToString("t"), string.Empty);
                }
                return true;
            }
            return false;
        }

        private static AppConfigReaderClass.Timingsettings GetJsonFileData()
        {
            AppConfigReaderClass.AppSettingsRoot jsonData = AppConfigReaderClass.ReadAppDotConfig();
            AppConfigReaderClass.Timingsettings timeSettings = jsonData.TimingSettings;

            return timeSettings;
        }
    }
}
