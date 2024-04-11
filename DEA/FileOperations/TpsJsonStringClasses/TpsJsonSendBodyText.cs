namespace DEA.Next.FileOperations.TpsJsonStringClasses
{
    internal class TpsJsonSendBodyTextClass
    {
        public class TpsJsonSendBodyText
        {
            public string Token { get; set; }
            public string Username { get; set; }
            public object TemplateKey { get; set; }
            public string Queue { get; set; }
            public string ProjectID { get; set; }
            public List<Emailfieldlist> EmailFieldList { get; set; }
        }

        public class Emailfieldlist
        {
            public string Name { get; set; }
            public string Value { get; set; }
        }
    }
}
