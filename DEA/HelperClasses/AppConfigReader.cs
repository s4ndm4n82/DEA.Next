using Newtonsoft.Json;
using WriteLog;

namespace AppConfigReader
{
    internal class AppConfigReaderClass
    {

        public class AppSettingsRoot
        {
            public Programsettings ProgramSettings { get; set; }
            public Graphconfig GraphConfig { get; set; }
        }

        public class Programsettings
        {
            public int MaxEmails { get; set; }
        }

        public class Graphconfig
        {
            public string TenantId { get; set; }
            public string Instance { get; set; }
            public string GraphApiUrl { get; set; }
            public string ClientSecret { get; set; }
            public string ClientId { get; set; }
            public string[] Scopes { get; set; }
        }


        public static AppSettingsRoot ReadAppDotConfig()
        {
            AppSettingsRoot? jsonAppData = default;
            try
            {
                string jsonFileData = File.ReadAllText(@".\Config\appsettings.json");
                return jsonAppData = JsonConvert.DeserializeObject<AppSettingsRoot>(jsonFileData)!;
            }
            catch (Exception ex)
            {
                WriteLogClass.WriteToLog(0, $"Exception at reading app dot config: {ex.Message}", 0);
                return jsonAppData!;
            }
        }
    }
}
