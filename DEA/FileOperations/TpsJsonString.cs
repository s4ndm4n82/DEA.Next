using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TpsJsonString
{
    internal class TpsJasonStringClass
    {
        public class TpsJsonObject
        {
            public string? Token { get; set; }
            public string? Username { get; set; }
            public object? TemplateKey { get; set; }
            public string? Queue { get; set; }
            public string? ProjectID { get; set; }
            public List<FileList>? Files { get; set; }
        }

        public class FileList
        {
            public string? Name { get; set; }
            public string? Data { get; set; }
        }

    }
}
