using AppConfigReader;
using Newtonsoft.Json;
using WriteLog;

namespace AppConfigUpdater
{
    internal class AppConfigUpdaterClass
    {
        public static bool UpdateConfigFile(string lastRunTime, string lastRunDate)
        {
            try
            {
                AppConfigReaderClass.AppSettingsRoot jsonData = AppConfigReaderClass.ReadAppDotConfig();
                AppConfigReaderClass.Timingsettings timeSettings = jsonData.TimingSettings;

                if (!string.IsNullOrEmpty(lastRunTime) && string.IsNullOrEmpty(lastRunDate))
                {
                    timeSettings.PreviousRunTime = lastRunTime;
                }                

                if (lastRunDate != timeSettings.PreviousRunDate && !string.IsNullOrEmpty(lastRunDate)  && string.IsNullOrEmpty(lastRunTime))
                {
                    timeSettings.PreviousRunDate = lastRunDate;
                }
                 
                string updatedJson = JsonConvert.SerializeObject(jsonData, Formatting.Indented);

                File.WriteAllText(@".\Config\appsettings.json", updatedJson);
                WriteLogClass.WriteToLog(1, "Config file updated ....", 1);
                return true;
            }
            catch (Exception ex)
            {
                WriteLogClass.WriteToLog(0, $"Error at configupdater: {ex.Message}", 0);
                return false;
            }            
        }
    }
}
