﻿using System.Text.Json;

namespace UserConfigReader
{
    internal class UserConfigReaderClass
    {


        public class CustomerDetailsObject
        {
            public Customerdetail[]? CustomerDetails { get; set; }
        }

        public class Customerdetail
        {
            public int id { get; set; }
            public string? Token { get; set; }
            public string? UserName { get; set; }
            public object? TemplateKey { get; set; }
            public string? Queue { get; set; }
            public string? ProjetID { get; set; }
            public string? MainCustomer { get; set; }
            public string? ClientName { get; set; }
            public string? FileDeliveryMethod { get; set; }
            public Ftpdetails? FtpDetails { get; set; }
            public Emaildetails? EmailDetails { get; set; }
            public Documentdetails? DocumentDetails { get; set; }
        }

        public class Ftpdetails
        {
            public string? FtpType { get; set; }
            public string? FtpHostName { get; set; }
            public string? FtpHostIp { get; set; }
            public string? FtpUser { get; set; }
            public string? FtpPassword { get; set; }
            public string? FtpMainFolder { get; set; }
            public string? FtpSubFolder { get; set; }
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
            public string? DocumentExtension { get; set; }
        }

        public static async Task<T> ReadAppDotConfig<T>()
        {
            using FileStream ReadFile = File.OpenRead(@".\Config\CustomerConfig.json");
            T? jsonData = await JsonSerializer.DeserializeAsync<T>(ReadFile);

            return jsonData!;
        }
    }
}