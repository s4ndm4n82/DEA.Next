namespace DEA.Next.FileOperations.TpsJsonStringClasses;

internal class TpsJsonSendBodyTextClass
{
    public class TpsJsonSendBodyText
    {
        public required string Token { get; set; }
        public required string Username { get; set; }
        public string TemplateKey { get; set; } = string.Empty;
        public required int Queue { get; set; }
        public required string ProjectId { get; set; }
        public List<FieldList> Fields { get; set; } = [];
        public List<FileList> Files { get; set; } = [];
    }

    public class FieldList
    {
        public string Name { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }

    public class FileList
    {
        public string Name { get; set; } = string.Empty;
        public string Data { get; set; } = string.Empty;
    }
}