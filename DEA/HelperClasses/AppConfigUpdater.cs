using AppConfigReader;
using Newtonsoft.Json;
using WriteLog;

namespace AppConfigUpdater
{
    internal class AppConfigUpdaterClass
    {
        public static void UpdateConfigFile(string lastRunTime, string lastRunDate)
        {
            try
            {
                AppConfigReaderClass.AppSettingsRoot jsonData = AppConfigReaderClass.ReadAppDotConfig();
                AppConfigReaderClass.Timingsettings timeSettings = jsonData.TimingSettings;

                if (!string.IsNullOrEmpty(lastRunTime) || string.IsNullOrEmpty(lastRunDate))
                {
                    timeSettings.PreviousRunTime = lastRunTime;
                }                

                if (lastRunDate != timeSettings.PreviousRunDate || string.IsNullOrEmpty(lastRunDate))
                {
                    timeSettings.PreviousRunDate = lastRunDate;
                }
                 
                string updatedJson = JsonConvert.SerializeObject(jsonData, Formatting.Indented);

                File.WriteAllText(@".\Config\appsettings.json", updatedJson);
                WriteLogClass.WriteToLog(1, "Config file updated with recent time and date.", 1);
            }
            catch (Exception ex)
            {
                WriteLogClass.WriteToLog(0, $"Error at configupdater: {ex.Message}", 0);
            }            
        }
    }
}
