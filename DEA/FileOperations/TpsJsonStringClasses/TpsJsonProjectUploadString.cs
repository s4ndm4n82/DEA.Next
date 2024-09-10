namespace TpsJsonProjectUploadString
{
    internal class TpsJsonProjectUploadStringClass
    {
        public class TpsJsonProjectUploadObject
        {
            public string Token { get; set; }
            public string Username { get; set; }
            public object TemplateKey { get; set; }
            public string Queue { get; set; }
            public string ProjectID { get; set; }
            public List<FieldList> Fields { get; set; }
            public List<FileList> Files { get; set; }

        }

        public class FileList
        {
            public string Name { get; set; }
            public string Data { get; set; }
        }

        public class FieldList
        {
            public string Name { get; set;}
            public string Value { get; set; }
        }

    }
}
