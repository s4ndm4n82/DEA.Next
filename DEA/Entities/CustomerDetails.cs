using UserConfigSetterClass;

namespace DEA.Next.Entities;

public class CustomerDetails
{
    public Guid Id { get; set; }
    public bool Status { get; set; } // True = Active, False = Inactive Used to be known as CustomerStatus.
    public required string CustomerName { get; set; } // Used to be known as MainCustomerName.
    public required string UserName { get; set; }
    public required string Token { get; set; }
    public int Queue { get; set; }
    public string ProjectId { get; set; } = string.Empty;
    public string TemplateKey { get; set; } = string.Empty;
    public string DocumentId { get; set; } = string.Empty;
    public int MaxBatchSize { get; set; }
    public bool SendEmail { get; set; }
    public bool SendSubject { get; set; }
    public string FieldOneValue { get; set; } = string.Empty;
    public string FieldOneName { get; set; } = string.Empty;
    public string FieldTwoValue { get; set; } = string.Empty;
    public string FieldTwoName { get; set; } = string.Empty;
    public required string Domain { get; set; }
    public List<Documentdetails> DocumentDetails { get; set; } = [];
    public List<FtpDetails> FtpDetails { get; set; } = [];
    public List<Emaildetails> EmailDetails { get; set; } = [];
}