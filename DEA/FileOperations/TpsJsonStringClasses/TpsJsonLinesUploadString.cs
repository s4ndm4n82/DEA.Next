namespace DEA.Next.FileOperations.TpsJsonStringClasses;

public class TpsJsonLinesUploadString
{
    public class TpsJsonLinesUploadObject
    {
        public string Token { get; set; }
        public string Username { get; set; }
        public string TemplateKey { get; set; }
        public string Queue { get; set; }
        public string ProjectID { get; set; }
        public Fields[] Fields { get; set; }
        public Tables[] Tables { get; set; }
        public Files[] Files { get; set; }
    }

    public class Fields
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }

    public class Tables
    {
        public Rows[] Rows { get; set; }
    }

    public class Rows
    {
        public Fields1[] Fields { get; set; }
    }

    public class Fields1
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }

    public class Files
    {
        public string Name { get; set; }
        public string Data { get; set; }
    }
}