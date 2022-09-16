using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using WriteLog;

namespace UserConfigReader
{
    internal class UserConfigReaderClass
    {

        public class Rootobject
        {
            public Customerdetail[] CustomerDetails { get; set; }
        }

        public class Customerdetail
        {
            public int id { get; set; }
            public string MainCustomer { get; set; }
            public string ClientName { get; set; }
            public string FileDeliveryMethod { get; set; }
            public Ftpdetails FtpDetails { get; set; }
            public Emaildetails EmailDetails { get; set; }
            public Documentdetails DocumentDetails { get; set; }
        }

        public class Ftpdetails
        {
            public string FtpType { get; set; }
            public string FtpUser { get; set; }
            public string FtpPassword { get; set; }
            public string FtpMainFolder { get; set; }
            public string FtpSubFolder { get; set; }
        }

        public class Emaildetails
        {
            public string EmailAddress { get; set; }
            public string MainInbox { get; set; }
            public string SubInbox1 { get; set; }
            public string SubInbox2 { get; set; }
        }

        public class Documentdetails
        {
            public string DocumentType { get; set; }
            public string DocumentExtension { get; set; }
        }

        public static void ReadAppDotConfig()
        {
            StreamReader ReadData = new StreamReader(@".\Config\CustomerConfig.json");
        }
    }
}
