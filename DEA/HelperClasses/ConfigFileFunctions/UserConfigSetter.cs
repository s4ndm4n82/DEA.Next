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
            public string Token { get; set; }
            public string UserName { get; set; }
            public string TemplateKey { get; set; }
            public string Queue { get; set; }
            public string ProjectId { get; set; }
            public string DocumentId { get; set; }
            public string DocumentEncoding { get; set; } = "UTF-8";
            public int MaxBatchSize { get; set; }
            public int RenameFile { get; set; }
            public string MainCustomer { get; set; }
            public string ClientName { get; set; }
            public int SendEmail { get; set; }
            public string ClientOrgNo { get; set; }
            public string ClientIdField { get; set; }
            public string IdField2Value { get; set; }
            public string ClientIdField2 { get; set; }
            public string FileDeliveryMethod { get; set; }
            public Domaindetails DomainDetails { get; set; }
            public Readcontentsettings ReadContentSettings { get; set; }
            public Ftpdetails FtpDetails { get; set; }
            public Emaildetails EmailDetails { get; set; }
            public Documentdetails DocumentDetails { get; set; }
        }
        public class Domaindetails
        {
            public string MainDomain { get; set; }
            public string TpsRequestUrl { get; set; }
        }
        public class Readcontentsettings
        {
            public bool ReadTheContent { get; set; }
            public string ReadByLineTrigger { get; set; } = string.Empty;
            public bool MakeUploadFile { get; set; }
            public string OutputFileExtension { get; set; } = "pdf";
            public string SetDelimiter { get; set; } = string.Empty;
            public int NumberOfLinesToRead { get; set; }
            public string GeneratedField { get; set; } = string.Empty;
            public string GroupDataBy { get; set; } = string.Empty;
            public bool RemoveGroupDataField { get; set; }
            public bool GeneratedFieldToEnd { get; set; }
            public string[] MainFieldNameList { get; set; } = Array.Empty<string>();
            public string[] MainFieldToSkip { get; set; } = Array.Empty<string>();
            public string[] LineFieldNameList { get; set; } = Array.Empty<string>();
            public string[] LineFieldToSkip { get; set; } = Array.Empty<string>();
        }
        public class Ftpdetails
        {
            public string FtpType { get; set; }
            public string FtpProfile { get; set; }
            public string FtpHostName { get; set; }
            public string FtpUser { get; set; }
            public string FtpPassword { get; set; }
            public int FtpPort { get; set; }
            public int FtpFolderLoop { get; set; }
            public string FtpMainFolder { get; set; }
            public bool FtpMoveToSubFolder { get; set; }
            public string FtpSubFolder { get; set; }
        }

        public class Emaildetails
        {
            public string EmailAddress { get; set; }
            public string EmailInboxPath { get; set; }
            public int EmailRead { get; set; }
            public List<string> EmailList { get; set; }
            public List<Emailfieldlist> EmailFieldList { get; set; }
        }

        public class Emailfieldlist
        {
            public int FieldId { get; set; }
            public string FieldName { get; set; }
        }

        public class Documentdetails
        {
            public string DocumentType { get; set; }
            public List<string> DocumentExtensions { get; set; }
        }

        public static async Task<T> ReadUserDotConfigAsync<T>(string userConfigFilePath = @".\Config\CustomerConfig.json")
        {
            try
            {
                using StreamReader fileData = new(userConfigFilePath);
                var userConfigData = await fileData.ReadToEndAsync();
                var jsonData = JsonConvert.DeserializeObject<T>(userConfigData);

                return jsonData ?? throw new InvalidOperationException("Failed to deserialize JSON");
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
