using Newtonsoft.Json;
using WriteLog;

namespace UserConfigReader
{
    public class UserConfigReaderClass
    {
        public class CustomerDetailsObject
        {
            public Customerdetail[]? CustomerDetails { get; set; }
        }

        public class Customerdetail
        {
            public int Id { get; set; }
            public int CustomerStatus { get; set; }
            public string? Token { get; set; }
            public string? UserName { get; set; }
            public string? TemplateKey { get; set; }
            public string? Queue { get; set; }
            public string? ProjetID { get; set; }
            public string? MainCustomer { get; set; }
            public string? ClientName { get; set; }
            public string? ClientOrgNo { get; set; }
            public string? ClientIdField { get; set; }
            public string? FileDeliveryMethod { get; set; }
            public Domaindetails DomainDetails { get; set; }
            public Ftpdetails? FtpDetails { get; set; }
            public Emaildetails? EmailDetails { get; set; }
            public Documentdetails? DocumentDetails { get; set; }
        }
        public class Domaindetails
        {
            public string MainDomain { get; set; }
            public string TpsRequestUrl { get; set; }
        }
        public class Ftpdetails
        {
            public string? FtpType { get; set; }
            public string? FtpHostName { get; set; }
            public string? FtpHostIp { get; set; }
            public string? FtpUser { get; set; }
            public string? FtpPassword { get; set; }
            public string? FtpMainFolder { get; set; }
            public int FtpFolderLoop { get; set; }
        }

        public class Emaildetails
        {
            public string? EmailAddress { get; set; }
            public string? MainInbox { get; set; }
            public string? SubInbox1 { get; set; }
            public string? SubInbox2 { get; set; }
        }

        public class Documentdetails
        {
            public string? DocumentType { get; set; }
            public List<string>? DocumentExtensions { get; set; }
        }

        public static T ReadUserDotConfig<T>()
        {
            T? jsonData = default;
            try
            {
                using StreamReader fileData = new StreamReader(@".\Config\CustomerConfig.json");
                string stringData = fileData.ReadToEnd();
                jsonData = JsonConvert.DeserializeObject<T>(stringData);

                return jsonData!;
            }
            catch (Exception ex)
            {
                WriteLogClass.WriteToLog(0, $"Exception at Json reader: {ex.Message}", 0);
                return jsonData!;
            }
        }
    }
}
