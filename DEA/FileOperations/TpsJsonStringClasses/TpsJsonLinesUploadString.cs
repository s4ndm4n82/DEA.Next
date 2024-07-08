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
        public Table Table { get; set; }
    }
    
    public class Table
    {
        public Row[] Rows { get; set; }
    }
    
    public class Rows
    {
        public Row Row { get; set; }
    }
    
    public class Row
    {
        public Fields1[] Fields { get; set; }
    }
    
    public class Fields1
    {
        public Field Field { get; set; }
    }
    
    public class Field
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