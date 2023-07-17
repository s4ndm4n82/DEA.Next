using AppConfigReader;
using AppConfigUpdater;

namespace RunTimer
{
    internal class RunTimerClass
    {
        public static (string,double) RunTimeChecker()
        {   
            double timeItnterval = TimeSpan.FromMinutes(GetJsonFileData().ErrorCheckInterval).TotalMinutes;
            double previousTime = TimeSpan.Parse(GetJsonFileData().PreviousRunTime).TotalMinutes;
            double timeNow = TimeSpan.Parse(DateTime.Now.ToString("t")).TotalMinutes;

            double timeDiff = timeNow - previousTime;

            if (timeDiff >= timeItnterval)
            {
                AppConfigUpdaterClass.UpdateConfigFile(timeNow);
                return (temp,timeDiff);
            }

             return ("Out of if", timeDiff);
        }

        private static AppConfigReaderClass.Timingsettings GetJsonFileData()
        {
            AppConfigReaderClass.AppSettingsRoot jsonData = AppConfigReaderClass.ReadAppDotConfig();
            AppConfigReaderClass.Timingsettings timeSettings = jsonData.TimingSettings;

            return timeSettings;
        }
    }
}
