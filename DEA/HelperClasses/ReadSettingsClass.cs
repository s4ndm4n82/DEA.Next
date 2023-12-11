using WriteLog;

namespace ReadSettings
{
    public class ReadSettingsClass
    {
        // Read the dea.conf file and adds the line into DeaConfig array.
        public string[] ReadConfig()
        {
            string[] DeaConfigs = { };

            try
            {
                var ConfigFolderPath = Directory.GetCurrentDirectory(); // Working Directory.
                var ConfigFileName = @".\Config\dea.conf"; // Config file name.
                var ConfigFileFullPath = Path.Combine(ConfigFolderPath, ConfigFileName); // Makes the config file path.
                DeaConfigs = File.ReadAllLines(ConfigFileFullPath);
            }
            catch (Exception ex)
            {
                WriteLogClass.WriteToLog(0, $"Exception at reading the conf file: {ex.Message}", 0);
            }

            return DeaConfigs;
        }

        private string ReturnConfValue(string SearchTerm)
        {
            var ConfigValue = string.Empty;

            try
            {
                string[] ConfigData = ReadConfig();
                int pos = Array.FindIndex(ConfigData, row => row.Contains($"{SearchTerm}"));
                ConfigValue = ConfigData[pos].Replace(" ", "").Split('=').Last();                
            }
            catch (Exception ex)
            {
                WriteLogClass.WriteToLog(0, $"Exception at caonf value return: {ex.Message}", 0);
            }

            return ConfigValue;
        }

        public bool DateFilter
        {
            get
            {
                return bool.Parse(ReturnConfValue("DateFilter"));                
            }
        }
        public int MaxLoadEmails
        {
            get
            {   
                return int.Parse(ReturnConfValue("MaxLoadEmails"));
            }
        }
        public string[]? UserAccounts
        {
            get
            {
                string[]? Emails;

                Emails = ReturnConfValue("UserAccounts").Split(",");
                List<string> EmailList = new List<string>(Emails.Length);
                EmailList.AddRange(Emails);
                return EmailList.ToArray();
            }
        }
        public string ImportFolderLetter
        {
            get
            {   
                return ReturnConfValue("ImportFolderLetter");
            }
        }
        public string ImportFolderPath
        {
            get
            {
                return ReturnConfValue("ImportFolderPath");
            }
        }
        public string[] AllowedExtentions
        {
            get
            {
                string[]? exts;

                exts = ReturnConfValue("AllowedExtentions").Split(",");
                List<string> extList = new List<string>(exts.Length);
                extList.AddRange(exts);
                return exts.ToArray();
            }
        }
    }
}
