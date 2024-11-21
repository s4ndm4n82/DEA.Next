using DEA.Next.HelperClasses.OtherFunctions;
using Newtonsoft.Json;
using WriteLog;

namespace UserConfigSetterClass
{
    public class UserConfigSetter
    {
        public class CustomerDetailsObject
        {
            public Customerdetail[]? CustomerDetails { get; set; }
        }

        public class Customerdetail
        {
            public int Id { get; set; }
            public int CustomerStatus { get; set; }
            public string Token { get; set; } = string.Empty;
            public string UserName { get; set; } = string.Empty;
            public string TemplateKey { get; set; } = string.Empty;
            public string Queue { get; set; } = string.Empty;
            public string ProjectID { get; set; } = string.Empty;
            public string DocumentId { get; set; } = string.Empty;
            public string DocumentEncoding { get; set; } = "UTF-8";
            public int MaxBatchSize { get; set; }
            public int RenameFile { get; set; }
            public string MainCustomer { get; set; } = string.Empty;
            public string ClientName { get; set; } = string.Empty;
            public int SendEmail { get; set; }
            public string ClientOrgNo { get; set; } = string.Empty;
            public string ClientIdField { get; set; } = string.Empty;
            public string IdField2Value { get; set; } = string.Empty;
            public string ClientIdField2 { get; set; } = string.Empty;
            public string FileDeliveryMethod { get; set; } = string.Empty;
            public Domaindetails DomainDetails { get; set; } = new();
            public Ftpdetails FtpDetails { get; set; } = new();
            public Emaildetails EmailDetails { get; set; } = new();
            public Documentdetails DocumentDetails { get; set; } = new();
        }
        public class Domaindetails
        {
            public string MainDomain { get; set; } = string.Empty;
            public string TpsRequestUrl { get; set; } = string.Empty;
        }
        public class Ftpdetails
        {
            public string FtpType { get; set; } = MagicWords.ftp;
            public string FtpProfile { get; set; } = string.Empty;
            public string FtpHostName { get; set; } = string.Empty;
            public string FtpUser { get; set; } = string.Empty;
            public string FtpPassword { get; set; } = string.Empty;
            public int FtpPort { get; set; }
            public int FtpFolderLoop { get; set; }
            public string FtpMainFolder { get; set; } = string.Empty;
            public bool FtpMoveToSubFolder { get; set; }
            public string FtpSubFolder { get; set; } = string.Empty;
            public bool FtpRemoveFiles { get; set; } = true;
        }

        public class Emaildetails
        {
            public string EmailAddress { get; set; } = string.Empty;
            public string EmailInboxPath { get; set; } = string.Empty;
            public int EmailRead { get; set; }
            public List<string> EmailList { get; set; }
            public List<Emailfieldlist> EmailFieldList { get; set; }
        }

        public class Emailfieldlist
        {
            public int FieldId { get; set; }
            public string FieldName { get; set; } = string.Empty;
        }

        public class Documentdetails
        {
            public string DocumentType { get; set; } = string.Empty;
            public List<string> DocumentExtensions { get; set; } = new();
        }

        public static async Task<T> ReadUserDotConfigAsync<T>(string userConfigFilePath = @".\Config\CustomerConfig.json")
        {
            try
            {
                using StreamReader fileData = new(userConfigFilePath);
                var userConfigData = await fileData.ReadToEndAsync();
                var jsonData = string.IsNullOrEmpty(userConfigData)
                    ? default(T) :
                    JsonConvert.DeserializeObject<T>(userConfigData);
                
                return jsonData ?? default(T)!;
            }
            catch (IOException ioEx)
            {
                WriteLogClass.WriteToLog(0, $"IOException at Json reader: {ioEx.Message}", 0);
                // Consider re-throwing if the application cannot function without this configuration.
                throw;
            }
            catch (JsonException jsonEx)
            {
                WriteLogClass.WriteToLog(0, $"JsonException at Json reader: {jsonEx.Message}", 0);
                // Consider re-throwing if the application cannot function without this configuration.
                throw;
            }
        }
    }
}
