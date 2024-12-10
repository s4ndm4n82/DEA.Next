namespace DEA.Next.FileOperations.TpsJsonStringClasses;

internal static class TpsJsonDataFileUploadString
{
    public class TpsJsonDataFileUploadObject
    {
        public string Token { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string ID { get; set; } = string.Empty;
        public string Encoding { get; set; } = "UTF-8";
        public string FileData { get; set; } = string.Empty;
    }
}