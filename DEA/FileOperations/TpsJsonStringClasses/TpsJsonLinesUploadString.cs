namespace DEA.Next.FileOperations.TpsJsonStringClasses;

public abstract class TpsJsonLinesUploadString
{
    public class TpsJsonLinesUploadObject
    {
        public string Token { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string TemplateKey { get; set; } = string.Empty;
        public string Queue { get; set; } = string.Empty;
        public string ProjectId { get; set; } = string.Empty;
        public Fields[] Fields { get; set; } = Array.Empty<Fields>();
        public Tables[] Tables { get; set; } = Array.Empty<Tables>();
        public Files[] Files { get; set; } = Array.Empty<Files>();
    }

    public class Fields
    {
        public string Name { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }

    public class Tables
    {
        public Rows[] Rows { get; set; } = Array.Empty<Rows>();
    }

    public class Rows
    {
        public Fields1[] Fields { get; init; } = Array.Empty<Fields1>();
    }

    public class Fields1
    {
        public string Name { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }

    public class Files
    {
        public string Name { get; set; } = string.Empty;
        public string Data { get; set; } = string.Empty;
    }
}