using Newtonsoft.Json;
using WriteLog;

namespace AppConfigReader
{
    public class AppConfigReaderClass
    {

        public class AppSettingsRoot
        {
            public Programsettings ProgramSettings { get; set; }
            public Emailserversettings EmailServerSettings { get; set; }
            public Timingsettings TimingSettings { get; set; }
            public Graphconfig GraphConfig { get; set; }
        }

        public class Programsettings
        {            
            public int MaxEmails { get; set; }
            public int MaxMainEmailFolders { get; set; }
            public int MaxSubEmailFolders { get; set; }
            public int MaxBatchSize { get; set; }
            public int MaxErrorFolders { get; set; }
            public bool SendErrorEmail { get; set; }
            public bool CleanLogs { get; set; }
        }

        public class Emailserversettings
        {
            public Serversettings ServerSettings { get; set; }
            public Credntials Credntials { get; set; }
            public Emailsettings EmailSettings { get; set; }
        }

        public class Serversettings
        {
            public string SmtpServer { get; set; }
            public int Port { get; set; }
        }

        public class Credntials
        {
            public string UserName { get; set; }
            public string Password { get; set; }
        }

        public class Emailsettings
        {
            public string FromEmail { get; set; }
            public string ReplyEmail { get; set; }
            public string[] ToEmail { get; set; }
            public string Subject { get; set; }
        }

        public class Timingsettings
        {
            public int ErrorCheckInterval { get; set; }
            public int LogsDeleteAfter { get; set; }
            public string PreviousRunTime { get; set; }
            public string PreviousRunDate { get; set; }
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
