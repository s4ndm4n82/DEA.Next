using AppConfigReader;
using Newtonsoft.Json.Linq;

namespace AppConfigUpdater
{
    internal class AppConfigUpdaterClass
    {
        public static string UpdateConfigFile(double runTime)
        {
            AppConfigReaderClass.AppSettingsRoot jsonData = AppConfigReaderClass.ReadAppDotConfig();
            AppConfigReaderClass.Timingsettings timeSettings = jsonData.TimingSettings;

            JToken jToken = timeSettings.
        }
    }
}
